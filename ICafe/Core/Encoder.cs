using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Text.Json;
using Microsoft.Win32;

namespace ICafe.Core
{
    public static class Encoder
    {
        const string start_pattern = "<{";
        const string end_pattern = "}/>";
        const string search_pattern = "(?s)(.*)";

        public struct NodeData
        {
            public Guid id;
            public List<ConnectionData> connections;
            public Point position;
        }

        public struct ConnectionData
        {
            public int node_id;
            public string field_name, parameter_name;
            public int parameter_index;
        }

        [Serializable]
        public struct Test
        {
            public int a { get; set; }
        }

        public static string Encode(List<Node> nodes, List<Point> positions)
        {
            if (nodes == null || positions == null || nodes.Count != positions.Count) return "";

            StringBuilder builder = new StringBuilder();

            //write ids
            builder.Append(start_pattern);

            Dictionary<Guid, int> ids = new Dictionary<Guid, int>();
            for (int i = 0; i < nodes.Count; i++)
            {
                ids.Add(nodes[i].ID, i);
                builder.Append(nodes[i].ID + "|");
            }
            builder.Append(end_pattern);
            builder.Append(Environment.NewLine);

            //write nodes
            for (int i = 0; i < nodes.Count; i++)
            {
                Node n = nodes[i];

                //open
                builder.Append(Environment.NewLine);
                builder.Append(start_pattern);

                //type
                builder.Append("[type:" + n.GetType() + "]");

                //id
                builder.Append("[id:" + ids[n.ID] + "]");

                //parameters
                builder.Append("[parameters:");
                var parameters = n.GetParameters();
                for (int j = 0; j < parameters.Length; j++)
                {
                    builder.Append("(" + parameters[j].Parameter.Name + "=");
                    var inputs = parameters[j].Inputs;
                    int count = parameters[j].Count;

                    if (inputs == null || count == 0)
                    {
                        builder.Append(")");
                        continue;
                    }
                    for (int k = 0; k < inputs.Length; k++)
                    {
                        if (inputs[k] != null)
                            builder.Append(ids[inputs[k].Node.ID] + "-" + inputs[k].Field.field.Name + "-" + k + "|");
                    }

                    builder.Append(")");
                }
                builder.Append("]");

                //position
                builder.Append("[position:" + positions[i] + "]");
                builder.Append(Environment.NewLine);

                //values
                builder.Append("VALUES");
                var fields = n.GetFields();
                for (int j = 0; j < fields.Length; j++)
                {
                    if (Node.IsTypeValid(fields[j].Field.field.FieldType))
                    {
                        builder.Append(Environment.NewLine);
                        builder.Append(EncodeField(fields[j].Field, ","));
                    }
                }

                //close
                builder.Append(Environment.NewLine);
                builder.Append(end_pattern);
                builder.Append(Environment.NewLine);
            }

            return builder.ToString();
        }

        static string EncodeField(ReactiveField field, string separator)
        {
            string str = null;

            if (field.internal_fields.Count > 0)
            {
                StringBuilder b = new StringBuilder();

                b.Append("{");
                for (int k = 0; k < field.internal_fields.Count; k++)
                    b.Append(EncodeField(field.internal_fields[k], "|"));
                b.Append("}");

                str = b.ToString();
            }
            else
            {
                bool is_string = field.field.FieldType == typeof(string) || field.field.FieldType == typeof(char);

                if (field.Property == null) str = "";
                else str = is_string ? '"' + field.Property.ToString() + '"' : field.Property.ToString();
            }

            return field.field.Name + "=" + str + separator;
        }

        public static int Decode(string data, out List<Node> nodes, out Dictionary<Node, Point> positions)
        {
            nodes = new List<Node>();
            positions = new Dictionary<Node, Point>();

            var matches = SplitByValues(data, start_pattern, end_pattern);

            List<NodeData> datas = new List<NodeData>();

            Dictionary<int, Guid> ids = null;

            int output = CreateNodes(matches, ref ids, ref nodes, ref datas);
            if (output != 0)
                return output;

            CreateConnections(ids, nodes, datas, ref positions);

            return 0;
        }

        static int CreateNodes(List<string> matches, ref Dictionary<int, Guid> ids, ref List<Node> nodes, ref List<NodeData> datas)
        {
            bool first = true;
            foreach (string match in matches)
            {
                //substring patterns
                string str = match.Substring(start_pattern.Length, match.Length - end_pattern.Length - start_pattern.Length);

                if (first)
                {
                    //dictionary
                    first = false;
                    ids = BuildDictionary(str);

                    if (ids == null) return 2;
                    if (ids.Count == 0) return 1;
                }
                else
                {
                    //create nodes
                    Node n = null;
                    NodeData nd = new NodeData();

                    n = ExtractNode(match, ids, ref nd);
                    if (n != null)
                    {
                        nodes.Add(n);
                        datas.Add(nd);

                        SetNodeValues(match, n);
                    }
                }
            }
            return 0;
        }

        static Dictionary<int, Guid> BuildDictionary(string data)
        {
            if (data == null) return null;

            var dict = new Dictionary<int, Guid>();

            string[] entries = data.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < entries.Length; i++)
            {
                Guid id = new Guid(entries[i]);
                dict.Add(i, id);
            }

            return dict;
        }

        static Node ExtractNode(string data, Dictionary<int, Guid> dict, ref NodeData nd)
        {
            //divide by [...]
            List<string> matches = SplitByValues(data, "[", "]");

            string[] passes = new string[] { "type", "id", "parameters", "position" };

            Node node = null;

            int index = 0;
            foreach (string match in matches)
            {
                string[] content = match.Substring(1, match.Length - 2).Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                if (content.Length < 2)
                {
                    index++;
                    continue;
                }

                string content_id = content[0];
                string content_data = content[1];

                if (content_id == passes[index])
                {
                    switch (index)
                    {
                        //instatiate from type
                        case 0:
                            node = Node.CreateNodeFromTypeName(content_data, true) as Node;
                            if (node == null) return null;
                            break;

                        //initialize
                        case 1:
                            int id;
                            if (int.TryParse(content_data, out id))
                                nd.id = node.Initialize(dict[id]);
                            else return null;
                            break;

                        case 2:
                            if (!GetParameters(content_data, node, ref nd))
                                return null;
                            break;

                        case 3:
                            if (!GetPosition(content_data, out nd.position))
                                return null;
                            break;
                    }
                }
                index++;
            }
            return node;
        }

        static bool GetParameters(string data, Node node, ref NodeData nd)
        {
            nd.connections = new List<ConnectionData>();

            List<string> matches = SplitByValues(data, "(", ")");

            foreach (string match in matches)
            {
                string[] str = match.Substring(1, match.Length - 2).Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                if (str.Length < 2) continue;

                var param = node.GetParameter(str[0]);
                if (param == null)
                    continue;

                string[] content = str[1].Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < content.Length; i++)
                {
                    string[] values = content[i].Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);

                    int id;
                    if (!int.TryParse(values[0], out id))
                        continue;

                    ConnectionData cd = new ConnectionData();
                    cd.node_id = id;
                    cd.parameter_name = str[0];
                    cd.field_name = values[1];
                    cd.parameter_index = int.Parse(values[2]);

                    nd.connections.Add(cd);
                }
            }

            return true;
        }

        static bool GetPosition(string data, out Point p)
        {
            p = new Point(0, 0);

            string[] strs = data.Split(';');
            if (strs.Length < 2)
                return false;

            double x, y;
            if (!double.TryParse(strs[0], out x))
                return false;
            if (!double.TryParse(strs[1], out y))
                return false;

            p = new Point(x, y);
            return true;
        }

        static void CreateConnections(Dictionary<int, Guid> ids, List<Node> nodes, List<NodeData> datas, ref Dictionary<Node, Point> positions)
        {
            //create dictionary
            Dictionary<Guid, Node> dict = new Dictionary<Guid, Node>();
            for (int i = 0; i < nodes.Count; i++)
            {
                Guid id = ids[i];
                for (int j = 0; j < nodes.Count; j++)
                {
                    if (nodes[j].ID == id)
                        dict.Add(id, nodes[j]);
                }
            }

            //set connections
            for (int i = 0; i < datas.Count; i++)
            {
                NodeData data = datas[i];
                Guid id = data.id;

                if (!dict.ContainsKey(id)) continue;

                Node n = dict[id];
                if (data.connections != null)
                {
                    for (int j = 0; j < data.connections.Count; j++)
                    {
                        int int_id = data.connections[j].node_id;
                        if (!ids.ContainsKey(int_id)) continue;

                        Guid guid = ids[int_id];
                        if (!dict.ContainsKey(guid)) continue;

                        bool b = false;
                        Node other = dict[guid];
                        Connector.CreateConnection(other, data.connections[j].field_name, n, data.connections[j].parameter_name, ref b, data.connections[j].parameter_index);
                    }
                }

                if (!positions.ContainsKey(n))
                    positions.Add(n, data.position);
            }
        }

        static List<string> SplitByValues(string data, string start, string end)
        {
            List<string> str = new List<string>();

            bool open = true;
            int s_index = 0;
            int internal_count = 0;
            int search_start = 0;

            bool valid = true;
            while (valid)
            {
                int index = data.IndexOf(open ? start : end, search_start);
                if (index == -1)
                    valid = false;
                else
                {
                    int other_index = data.IndexOf(!open ? start : end, search_start);

                    if (other_index != -1 && index > other_index)
                    {
                        if (!open)
                            internal_count++;
                        search_start = other_index + 1;
                    }
                    else
                    {
                        if (internal_count > 0)
                        {
                            internal_count--;
                            search_start = index + 1;
                        }
                        else
                        {
                            if (open)
                            {
                                s_index = index;
                                search_start = index + 1;
                            }
                            else
                            {
                                int i = index + end.Length;
                                str.Add(data.Substring(s_index, i - s_index));
                                data = data.Substring(i, data.Length - i);
                                search_start = 0;
                            }
                            open = !open;
                        }
                    }
                }
            }

            return str;
        }

        static List<string> Split(string data, char separator, bool ignore_enclosed_fields, bool remove_empty_entries)
        {
            List<string> str = new List<string>();

            bool enclose_open = false;
            int ignore_start = 0;

            bool valid = true;
            while (valid)
            {
                if (enclose_open)
                {
                    int ignore_index = data.IndexOf('"', ignore_start);
                    if (ignore_index == -1) return str;

                    string d = data.Substring(0, ignore_index + 1);
                    if (d != "")
                        str.Add(d);
                    data = data.Substring(ignore_index + 2, data.Length - ignore_index - 2);
                    enclose_open = false;
                }
                else
                {
                    int index = data.IndexOf(separator);
                    if (index == -1)
                    {
                        if (data != "")
                            str.Add(data);
                        valid = false;
                    }
                    else
                    {
                        int ignore_index = data.IndexOf('"');
                        if (ignore_index != -1 && ignore_index < index)
                        {
                            enclose_open = true;
                            ignore_start = ignore_index + 1;
                        }
                        else
                        {
                            string d = data.Substring(0, index);
                            if (d != "")
                                str.Add(d);
                            data = data.Substring(index + 1, data.Length - index - 1);
                        }
                    }
                }
            }

            return str;
        }

        static void SetNodeValues(string data, Node node)
        {
            string str = data.Substring(start_pattern.Length, data.Length - start_pattern.Length - end_pattern.Length);

            string pat = "VALUES";
            int index = str.IndexOf(pat);
            if (index == -1) return;

            string content = str.Substring(index + pat.Length, str.Length - index - pat.Length);
            content = content.Replace(" ", "");
            content = content.Replace(Environment.NewLine, "");

            string[] fields = Split(content, ',', true, true).ToArray();// content.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            for (int j = 0; j < fields.Length; j++)
            {
                string[] field_data = fields[j].Split(new char[] { '=' }, 2, StringSplitOptions.RemoveEmptyEntries);
                if (field_data.Length < 2) continue;

                string field_name = field_data[0];
                string field_value = field_data[1];

                var f = node.GetField(field_name);
                if (f == null) continue;

                ReadField(f.Field, field_value);
            }
        }

        static void ReadField(ReactiveField field, string data)
        {
            if (data[0] != '{')
                SetField(field, data);
            else
            {
                string str = data.Substring(1, data.Length - 2);

                while (str.Length > 0)
                {
                    int index = str.IndexOf('=');
                    if (index == -1) return;

                    string name = str.Substring(0, index);
                    var f = field.GetInternalField(name);

                    string value;
                    int end = 0;

                    bool is_struct = str[index + 1] == '{';
                    if (is_struct)
                    {
                        value = SplitByValues(str, "{", "}")[0];
                        end = value.Length + 1 + name.Length;
                    }
                    else
                    {
                        end = str.IndexOf('|');
                        value = str.Substring(index + 1, end - index - 1);
                    }

                    if (f != null)
                        ReadField(f, value);
                    str = str.Remove(0, end + 1);
                }
            }

        }

        static void SetField(ReactiveField field, string data)
        {
            try
            {
                if (field.field.FieldType == typeof(string) || field.field.FieldType == typeof(char))
                    data = data.Substring(1, data.Length - 2);

                object obj;
                if (field.field.FieldType.IsEnum)
                    obj = Enum.Parse(field.field.FieldType, data);
                else obj = Convert.ChangeType(data, field.field.FieldType);

                field.Property = obj;
            }
            catch (Exception)
            {
                Console.WriteLine("error: " + field.field.Name);
            }
        }

        static void DecodeField(ReactiveField field, string data)
        {
            if (field.internal_fields != null && field.internal_fields.Count > 0)
            {
                //remove {}
                data = data.Substring(1, data.Length - 2);

                //split by <|>
                string[] entries = data.Split(new string[] { "<|>" }, StringSplitOptions.RemoveEmptyEntries);

                List<string> entries_c = SplitByValues(data, "<|>", "<|>");
                if (entries.Length == 0) return;

                for (int i = 0; i < entries.Length; i++)
                {
                    string[] field_data = entries[i].Split(new char[] { '=' }, 2, StringSplitOptions.RemoveEmptyEntries);
                    if (field_data.Length < 2) continue;

                    string field_name = field_data[0];
                    string field_value = field_data[1];

                    var f = field.GetInternalField(field_name);
                    if (f != null)
                        DecodeField(f, field_value);
                }
            }
            else
            {
                try
                {
                    object obj;
                    if (field.field.FieldType.IsEnum)
                        obj = Enum.Parse(field.field.FieldType, data);
                    else obj = Convert.ChangeType(data, field.field.FieldType);

                    field.Property = obj;
                }
                catch (Exception)
                {
                    Console.WriteLine("error: " + field.field.Name);
                }
            }
        }
    }
}
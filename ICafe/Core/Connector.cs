using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ICafe.Core
{
    public class Connector
    {
        public static Action<Guid, Guid> OnConnectionCreated, OnConnectionDeleted;

        public struct ConnectionData
        {
            public Node.ParameterData input;
            public Node.FieldData output;
        }

        public static bool CreateConnection(ref ConnectionData handler, Node A, string fieldName, Node B, string parameterName)
        {
            var field = A.GetField(fieldName);
            if (field != null)
            {
                var param = B.GetParameter(parameterName);
                if (param != null)
                {
                    if (!Node.CanBeAssigned(field.Field.field.FieldType, param.Parameter.ParameterType))
                        return false;

                    if (param.Input != null)
                        RemoveConnection(B, parameterName);

                    ConnectionData data = new ConnectionData { input = param, output = field };

                    param.Input = field;
                    field.Inputs.Add(param);

                    handler = data;
                    OnConnectionCreated?.Invoke(A.ID, B.ID);
                    return true;
                }
            }

            return false;
        }

        public static void RemoveConnection(ConnectionData data)
        {
            data.input.Input = null;
            data.output.Inputs.Remove(data.input);
        }

        public static void RemoveConnection(Node node, string parameterName)
        {
            Node.ParameterData p = node.GetParameter(parameterName);
            if (p == null || p.Input == null) return;

            p.Input.Inputs.Remove(p);
            Guid id = p.Input.Node.ID;
            p.Input = null;

            OnConnectionDeleted?.Invoke(node.ID, id);
        }

        public static void RemoveConnections(Node output, string fieldName)
        {
            Node.FieldData f = output.GetField(fieldName);
            if (f == null) return;

            var list = f.Inputs;
            for (int i = list.Count - 1; i >= 0; i--)
                RemoveConnection(list[i].Node, list[i].Parameter.Name);
        }

        public static void RemoveAllConnections(Node node)
        {
            var list = node.GetParameters();
            for (int i = 0; i < list.Length; i++)
                RemoveConnection(node, list[i].Parameter.Name);

            var list2 = node.GetFields();
            for (int i = 0; i < list2.Length; i++)
                RemoveConnections(node, list2[i].Field.field.Name);
        }
    }
}

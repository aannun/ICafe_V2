using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICafe.Core
{
    public static class Registry
    {
        struct NodeData
        {
            public string name;
            public Type type;
        }

        static List<NodeData> nodeDatas;

        static Registry()
        {
            InitNodeDatas();
        }

        static void InitNodeDatas()
        {
            nodeDatas = new List<NodeData>();

            var list = ICafe.Core.Node.GetAllNodeTypes();
            for (int i = 0; i < list.Length; i++)
            {
                var instance = (ICafe.Core.Node)Activator.CreateInstance(list[i]);
                NodeData d = new NodeData() { name = instance.GetTypeName(), type = list[i] };
                nodeDatas.Add(d);
            }
        }

        public static Type GetNodeTypeFromName(string node_name)
        {
            foreach (var item in nodeDatas)
            {
                if (item.name == node_name)
                    return item.type;
            }
            return null;
        }

        public static string GetNodeNameFromType(Type node_type)
        {
            foreach (var item in nodeDatas)
            {
                if (item.type == node_type)
                    return item.name;
            }
            return null;
        }
    }
}

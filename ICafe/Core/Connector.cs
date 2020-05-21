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
        public static bool CreateConnection(Node A, string fieldName, Node B, string parameterName, ref bool removedConnection, int parameter_index = 0)
        {
            removedConnection = false;

            var field = A.GetField(fieldName);
            if (field != null)
            {
                var param = B.GetParameter(parameterName);
                if (param != null)
                {
                    if (parameter_index < 0 || parameter_index >= param.Lenght)
                        return false;

                    if (!Node.CanBeAssigned(field.Field.field.FieldType, param.Parameter.ParameterType))
                        return false;

                    if (param.Inputs[parameter_index] == field)
                        return false;

                    if (param.Inputs[parameter_index] != null)
                    {
                        RemoveConnection_Parameter(B, parameterName, parameter_index);
                        removedConnection = true;
                    }

                    field.Inputs.Add(new Node.FieldData.ParameterConnection() { data = param, index = parameter_index });
                    param.Assign(field, parameter_index);

                    return true;
                }
            }

            return false;
        }

        public static void RemoveConnection_Parameter(Node node, string parameterName, int index = 0)
        {
            Node.ParameterData p = node.GetParameter(parameterName);
            if (p == null) return;

            if (p.Inputs[index] != null)
            {
                p.Inputs[index].Remove(p, index);
                p.RemoveAt(index);
            }
        }

        public static void RemoveConnection_Field(Node node, string fieldName)
        {
            Node.FieldData p = node.GetField(fieldName);
            if (p == null) return;

            for (int i = p.Inputs.Count - 1; i >= 0; i--)
            {
                if (p.Inputs.Count == 0 || i >= p.Inputs.Count) return;

                var item = p.Inputs[i];
                if (item.data.IsCollection)
                {
                    var indexes = item.data.GetIndexesFromField(p);
                    foreach (var index in indexes)
                        RemoveConnection_Parameter(item.data.Node, item.data.Parameter.Name, index);
                }
                else
                    RemoveConnection_Parameter(item.data.Node, item.data.Parameter.Name, 0);
            }
        }
    }
}

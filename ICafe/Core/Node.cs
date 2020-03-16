using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

public class Reactive : Attribute
{
}

public class BaseNodeClass : Attribute
{
}

public class ControlNode : Attribute
{
}

namespace ICafe.Core
{
    [BaseNodeClass]
    public class ReactiveField : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public FieldInfo field { get; }
        object obj;

        public List<ReactiveField> internal_fields { get; private set; }

        public ReactiveField(object obj, FieldInfo field)
        {
            this.obj = obj;
            this.field = field;

            InitInternalFields();
        }

        void InitInternalFields()
        {
            var fs = field.FieldType.GetFields(BindingFlags.Public | BindingFlags.Instance);
            if (fs == null)
                return;

            internal_fields = new List<ReactiveField>();
            for (int i = 0; i < fs.Length; i++)
            {
                if (Node.IsTypeValid(fs[i]))
                    internal_fields.Add(new ReactiveField(Property, fs[i]));
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public object Property
        {
            get { return field.GetValue(obj); }
            set
            {
                if (value != field.GetValue(obj))
                {
                    field.SetValue(obj, value);
                    OnPropertyChanged("Property");
                }
            }
        }

        bool active;
        public bool Active
        {
            get { return active; }
            set
            {
                if (value != active)
                {
                    active = value;
                    if (active)
                        (obj as IReactive)?.UpdateReactiveFieldActive(this);
                    OnPropertyChanged("Active");
                }
            }
        }
    }

    public interface IReactive
    {
        void UpdateReactiveFieldActive(ReactiveField field);
    }

    public class Node : IReactive
    {
        public string NodeName;

        public int CurrentFieldIndex { get; private set; }

        public MethodInfo ExecuteInfo { get; private set; }
        public Guid ID { get; private set; }

        Dictionary<string, FieldData> fields;
        Dictionary<string, ParameterData> parameters;
        bool active, valid;

        public class ParameterData
        {
            public ParameterInfo Parameter;
            public FieldData Input;
            public Node Node;
        }

        public class FieldData
        {
            public ReactiveField Field;
            public List<ParameterData> Inputs;
            public Node Node;
        }

        public Node()
        {

        }

        public Guid Initialize()
        {
            ID = Guid.NewGuid();

            active = true;
            NodeName = this.GetType().Name;

            InitializeReactiveFields();
            SetExecute();
            SetInputs();

            return ID;
        }

        void SetExecute()
        {
            ExecuteInfo = this.GetType().GetMethod("Execute");
            valid = ExecuteInfo != null;
        }

        void SetInputs()
        {
            if (!valid) return;

            var param = ExecuteInfo.GetParameters();
            parameters = new Dictionary<string, ParameterData>();

            for (int i = 0; i < param.Length; i++)
                parameters.Add(param[i].Name, new ParameterData { Parameter = param[i], Node = this });
        }

        void InitializeReactiveFields()
        {
            fields = new Dictionary<string, FieldData>();

            var fs = this.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            for (int i = 0; i < fs.Length; i++)
            {
                if (IsTypeValid(fs[i]))
                    fields.Add(fs[i].Name, new FieldData { Field = new ReactiveField(this, fs[i]), Node = this, Inputs = new List<ParameterData>() });
            }
        }

        public static bool IsTypeValid(FieldInfo info)
        {
            if (info.GetCustomAttribute(typeof(Reactive)) != null && (info.FieldType.IsValueType || info.FieldType == typeof(string) || info.FieldType == typeof(object)))
                return true;
            return false;
        }

        public static bool CanBeAssigned(Type from, Type to)
        {
            return to.IsAssignableFrom(from);
        }

        public void SetFieldValue(string FieldName, object Value)
        {
            if (fields.ContainsKey(FieldName)) fields[FieldName].Field.Property = Value;
        }

        public FieldData GetField(string FieldName)
        {
            if (fields.ContainsKey(FieldName)) return fields[FieldName];
            return null;
        }

        public FieldData[] GetFields()
        {
            return fields != null ? fields.Values.ToArray() : null;
        }

        public ParameterData[] GetParameters()
        {
            return parameters != null ? parameters.Values.ToArray() : null;
        }

        public ParameterData GetParameter(string ParameterName)
        {
            if (parameters.ContainsKey(ParameterName)) return parameters[ParameterName];
            return null;
        }

        public ReactiveField GetReactiveField(string FieldName)
        {
            if (fields.ContainsKey(FieldName)) return fields[FieldName].Field;
            return null;
        }

        public bool CallExecution()
        {
            if (!active || !valid) return false;

            ExecuteInfo.Invoke(this, GetExecuteParametersValues());
            return true;
        }

        object[] GetExecuteParametersValues()
        {
            object[] objs = new object[parameters.Count];
            int index = 0;
            foreach (KeyValuePair<string, ParameterData> item in parameters)
            {
                if (item.Value.Input != null)
                    objs[index] = item.Value.Input.Field.Property;
                index++;
            }

            return objs;
        }

        public void UpdateReactiveFieldActive(ReactiveField field)
        {
            var list = GetFields();
            for (int i = 0; i < list.Length; i++)
            {
                ReactiveField f = list[i].Field;
                if (f == field)
                    CurrentFieldIndex = i;
                else f.Active = false;
            }
        }

        public ReactiveField GetActiveField()
        {
            var list = GetFields();
            if (list != null && list.Length > 0)
                return list[CurrentFieldIndex].Field;
            return null;
        }

        public static Type[] GetAllNodeTypes()
        {
            var q = from t in Assembly.GetExecutingAssembly().GetTypes()
                    where t.IsClass && t.IsSubclassOf(typeof(Node)) && t.GetCustomAttribute(typeof(BaseNodeClass)) == null
                    select t;

            return q.ToArray();
        }

        public static object CreateNodeFromTypeName(string type_name)
        {
            return Assembly.GetExecutingAssembly().CreateInstance(typeof(Node).Namespace + "." + type_name);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

public class Out : Attribute
{
}

public class BaseNodeClass : Attribute
{
}

public class Collection : Attribute
{
    public int lenght;
    public string[] names;

    public Collection(int lenght, string[] names = null)
    {
        this.lenght = Math.Max(1, lenght);
        this.names = names;
    }
}

public class FileField : Attribute
{
    public string filter;
    public bool is_folder;

    public FileField(string filter = null, bool is_folder = false)
    {
        this.filter = filter;
        this.is_folder = is_folder;
    }
}

namespace ICafe.Core
{
    public enum ExecutionOutput { OK = 0, NOTVALID = 1, LOOP = 2 }

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
                internal_fields.Add(new ReactiveField(Property, fs[i]));
            }
        }

        public ReactiveField GetInternalField(string field_name)
        {
            if (internal_fields == null) return null;

            for (int i = 0; i < internal_fields.Count; i++)
            {
                if (internal_fields[i].field.Name == field_name)
                    return internal_fields[i];
            }
            return null;
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
    }

    public interface ICollectionNotify
    {
        int GetCollectionIndex();
        bool IsParentACollection();
    }

    public class Node
    {
        public string NodeName;

        public MethodInfo ExecuteInfo { get; private set; }

        public Guid ID { get; private set; }

        Dictionary<string, FieldData> fields;
        Dictionary<string, ParameterData> parameters;
        bool valid, updated, waiting_execution;

        public class ParameterData : ICollectionNotify
        {
            public ParameterInfo Parameter;
            public FieldData[] Inputs;
            public Node Node;
            public int Lenght
            {
                get { return lenght; }
                set { lenght = Math.Max(0, value); Inputs = new FieldData[lenght]; }
            }
            public int Count { get; private set; }
            public bool IsCollection { get; private set; }

            Collection collection;
            int lenght;
            ICollectionNotify required;

            public ParameterData()
            {
                Lenght = 1;
            }

            public void SetColleciton(Collection collection)
            {
                if (collection == null) return;

                this.collection = collection;
                Lenght = collection.lenght;
                IsCollection = true;
            }

            public void SetCollectionNotify(ICollectionNotify parameter)
            {
                required = parameter;
            }

            public bool Assign(FieldData f, int index)
            {
                if (index >= 0 && index < lenght)
                {
                    Inputs[index] = f;
                    Count++;
                    return true;
                }
                return false;
            }

            public void RemoveAt(int index)
            {
                if (Inputs[index] != null) { Inputs[index] = null; Count--; }
            }

            public List<int> GetIndexesFromField(FieldData f)
            {
                List<int> indexes = new List<int>();
                for (int i = 0; i < Inputs.Length; i++)
                {
                    if (Inputs[i] == f)
                        indexes.Add(i);
                }
                return indexes;
            }

            public FieldData GetData()
            {
                if (Count == 0) return null;
                if (!IsCollection) return Inputs[0];
                if (required == null) return Inputs[0];

                int i = required.GetCollectionIndex();
                i = i < 0 || i >= Inputs.Length ? 0 : i;

                return Inputs[i];
            }

            public void BindParameterCollectionIndex(ParameterData field)
            {
                if (field.Lenght == 1)
                    required = field;
            }

            public string GetName(int index)
            {
                if (Lenght > 1)
                {
                    if (collection != null && collection.names != null)
                        if (index >= 0 && index < collection.names.Length)
                            return collection.names[index];
                    return Parameter.Name + "_" + index;
                }
                return Parameter.Name;
            }

            public int GetCollectionIndex()
            {
                int i = -1;

                var data = GetData();

                object obj = data != null ? data.Field.Property : Parameter.DefaultValue;
                if (obj == null) return -1;

                try
                {
                    i = Convert.ToInt32(obj);
                }
                catch (Exception)
                {
                    throw;
                }
                return i;
            }

            public bool IsParentACollection()
            {
                return IsCollection;
            }
        }

        public class FieldData : ICollectionNotify
        {
            public ReactiveField Field;
            public List<ParameterConnection> Inputs;
            public Node Node;
            public bool IsOut;

            public struct ParameterConnection
            {
                public ParameterData data;
                public int index;
            }

            public int GetCollectionIndex()
            {
                int i = -1;

                try
                {
                    i = Convert.ToInt32(Field.Property);
                }
                catch (Exception)
                {
                    throw;
                }
                return i;
            }

            public bool IsParentACollection()
            {
                return false;
            }

            public void Remove(ParameterData param, int index)
            {
                foreach (var item in Inputs)
                {
                    if (item.data == param && item.index == index)
                    {
                        Inputs.Remove(item);
                        return;
                    }
                }
            }

        }

        public virtual void Start()
        { }

        public virtual void Stop()
        { }

        public virtual void Init()
        { }

        public Guid Initialize(Guid guid)
        {
            ID = guid;

            NodeName = GetTypeName();

            InitializeFields();
            InitMethods();
            InitializeParameters();

            Init();

            return ID;
        }

        public Guid Initialize()
        {
            return Initialize(Guid.NewGuid());
        }

        void InitMethods()
        {
            SetExecute();
        }

        void SetExecute()
        {
            var methods = this.GetType().GetMethods();
            foreach (var item in methods)
            {
                if (item.Name == "Execute")
                {
                    ExecuteInfo = item;
                    valid = true;
                    return;
                }
            }
            valid = false;
        }

        void InitializeParameters()
        {
            if (!valid) return;

            var param = ExecuteInfo.GetParameters();
            parameters = new Dictionary<string, ParameterData>();

            for (int i = 0; i < param.Length; i++)
            {
                ParameterData pd = new ParameterData { Parameter = param[i], Node = this };

                Collection a = param[i].GetCustomAttribute(typeof(Collection)) as Collection;
                pd.SetColleciton(a);

                parameters.Add(param[i].Name, pd);
            }
        }

        void InitializeFields()
        {
            fields = new Dictionary<string, FieldData>();

            var f_name = this.GetType().GetField("NodeName");
            fields.Add(f_name.Name, new FieldData { Field = new ReactiveField(this, f_name), Node = this });

            var fs = this.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            for (int i = 0; i < fs.Length; i++)
            {
                if (fs[i] == f_name) continue;

                bool is_out = fs[i].GetCustomAttribute<Out>() != null;
                fields.Add(fs[i].Name, new FieldData { Field = new ReactiveField(this, fs[i]), Node = this, Inputs = new List<FieldData.ParameterConnection>(), IsOut = is_out });
            }
        }

        public static bool IsTypeValid(Type type)
        {
            if (type.IsValueType || type == typeof(string) || type == typeof(object))
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

        public FieldData GetNameField()
        {
            return fields["NodeName"];
        }

        public void SetNameField(string new_name)
        {
            fields["NodeName"].Field.Property = new_name;
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

        public void ResetExecution()
        {
            waiting_execution = false;
            updated = false;
        }

        public ExecutionOutput CallFullExecution(bool force_nodes_execution)
        {
            if (waiting_execution)
                return ExecutionOutput.LOOP;

            waiting_execution = true;

            var output = CallExecution(force_nodes_execution);
            if (output != ExecutionOutput.OK)
                return output;

            updated = true;
            waiting_execution = false;

            return ExecutionOutput.OK;
        }

        ExecutionOutput CallExecution(bool force_nodes_execution)
        {
            if (!valid) return ExecutionOutput.NOTVALID;

            var output = ExecutionOutput.OK;
            var objs = GetExecuteParametersValues(force_nodes_execution, ref output);
            if (output != ExecutionOutput.OK) return output;

            ExecuteInfo.Invoke(this, objs);
            return ExecutionOutput.OK;
        }

        object[] GetExecuteParametersValues(bool force_update, ref ExecutionOutput output)
        {
            object[] objs = new object[parameters.Count];
            int index = 0;
            foreach (KeyValuePair<string, ParameterData> item in parameters)
            {
                FieldData f = item.Value.GetData();
                if (f != null)
                {
                    if (f.Node.updated == false)
                        if ((output = f.Node.CallFullExecution(force_update)) != ExecutionOutput.OK)
                            return null;
                    objs[index] = f.Field.Property;
                }
                index++;
            }

            return objs;
        }

        public static Type[] GetAllNodeTypes()
        {
            var q = from t in Assembly.GetExecutingAssembly().GetTypes()
                    where t.IsClass && t.IsSubclassOf(typeof(Node)) && t.GetCustomAttribute(typeof(BaseNodeClass)) == null
                    select t;

            return q.ToArray();
        }

        public static object CreateNodeFromTypeName(string type_name, bool as_full_path = false)
        {
            return Assembly.GetExecutingAssembly().CreateInstance(!as_full_path ? typeof(Node).Namespace + "." + type_name : type_name);
        }

        public static object CreateNodeFromUserTypeName(string type_name)
        {
            Type type = Registry.GetNodeTypeFromName(type_name);
            return Assembly.GetExecutingAssembly().CreateInstance(type.FullName);
        }

        public virtual string GetTypeName()
        {
            return this.GetType().Name;
        }

        public void BindCollectionParameter(ParameterData collection, ICollectionNotify parameter)
        {
            if (collection != null && parameter != null && collection.IsCollection)
            {
                if (!parameter.IsParentACollection())
                {
                    collection.SetCollectionNotify(parameter);
                }
            }
        }
    }
}
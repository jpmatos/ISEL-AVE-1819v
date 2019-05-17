using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Csvier
{
    public abstract class AbstractParser<T> : IParser<T>
    {
        public abstract T[] Parse();
        protected abstract PropertyInfo FindProperty(string arg);
        protected abstract FieldInfo FindField(string pairArg);
        
        protected struct Pair
        {
            public readonly string arg;
            public readonly int col;

            public Pair(string arg, int col)
            {
                this.arg = arg;
                this.col = col;
            }
        }

        protected static readonly Dictionary<Type, MethodBase> parsers = new Dictionary<Type, MethodBase>();
        protected readonly char separator;
        protected readonly List<Pair> ctorPairs = new List<Pair>();
        protected readonly List<Pair> propPairs = new List<Pair>();
        protected readonly List<Pair> fieldPairs = new List<Pair>();
        public string[] arr { get; protected set; }

        protected AbstractParser(char separator)
        {
            this.separator = separator;
        }
        

        public AbstractParser<T> CtorArg(string arg, int col)
        {
            ctorPairs.Add(new Pair(arg, col));
            return this;
        }

        public AbstractParser<T> PropArg(string arg, int col)
        {
            propPairs.Add(new Pair(arg, col));
            return this;
        }

        public AbstractParser<T> FieldArg(string arg, int col)
        {
            fieldPairs.Add(new Pair(arg, col));
            return this;
        }

        public AbstractParser<T> Load(string src)
        {
            arr = src.Split(new[] {"\r\n", "\r", "\n"}, StringSplitOptions.None);
            return this;
        }

        public AbstractParser<T> Remove(int count)
        {
            arr = arr.Skip(count).ToArray();
            return this;
        }

        public AbstractParser<T> RemoveEmpties()
        {
            arr =  arr.Where(str => !string.IsNullOrEmpty(str)).ToArray();
            return this;
        }

        public AbstractParser<T> RemoveWith(string word)
        {
            arr = arr.Where(str => !str.StartsWith(word)).ToArray();
            return this;
        }

        public AbstractParser<T> RemoveEvenIndexes()
        {
            arr = arr.Where((str, index) => index % 2 != 0).ToArray();
            return this;
        }

        public AbstractParser<T> RemoveOddIndexes()
        {
            arr = arr.Where((str, index) => index % 2 == 0).ToArray();
            return this;
        }

        protected T[] ConstructArray(ConstructorInfo con)
        {
            ParameterInfo[] parameters = con.GetParameters();
            T[] res = new T[arr.Length];
            object[] args = new object[ctorPairs.Count];
            for (int i = 0; i < res.Length; i++)
            {
                string[] values = arr[i].Split(separator);
                for (int j = 0; j < args.Length; j++)
                {
                    int index = ctorPairs[j].col;
                    string value = values[index];
                    Type paramType = parameters[j].ParameterType;
                    MethodBase parser = FindParser(paramType);
                    var parsedValue = parser != null ? parser.Invoke(null, new object[] {value}) : value;
                    args[j] = parsedValue;
                }

                res[i] = (T) con.Invoke(args);
            }

            return res;
        }

        protected void SetProperties(T[] res)
        {
            foreach (Pair pair in propPairs)
            {
                PropertyInfo propertyInfo = FindProperty(pair.arg);
                if (propertyInfo == null) continue;
                int i = 0;
                foreach (object obj in res)
                {
                    string[] values = arr[i++].Split(separator);
                    int index = pair.col;
                    string value = values[index];
                    MethodBase parser = FindParser(propertyInfo.PropertyType);
                    var parsedValue = parser != null ? parser.Invoke(null, new object[] {value}) : value;
                    propertyInfo.SetValue(obj, parsedValue);
                }
            }
        }

        protected void SetFields(T[] res)
        {
            foreach (Pair pair in fieldPairs)
            {
                FieldInfo fieldInfo = FindField(pair.arg);
                if (fieldInfo == null) continue;
                int i = 0;
                foreach (object obj in res)
                {
                    string[] values = arr[i++].Split(separator);
                    int index = pair.col;
                    string value = values[index];
                    MethodBase parser = FindParser(fieldInfo.FieldType);
                    var parsedValue = parser != null ? parser.Invoke(null, new object[] {value}) : value;
                    fieldInfo.SetValue(obj, parsedValue);
                }
            }
        }

        protected static MethodBase FindParser(Type paramType)
        {
            if (parsers.TryGetValue(paramType, out var parser)) return parser;

            parser = paramType.GetMethod("Parse", new Type[] {typeof(string)});
            if (parser == null) return null;
            parsers.Add(paramType, parser);

            return parser;
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Csvier
{
    public class CsvParser
    {
        private struct Pair
        {
            public readonly string arg;
            public readonly int col;

            public Pair(string arg, int col)
            {
                this.arg = arg;
                this.col = col;
            }
        }

        private readonly Type klass;
        private readonly char separator;
        private string[] arr;
        private readonly List<Pair> ctorPairs;
        private readonly List<Pair> propPairs;
        private readonly List<Pair> fieldPairs;
        private static readonly Dictionary<Type, MethodBase> parsers = new Dictionary<Type, MethodBase>();

        public string[] getArr()
        {
            return arr;
        }

        public CsvParser(Type klass, char separator = ',')
        {
            this.klass = klass;
            this.separator = separator;
            this.ctorPairs = new List<Pair>();
            this.propPairs = new List<Pair>();
            this.fieldPairs = new List<Pair>();
        }

        public CsvParser CtorArg(string arg, int col)
        {
            ctorPairs.Add(new Pair(arg, col));
            return this;
        }

        public CsvParser PropArg(string arg, int col)
        {
            propPairs.Add(new Pair(arg, col));
            return this;
        }

        public CsvParser FieldArg(string arg, int col)
        {
            fieldPairs.Add(new Pair(arg, col));
            return this;
        }

        public CsvParser Load(string src)
        {
            arr = src.Split(new[] {"\r\n", "\r", "\n"}, StringSplitOptions.None);
            return this;
        }

        public CsvParser Remove(int count)
        {
            arr = arr.Skip(count).ToArray();
            return this;
        }

        public CsvParser RemoveEmpties()
        {
            arr =  arr.Where(str => !string.IsNullOrEmpty(str)).ToArray();
            return this;
        }

        public CsvParser RemoveWith(string word)
        {
            arr = arr.Where(str => !str.StartsWith(word)).ToArray();
            return this;
        }

        public CsvParser RemoveEvenIndexes()
        {
            arr = arr.Where((str, index) => index % 2 != 0).ToArray();
            return this;
        }

        public CsvParser RemoveOddIndexes()
        {
            arr = arr.Where((str, index) => index % 2 == 0).ToArray();
            return this;
        }

        public object[] Parse()
        {
            ConstructorInfo con = FindConstructor();
            object[] res = ConstructArray(con);
            SetProperties(res);
            SetFields(res);
            return res;
        }

        private void SetFields(object[] res)
        {
            foreach (Pair pair in fieldPairs)
            {
                FieldInfo fieldInfo = klass.GetField(pair.arg);
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

        private void SetProperties(object[] res)
        {
            foreach (Pair pair in propPairs)
            {
                PropertyInfo propertyInfo = klass.GetProperty(pair.arg);
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

        private object[] ConstructArray(ConstructorInfo con)
        {
            ParameterInfo[] parameters = con.GetParameters();
            object[] res = new object[arr.Length];
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

                res[i] = con.Invoke(args);
            }

            return res;
        }

        private ConstructorInfo FindConstructor()
        {
            ConstructorInfo[] cons = klass.GetConstructors();
            foreach (ConstructorInfo con in cons)
            {
                ParameterInfo[] parameters = con.GetParameters();
                if (parameters.Length != ctorPairs.Count)
                    continue;
                using (List<Pair>.Enumerator enumerator = ctorPairs.GetEnumerator())
                {
                    enumerator.MoveNext();
                    foreach (ParameterInfo parameter in parameters)
                    {
                        if (parameter.Name != enumerator.Current.arg)
                            break;
                        if (!enumerator.MoveNext())
                            return con;
                    }
                }
            }

            return null;
        }

        private static MethodBase FindParser(Type paramType)
        {
            if (parsers.TryGetValue(paramType, out var parser)) return parser;

            parser = paramType.GetMethod("Parse", new Type[] {typeof(string)});
            if (parser == null) return null;
            parsers.Add(paramType, parser);

            return parser;
        }
    }
}
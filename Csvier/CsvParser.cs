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
        private readonly List<Pair> ctorParams;
        private static readonly Dictionary<Type, MethodBase> parsers = new Dictionary<Type, MethodBase>();

        public CsvParser(Type klass, char separator = ',')
        {
            this.klass = klass;
            this.separator = separator;
            this.ctorParams = new List<Pair>();
        }

        public CsvParser CtorArg(string arg, int col)
        {
            ctorParams.Add(new Pair(arg, col));
            return this;
        }

        public CsvParser PropArg(string arg, int col)
        {
            return this;
        }

        public CsvParser FieldArg(string arg, int col)
        {
            return this;
        }

        public CsvParser Load(String src)
        {
            arr = src.Split(new[] {"\r\n", "\r", "\n"}, StringSplitOptions.None);
            return this;
        }

        public CsvParser Remove(int count)
        {
            return this;
        }

        public CsvParser RemoveEmpties()
        {
            return this;
        }

        public CsvParser RemoveWith(string word)
        {
            return this;
        }

        public CsvParser RemoveEvenIndexes()
        {
            return this;
        }

        public CsvParser RemoveOddIndexes()
        {
            return this;
        }

        public IEnumerable<object> Parse()
        {
            ConstructorInfo con = FindConstructor(); // helper method
            IEnumerable<object> res = ParseArray(con); // helper method
            return res;
        }

        private ConstructorInfo FindConstructor()
        {
            ConstructorInfo[] cons = klass.GetConstructors();
            foreach (ConstructorInfo con in cons)
            {
                ParameterInfo[] parameters = con.GetParameters();
                if (parameters.Length != ctorParams.Count)
                    continue;
                using (List<Pair>.Enumerator enumerator = ctorParams.GetEnumerator())
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

        private IEnumerable<object> ParseArray(ConstructorInfo con)
        {
            ParameterInfo[] parameters = con.GetParameters();
            object[] res = new object[arr.Length];
            object[] args = new object[ctorParams.Count];
            for (int i = 0; i < res.Length; i++)
            {
                string[] values = arr[i].Split(separator);
                for (int j = 0; j < args.Length; j++)
                {
                    Type paramType = parameters[j].ParameterType;
                    if (!parsers.TryGetValue(paramType, out var parser))
                    {
                        parser = paramType.GetMethod("Parse", new Type[] {typeof(string)});
                        parsers.Add(paramType, parser);
                    }

                    if (parser == null) throw new MissingMethodException();
                    int index = ctorParams[j].col;
                    string value = values[index];
                    args[j] = parser.Invoke(null, new object[] {value});
                }

                res[i] = con.Invoke(args);
            }

            return res;
        }
    }
}
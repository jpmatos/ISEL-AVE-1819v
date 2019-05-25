using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Csvier.Enumerables;
// ReSharper disable All

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
        public char separator { get; }
        protected readonly List<Pair> ctorPairs = new List<Pair>();
        protected readonly List<Pair> propPairs = new List<Pair>();

        protected readonly List<Pair> fieldPairs = new List<Pair>();

        //public string[] arr { get; protected set; }
        public RowEnumerable src { get; protected set; }

        protected AbstractParser(char separator)
        {
            this.separator = separator;
        }

        public T[] Parse(Func<string, T> parser)
        {
            T[] res = new T[src.Count()];

            int i = 0;
            foreach (string str in src)
            {
                res[i++] = parser.Invoke(str);
            }

            return res;
        }

        public IEnumerable<T> ToEnumerable(Func<string, T> parser)
        {
            foreach (string str in src)
            {
                yield return parser.Invoke(str);
            }
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

        public AbstractParser<T> Load(string csvSrc)
        {
            src = new RowEnumerable(csvSrc);
            return this;
        }

        public AbstractParser<T> Remove(int count)
        {
            src.Remove(count);
            return this;
        }

        public AbstractParser<T> RemoveEmpties()
        {
            src.RemoveEmpties();
            return this;
        }

        public AbstractParser<T> RemoveWith(string word)
        {
            src.RemoveWith(word);
            return this;
        }

        public AbstractParser<T> RemoveEvenIndexes()
        {
            src.RemoveEvens();
            return this;
        }

        public AbstractParser<T> RemoveOddIndexes()
        {
            src.RemoveOdds();
            return this;
        }

        protected T[] ConstructArray(ConstructorInfo con)
        {
            ParameterInfo[] parameters = con.GetParameters();
            T[] res = new T[src.Count()];
            object[] args = new object[ctorPairs.Count];
            int i = 0;
            foreach (string str in src)
            {
                string[] values = str.Split(separator);
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
                i++;
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
                IEnumerator<string> enm = src.GetEnumerator();
                foreach (object obj in res)
                {
                    enm.MoveNext();
                    WordEnumerable values = new WordEnumerable(enm.Current, separator);
                    int index = pair.col;
                    string value = values.GetWord(index);
                    MethodBase parser = FindParser(propertyInfo.PropertyType);
                    object parsedValue = parser != null ? parser.Invoke(null, new object[] {value}) : value;
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
                IEnumerator<string> enm = src.GetEnumerator();
                foreach (object obj in res)
                {
                    WordEnumerable values = new WordEnumerable(enm.Current, separator);
                    int index = pair.col;
                    string value = values.GetWord(index);
                    MethodBase parser = FindParser(fieldInfo.FieldType);
                    object parsedValue = parser != null ? parser.Invoke(null, new object[] {value}) : value;
                    fieldInfo.SetValue(obj, parsedValue);
                }
            }
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
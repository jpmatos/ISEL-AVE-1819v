using System;
using System.Collections.Generic;
using System.Reflection;

namespace Csvier
{
    public class CsvParser<T> : AbstractParser<T>
    {
        public CsvParser(char separator = ',') : base(separator)
        {
        }

        public override T[] Parse()
        {
            ConstructorInfo con = FindConstructor();
            T[] res = ConstructArray(con);
            SetProperties(res);
            SetFields(res);
            return res;
        }

        protected override FieldInfo FindField(string arg)
        {
            return typeof(T).GetField(arg);
        }

        protected override PropertyInfo FindProperty(string arg)
        {
            return typeof(T).GetProperty(arg);
        }

        private ConstructorInfo FindConstructor()
        {
            ConstructorInfo[] cons = typeof(T).GetConstructors();
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
            throw new MissingMethodException();
        }
    }
}
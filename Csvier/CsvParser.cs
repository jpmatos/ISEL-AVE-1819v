using System;
using System.Collections.Generic;
using System.Reflection;

namespace Csvier
{
    public class CsvParser : AbstractParser
    {
        public CsvParser(Type klass, char separator = ',') : base(klass, separator)
        {
        }

        public override object[] Parse()
        {
            ConstructorInfo con = FindConstructor();
            object[] res = ConstructArray(con);
            SetProperties(res);
            SetFields(res);
            return res;
        }

        protected override FieldInfo FindField(string arg)
        {
            return klass.GetField(arg);
        }

        protected override PropertyInfo FindProperty(string arg)
        {
            return klass.GetProperty(arg);
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
            throw new MissingMethodException();
        }
    }
}
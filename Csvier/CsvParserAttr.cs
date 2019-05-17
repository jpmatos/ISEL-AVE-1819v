using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Csvier
{
    public class CsvParserAttr<T> : AbstractParser<T>
    {
        private readonly string constructorName;
        private Dictionary<string, PropertyInfo> propertyDict;
        private Dictionary<string, FieldInfo> fieldDict;
        private bool filledDictionaries;
        
        public CsvParserAttr(string constructorName, char separator = ',') : base(separator)
        {
            this.constructorName = constructorName;
        }

        private void FillPropertyDict()
        {
            foreach (PropertyInfo propertyInfo in typeof(T).GetProperties())
            {
                foreach (Attribute attribute in Attribute.GetCustomAttributes(propertyInfo))
                {
                    if (attribute.GetType() != typeof(DescriptionAttribute)) continue;
                    DescriptionAttribute attr = (DescriptionAttribute) attribute;
                    propertyDict.Add(attr.Description, propertyInfo);
                }
            }
        }

        private void FillFieldDict()
        {
            foreach (FieldInfo fieldInfo in typeof(T).GetFields())
            {
                foreach (Attribute attribute in Attribute.GetCustomAttributes(fieldInfo))
                {
                    if (attribute.GetType() != typeof(DescriptionAttribute)) continue;
                    DescriptionAttribute attr = (DescriptionAttribute) attribute;
                    fieldDict.Add(attr.Description, fieldInfo);
                }
            }
        }

        public override T[] Parse()
        {
            if (!filledDictionaries)
            {
                propertyDict = new Dictionary<string, PropertyInfo>();
                fieldDict = new Dictionary<string, FieldInfo>();
                FillPropertyDict();
                FillFieldDict();
                filledDictionaries = true;
            }
            ConstructorInfo con = FindConstructor();
            T[] res = ConstructArray(con);
            SetProperties(res);
            SetFields(res);
            return res;
        }

        protected override PropertyInfo FindProperty(string arg)
        {
            if (!propertyDict.TryGetValue(arg, out PropertyInfo res)) throw new MissingFieldException();
            return res;
        }

        protected override FieldInfo FindField(string arg)
        {
            if (!fieldDict.TryGetValue(arg, out FieldInfo res)) throw new MissingFieldException();
            return res;
        }

        private ConstructorInfo FindConstructor()
        {
            foreach (ConstructorInfo constructor in typeof(T).GetConstructors())
            {
                foreach (Attribute attribute in Attribute.GetCustomAttributes(constructor))
                {
                    if (attribute.GetType() != typeof(DescriptionAttribute)) continue;
                    DescriptionAttribute attr = (DescriptionAttribute) attribute;
                    if (attr.Description == constructorName) return constructor;
                }
            }
            throw new MissingMethodException();
        }
    }
}
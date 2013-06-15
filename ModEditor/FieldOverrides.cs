using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModEditor
{
    public class FieldOverrides
    {
        protected Dictionary<string, List<Attribute>> fieldAttributes = new Dictionary<string, List<Attribute>>();
        protected System.Type targetType;

        public FieldOverrides(System.Type type)
        {
            targetType = type;
        }

        public bool HasOverrides()
        {
            return fieldAttributes.Count > 0;
        }

        // Add custom field attribute to modify ItemExplorer sheet generation
        public void AddAttribute(string fieldName, Attribute attrib)
        {
            if (!fieldAttributes.ContainsKey(fieldName))
                fieldAttributes.Add(fieldName, new List<Attribute>());
            fieldAttributes[fieldName].Add(attrib);
        }

        // Mark field as "string token"            
        public void OverrideFieldLocString(string fieldName)
        {
            AddAttribute(fieldName, new LocStringToken());
        }

        // Mark field as "object reference"            
        public void OverrideFieldObjectReference(string fieldName, string group, bool required)
        {
            AddAttribute(fieldName, new ObjectReference(group, !required));
        }

        // Ignore field
        public void IgnoreField(string fieldName)
        {
            AddAttribute(fieldName, new IgnoreByEditor());
        }

        // Obtain all the attributes within spedified field
        public List<Attribute> GetFieldAttribute(System.Reflection.FieldInfo fieldInfo)
        {
            List<Attribute> result = new List<Attribute>();

            // Get attributes from local override list
            if (fieldAttributes.ContainsKey(fieldInfo.Name))
            {
                var list = fieldAttributes[fieldInfo.Name];
                result.AddRange(list);
            }

            // Get attributes from class definition                
            foreach (Attribute attribute in fieldInfo.GetCustomAttributes(true))
            {
                result.Add(attribute);
            }

            return result;
        }
    }
}

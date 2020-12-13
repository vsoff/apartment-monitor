using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DifferencesSearch.Trees
{
    public class PropertiesTreeNode
    {
        public Type PropertyType { get; }
        public string PropertyName { get; }
        public PropertyInfo Property { get; }
        public PropertyInfo[] InnerProperties { get; set; }
        public PropertiesTreeNode[] Nodes { get; set; }

        public PropertiesTreeNode(Type propertyType)
        {
            PropertyType = propertyType;
        }

        public PropertiesTreeNode(PropertyInfo property)
        {
            Property = property;
            PropertyName = property.Name;
            PropertyType = property.PropertyType;
        }
    }
}

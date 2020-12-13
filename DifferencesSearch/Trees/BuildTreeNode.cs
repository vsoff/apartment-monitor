using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DifferencesSearch.Trees
{
    internal class BuildTreeNodeConfig
    {
    }

    internal class BuildTreeNode<TBuildTreeNodeConfig> where TBuildTreeNodeConfig : BuildTreeNodeConfig, new()
    {
        public Type PropertyType { get; }
        public string PropertyName { get; }
        public PropertyInfo Property { get; }
        public TBuildTreeNodeConfig Config { get; }
        public Dictionary<string, BuildTreeNode<TBuildTreeNodeConfig>> ChildNodes { get; internal set; }

        public BuildTreeNode(PropertyInfo property) : this(property.PropertyType)
        {
            Property = property;
            PropertyName = property.Name;
        }

        public BuildTreeNode(Type propertyType)
        {
            PropertyName = null;
            PropertyType = propertyType;
            Config = new TBuildTreeNodeConfig();
            ChildNodes = new Dictionary<string, BuildTreeNode<TBuildTreeNodeConfig>>();
        }
    }
}

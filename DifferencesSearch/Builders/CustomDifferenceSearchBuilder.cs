using DifferencesSearch.Extensions;
using DifferencesSearch.Trees;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace DifferencesSearch.Builders
{
    public interface ICustomDifferenceSearchBuilder<TSource> : IDifferenceSearchBuilder<TSource>
    {
        ICustomDifferenceSearchBuilder<TSource> One<TProp>(Expression<Func<TSource, TProp>> expression);
        ICustomDifferenceSearchBuilder<TSource> All<TProp>(Expression<Func<TSource, TProp>> expression) where TProp : class;
        ICustomDifferenceSearchBuilder<TSource> GoDepth<TProp>(Expression<Func<TSource, TProp>> expression) where TProp : class;
    }

    public class CustomDifferenceSearchBuilder<TSource> : ICustomDifferenceSearchBuilder<TSource>
    {
        private BuildTreeNode<CustomBuildTreeNodeConfig> _buildRootNode;

        public PropertiesTreeNode PropertyTree { get; private set; }

        public CustomDifferenceSearchBuilder()
        {
            PropertyTree = null;
            _buildRootNode = new BuildTreeNode<CustomBuildTreeNodeConfig>(typeof(TSource));
        }

        public void Build()
        {
            PropertiesTreeNode root = new PropertiesTreeNode(typeof(TSource));

            DepthBuild(_buildRootNode, root);

            PropertyTree = root;
        }

        private void DepthBuild(BuildTreeNode<CustomBuildTreeNodeConfig> buildNode, PropertiesTreeNode newNode)
        {
            // Создаём другие node дерева.
            List<PropertiesTreeNode> newNodes = new List<PropertiesTreeNode>();
            foreach (var nodePair in buildNode.ChildNodes)
            {
                //string propName = nodePair.Key;
                //if (buildNode.Config.IgnoredProperties.Contains(propName))
                //    continue;

                var newInnerNode = new PropertiesTreeNode(nodePair.Value.Property);

                // Если это поле 
                if (buildNode.Config.GoDepthProperties.Contains(newInnerNode.PropertyName))
                {
                    DifferenceSearchBuilderExtensions.AutoDepthBuild(newInnerNode.PropertyType, newInnerNode);
                }
                else
                {
                    DepthBuild(nodePair.Value, newInnerNode);
                }

                newNodes.Add(newInnerNode);
            }

            newNode.InnerProperties = buildNode.Config.NeedAllProperties
                ? buildNode.PropertyType.GetProperties()
                : buildNode.Config.SelectedProperties.ToArray();
            newNode.Nodes = newNodes.ToArray();
        }

        public ICustomDifferenceSearchBuilder<TSource> One<TProp>(Expression<Func<TSource, TProp>> expression)
        {
            _buildRootNode.AddExpressionToTree(expression, (node, property) => node.Config.SelectedProperties.Add(property));
            return this;
        }

        public ICustomDifferenceSearchBuilder<TSource> All<TProp>(Expression<Func<TSource, TProp>> expression) where TProp : class
        {
            _buildRootNode.AddExpressionToTree(expression, (node, property) => node.Config.NeedAllProperties = true);
            return this;
        }

        public ICustomDifferenceSearchBuilder<TSource> GoDepth<TProp>(Expression<Func<TSource, TProp>> expression) where TProp : class
        {
            _buildRootNode.AddExpressionToTree(expression, (node, property) => node.Config.GoDepthProperties.Add(property.Name), true);
            return this;
        }

        internal class CustomBuildTreeNodeConfig : BuildTreeNodeConfig
        {
            public HashSet<PropertyInfo> SelectedProperties { get; }
            public HashSet<string> GoDepthProperties { get; }
            public bool NeedAllProperties { get; set; }

            public CustomBuildTreeNodeConfig()
            {
                NeedAllProperties = false;
                GoDepthProperties = new HashSet<string>();
                SelectedProperties = new HashSet<PropertyInfo>();
            }
        }
    }
}

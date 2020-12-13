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
    public interface IAutoDifferenceSearchBuilder<TSource> : IDifferenceSearchBuilder<TSource>
    {
        IAutoDifferenceSearchBuilder<TSource> Stop<TProp>(Expression<Func<TSource, TProp>> expression) where TProp : class;
        IAutoDifferenceSearchBuilder<TSource> Ignore<TProp>(Expression<Func<TSource, TProp>> expression);
    }

    public class AutoDifferenceSearchBuilder<TSource> : IAutoDifferenceSearchBuilder<TSource>
    {
        private BuildTreeNode<AutoBuildTreeNodeConfig> _buildRootNode;

        public PropertiesTreeNode PropertyTree { get; private set; }

        public AutoDifferenceSearchBuilder()
        {
            PropertyTree = null;
            _buildRootNode = new BuildTreeNode<AutoBuildTreeNodeConfig>(typeof(TSource));
        }

        public void Build()
        {
            PropertiesTreeNode root = new PropertiesTreeNode(typeof(TSource));

            DepthBuild(_buildRootNode, root);

            PropertyTree = root;
        }

        private void DepthBuild(BuildTreeNode<AutoBuildTreeNodeConfig> buildNode, PropertiesTreeNode newNode)
        {
            PropertyInfo[] allTypeProperties = buildNode.PropertyType.GetProperties();

            // Исключаем все игнорируемые поля.
            PropertyInfo[] neededProperties = allTypeProperties
                .Where(info => !buildNode.Config.IgnoredProperties.Contains(info.Name)).ToArray();

            // Создаём другие node дерева.
            List<PropertiesTreeNode> newNodes = new List<PropertiesTreeNode>();
            foreach (var info in neededProperties)
            {
                // Если в build версии дерева есть node, тогда проходим по его правилам.
                if (buildNode.ChildNodes.ContainsKey(info.Name))
                {
                    if (buildNode.ChildNodes[info.Name].Config.IsDeadEnd)
                        continue;

                    var newInnerNode = new PropertiesTreeNode(info);
                    DepthBuild(buildNode.ChildNodes[info.Name], newInnerNode);
                    newNodes.Add(newInnerNode);
                }
                // Если в build версии дерева нету node, тогда запускаем полностью рефлексивный обход.
                else if (!info.PropertyType.IsSimple())
                {
                    var newInnerNode = new PropertiesTreeNode(info);
                    DifferenceSearchBuilderExtensions.AutoDepthBuild(info.PropertyType, newInnerNode);
                    newNodes.Add(newInnerNode);
                }
            }

            newNode.InnerProperties = neededProperties;
            newNode.Nodes = newNodes.ToArray();
        }

        public IAutoDifferenceSearchBuilder<TSource> Ignore<TProp>(Expression<Func<TSource, TProp>> expression)
        {
            _buildRootNode.AddExpressionToTree(expression, (node, property) => node.Config.IgnoredProperties.Add(property.Name), true);
            return this;
        }

        public IAutoDifferenceSearchBuilder<TSource> Stop<TProp>(Expression<Func<TSource, TProp>> expression) where TProp : class
        {
            _buildRootNode.AddExpressionToTree(expression, (node, property) => node.Config.IsDeadEnd = true);
            return this;
        }

        internal class AutoBuildTreeNodeConfig : BuildTreeNodeConfig
        {
            public HashSet<string> IgnoredProperties { get; }
            public bool IsDeadEnd { get; set; }

            public AutoBuildTreeNodeConfig()
            {
                IsDeadEnd = false;
                IgnoredProperties = new HashSet<string>();
            }
        }
    }
}

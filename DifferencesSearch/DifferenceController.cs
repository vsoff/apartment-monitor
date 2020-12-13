using DifferencesSearch.Builders;
using DifferencesSearch.Trees;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace DifferencesSearch
{
    public interface IDifferenceController
    {
        IAutoDifferenceSearchBuilder<T> AutoBuilder<T>();
        ICustomDifferenceSearchBuilder<T> CustomBuilder<T>();
        PropertyDifference[] GetAutoDifferences<T>(T firstObj, T secondObj);
        PropertyDifference[] GetCustomDifferences<T>(T firstObj, T secondObj);
    }

    public class DifferenceController : IDifferenceController
    {
        /// <summary>
        /// Содежит билдеры автоматического сравнения.
        /// </summary>
        private readonly Dictionary<Type, IDifferenceSearchBuilder> _autoBuildersMap;

        /// <summary>
        /// Содежит билдеры ручного сравнения.
        /// </summary>
        private readonly Dictionary<Type, IDifferenceSearchBuilder> _customBuildersMap;

        public DifferenceController()
        {
            _autoBuildersMap = new Dictionary<Type, IDifferenceSearchBuilder>();
            _customBuildersMap = new Dictionary<Type, IDifferenceSearchBuilder>();
        }

        public IAutoDifferenceSearchBuilder<T> AutoBuilder<T>()
        {
            Type type = typeof(T);

            if (!_autoBuildersMap.ContainsKey(type))
                _autoBuildersMap[type] = new AutoDifferenceSearchBuilder<T>();

            return _autoBuildersMap[type] as IAutoDifferenceSearchBuilder<T>;
        }

        public ICustomDifferenceSearchBuilder<T> CustomBuilder<T>()
        {
            Type type = typeof(T);

            if (!_customBuildersMap.ContainsKey(type))
                _customBuildersMap[type] = new CustomDifferenceSearchBuilder<T>();

            return _customBuildersMap[type] as ICustomDifferenceSearchBuilder<T>;
        }

        public PropertyDifference[] GetAutoDifferences<T>(T firstObj, T secondObj)
        {
            IAutoDifferenceSearchBuilder<T> searchBuilder = AutoBuilder<T>();

            if (searchBuilder.PropertyTree == null)
                searchBuilder.Build();

            PropertiesTreeNode tree = searchBuilder.PropertyTree;

            return GetDifferences(tree, firstObj, secondObj);
        }

        public PropertyDifference[] GetCustomDifferences<T>(T firstObj, T secondObj)
        {
            Type type = typeof(T);

            if (!_customBuildersMap.ContainsKey(type))
                throw new KeyNotFoundException($"Для типа {type} не зарегистрирован builder");

            PropertiesTreeNode tree = _customBuildersMap[type].PropertyTree;

            return GetDifferences(tree, firstObj, secondObj);
        }

        private PropertyDifference[] GetDifferences(PropertiesTreeNode node, object firstObj, object secondObj)
        {
            List<PropertyDifference> differences = new List<PropertyDifference>();

            foreach (PropertyInfo info in node.InnerProperties)
            {
                var left = info.GetValue(firstObj);
                var right = info.GetValue(secondObj);

                if (left == right)
                    continue;

                if (left == null || right == null || !left.Equals(right))
                    differences.Add(new PropertyDifference
                    {
                        ClassType = node.PropertyType,
                        PropertyType = info.PropertyType,
                        PropertyName = info.Name,
                        ValueLeft = left,
                        ValueRight = right
                    });
            }

            foreach (var childNode in node.Nodes)
            {
                var left = childNode.Property.GetValue(firstObj);
                var right = childNode.Property.GetValue(secondObj);

                if (left == right)
                    continue;

                if (left == null || right == null)
                {
                    differences.Add(new PropertyDifference
                    {
                        ClassType = node.PropertyType,
                        PropertyType = childNode.PropertyType,
                        PropertyName = childNode.PropertyName,
                        ValueLeft = left,
                        ValueRight = right
                    });
                    continue;
                }

                var newDiffs = GetDifferences(childNode, left, right);
                differences.AddRange(newDiffs);
            }

            return differences.ToArray();
        }
    }
}

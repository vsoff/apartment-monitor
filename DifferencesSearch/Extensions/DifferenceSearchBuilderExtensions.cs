using DifferencesSearch.Builders;
using DifferencesSearch.Trees;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace DifferencesSearch.Extensions
{
    internal static class DifferenceSearchBuilderExtensions
    {
        /// <summary>
        /// Обрабатывает выражение и возвращает callback.
        /// </summary>
        /// <typeparam name="TSource">Тип сущности, для которого осуществляется поиск.</typeparam>
        /// <typeparam name="TProp">Тип поля, по которому осуществляется поиск</typeparam>
        /// <typeparam name="TBuildTreeNodeConfig">Тип конфига билдера дерева.</typeparam>
        /// <param name="rootNode">Корневой node билдера дерева.</param>
        /// <param name="expression">Добавляемое выражение.</param>
        /// <param name="callback">Callback-функция.</param>
        /// <param name="isNeedParentNode">Указывает, нужно ли возвращать в callback child или parent node. [По умолчанию выбирается child]</param>
        public static void AddExpressionToTree<TSource, TProp, TBuildTreeNodeConfig>(
            this BuildTreeNode<TBuildTreeNodeConfig> rootNode,
            Expression<Func<TSource, TProp>> expression,
            Action<BuildTreeNode<TBuildTreeNodeConfig>, PropertyInfo> callback,
            bool isNeedParentNode = false)
            where TBuildTreeNodeConfig : BuildTreeNodeConfig, new()
        {
            Stack<PropertyInfo> propertiesPath = new Stack<PropertyInfo>();

            // Записываем весь путь в стек.
            MemberExpression currentStep = expression.Body as MemberExpression;
            if (currentStep != null)
            {
                while (currentStep is MemberExpression)
                {
                    if (!(currentStep.Member is PropertyInfo))
                        continue;
                    propertiesPath.Push(currentStep.Member as PropertyInfo);
                    currentStep = currentStep.Expression as MemberExpression;
                }
            }

            // Проходим по стеку и достраиваем дерево.
            BuildTreeNode<TBuildTreeNodeConfig> node = rootNode;
            PropertyInfo pathPart = null;
            while (propertiesPath.Count != 0)
            {
                pathPart = propertiesPath.Pop();

                if (!pathPart.PropertyType.IsSimple())
                {
                    if (!node.ChildNodes.ContainsKey(pathPart.Name))
                        node.ChildNodes[pathPart.Name] = new BuildTreeNode<TBuildTreeNodeConfig>(pathPart);

                    if (!isNeedParentNode || propertiesPath.Count != 0 && isNeedParentNode)
                        node = node.ChildNodes[pathPart.Name];
                }
            }

            callback(node, pathPart);
        }

        /// <summary>
        /// Запускает бесконтрольный рекурсивный билдер дерева.
        /// </summary>
        /// <param name="type">Тип текущей ветки дерева.</param>
        /// <param name="newNode">Ветка дерева.</param>
        /// <remarks>
        /// TODO Надо добавить Hashset&ltType&gt чтобы отслеживать рекурсии и на них реагировать эксепшеном.
        /// </remarks>
        public static void AutoDepthBuild(Type type, PropertiesTreeNode newNode)
        {
            PropertyInfo[] allTypeProperties = type.GetProperties();

            if (allTypeProperties.Any(x => x.PropertyType == type))
                throw new ArgumentException($"Property type matched class type. Stack overflow is inevitable. Type: '{type}'");

            List<PropertiesTreeNode> newNodes = new List<PropertiesTreeNode>();
            foreach (var info in allTypeProperties)
            {
                if (!info.PropertyType.IsSimple())
                {
                    var newInnerNode = new PropertiesTreeNode(info);
                    AutoDepthBuild(info.PropertyType, newInnerNode);
                    newNodes.Add(newInnerNode);
                }
            }

            newNode.InnerProperties = allTypeProperties;
            newNode.Nodes = newNodes.ToArray();
        }

        /// <summary>
        /// Проверяет, принадлежит ли этот тип к простым типам.
        /// </summary>
        /// <param name="type">Проверяемый тип</param>
        /// <returns>Возвращает true, если тип является простым типом.</returns>
        public static bool IsSimple(this Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                return IsSimple(type.GetGenericArguments()[0]);

            return type.IsEnum
              || type.IsPrimitive
              || type == typeof(string)
              || type == typeof(DateTime)
              || type == typeof(decimal);
        }
    }
}

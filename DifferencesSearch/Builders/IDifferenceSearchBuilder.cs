using DifferencesSearch.Trees;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace DifferencesSearch.Builders
{
    public interface IDifferenceSearchBuilder
    {
        PropertiesTreeNode PropertyTree { get; }
        void Build();
    }

    public interface IDifferenceSearchBuilder<TSource> : IDifferenceSearchBuilder
    {
    }
}

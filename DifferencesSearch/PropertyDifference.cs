using System;
using System.Collections.Generic;
using System.Text;

namespace DifferencesSearch
{
    public class PropertyDifference
    {
        public Type ClassType { get; set; }
        public Type PropertyType { get; set; }
        public string PropertyName { get; set; }
        public object ValueLeft { get; set; }
        public object ValueRight { get; set; }
    }
}

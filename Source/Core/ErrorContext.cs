using System;
using System.Collections.Generic;

namespace XmlExtensions
{
    internal readonly struct ErrorContext(Type source, List<string> fields, List<string> values, string msg)
    {
        public readonly Type SourceType = source;
        public readonly List<string> Fields = fields;
        public readonly List<string> Values = values;
        public readonly string Message = msg;

        public override string ToString()
        {
            string details = SourceType.ToString();
            if (Fields != null && Fields.Count > 0)
            {
                details += $"({Fields[0]}='{Values[0]}'";
                for (int i = 1; i < Fields.Count; i++)
                    details += $", {Fields[i]}='{Values[i]}'";
                details += ")";
            }
            return $"{details}: {Message}";
        }
    }
}

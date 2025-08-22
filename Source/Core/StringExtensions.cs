using System.Collections.Generic;
using System.Text;
using Verse;

namespace XmlExtensions
{
    internal static class StringExtensions
    {
        extension(string str)
        {
            internal string TranslateIfTKeyAvailable(string tKey)
            {
                if (tKey != null)
                {
                    if (tKey.CanTranslate())
                        return tKey.Translate();
                    else
                        return str;
                }
                else
                    return str;
            }

            internal string SubstituteVariable(string var, string val, string brackets = "{}")
            {
                string variable = brackets[0] + var + brackets[1];
                return str.Replace(variable, val);
            }

            internal string SubstituteVariables(List<string> vars, List<string> vals, string brackets = "{}")
            {
                int i = 0;
                StringBuilder builder = new(str);
                foreach (string var in vars)
                {
                    builder.Replace(brackets[0] + var + brackets[1], vals[i]);
                    i++;
                }
                return builder.ToString();
            }
        }
    }
}

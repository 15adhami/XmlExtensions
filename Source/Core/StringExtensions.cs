using Verse;

namespace XmlExtensions
{
    internal static class StringExtensions
    {
        extension(string str)
        {
            internal string TryTKey(string tKey)
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
        }
    }
}

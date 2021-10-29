using System.Collections.Generic;

namespace XmlExtensions
{
    internal abstract class ErrorHandler
    {
        protected List<string> exceptionVals;
        protected List<string> exceptionFields;

        protected internal virtual void SetException()
        {
        }

        protected internal void CreateExceptions(params string[] values)
        {
            exceptionFields = new List<string>();
            exceptionVals = new List<string>();
            for (int i = 0; i < values.Length; i++)
            {
                if (i % 2 == 0)
                {
                    exceptionVals.Add(values[i]);
                }
                else
                {
                    exceptionFields.Add(values[i]);
                }
            }
        }

        /// <summary>
        /// Throw an error message.
        /// </summary>
        /// <param name="msg">The message</param>
        protected void Error(string msg)
        {
            SetException();
            string str = GetType().ToString();
            if (exceptionVals != null)
            {
                str += "(" + exceptionFields[0] + "=" + exceptionVals[0];
                for (int i = 1; i < exceptionVals.Count; i++)
                {
                    str += ", " + exceptionFields[i] + "=" + exceptionVals[i];
                }
                str += ")";
            }
            ErrorManager.AddError(str + ": " + msg);
        }

        protected internal void Error(string[] vals, string[] fields, string msg)
        {
            string str = GetType().ToString();
            if (vals != null)
            {
                str += "(" + fields[0] + "=" + vals[0];
                for (int i = 1; i < vals.Length; i++)
                {
                    str += ", " + fields[i] + "=" + vals[i];
                }
                str += ")";
            }
            ErrorManager.AddError(str + ": " + msg);
        }

        protected internal void NullError(string node)
        {
            Error("<" + node + "> is null");
        }

        protected internal void XPathError(string node = "xpath")
        {
            Error("Failed to find a node referenced by <" + node + ">");
        }
    }
}
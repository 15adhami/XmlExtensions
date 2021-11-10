using System.Collections.Generic;

namespace XmlExtensions
{
    /// <summary>
    /// The base type to inherit from in order to access improved error handling
    /// </summary>
    public abstract class ErrorHandler
    {
        private List<string> exceptionVals;
        private List<string> exceptionFields;

        private protected virtual void SetException()
        {
        }

        private protected void CreateExceptions(params string[] values)
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
                str += "(" + exceptionFields[0] + "='" + exceptionVals[0] + "'";
                for (int i = 1; i < exceptionVals.Count; i++)
                {
                    str += ", " + exceptionFields[i] + "='" + exceptionVals[i] + "'";
                }
                str += ")";
            }
            ErrorManager.AddError(str + ": " + msg);
        }

        private protected void Error(string[] vals, string[] fields, string msg)
        {
            string str = GetType().ToString();
            if (vals != null)
            {
                str += "(" + fields[0] + "='" + vals[0] + "'";
                for (int i = 1; i < vals.Length; i++)
                {
                    str += ", " + fields[i] + "='" + vals[i] + "'";
                }
                str += ")";
            }
            ErrorManager.AddError(str + ": " + msg);
        }

        private protected void NullError(string node)
        {
            Error("<" + node + "> is null");
        }

        private protected void XPathError(string node = "xpath")
        {
            Error("Failed to find a node referenced by <" + node + ">");
        }
    }
}
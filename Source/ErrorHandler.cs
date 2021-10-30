using System.Collections.Generic;

namespace XmlExtensions
{
    /// <summary>
    /// The base type to inherit from in order to access imoroved error handlingssS
    /// </summary>
    public abstract class ErrorHandler
    {
        /// <summary>
        /// The list of values of the relevant fields
        /// </summary>
        protected List<string> exceptionVals;

        /// <summary>
        /// The names of the fields that should be displayed in parenthesis
        /// </summary>
        protected List<string> exceptionFields;

        /// <summary>
        /// Override this method to set the relevaant fields for error handling
        /// </summary>
        protected internal virtual void SetException()
        {
        }

        /// <summary>
        /// Helper method to set the exceptions
        /// </summary>
        /// <param name="values">Formatted as: fieldVal1, "fieldName1", ...</param>
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

        /// <summary>
        /// Throw an error message using the given fields
        /// </summary>
        /// <param name="vals">An array containing the values of the fields</param>
        /// <param name="fields">An array containing the names of the fields</param>
        /// <param name="msg">The error message</param>
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

        /// <summary>
        /// Throw this error if a tag is null
        /// </summary>
        /// <param name="node">The name of the node</param>
        protected internal void NullError(string node)
        {
            Error("<" + node + "> is null");
        }

        /// <summary>
        /// Throw this error if the xpath failed to return a node
        /// </summary>
        /// <param name="node">The name of the node containing the xpath</param>
        protected internal void XPathError(string node = "xpath")
        {
            Error("Failed to find a node referenced by <" + node + ">");
        }
    }
}
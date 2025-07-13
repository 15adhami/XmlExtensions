using UnityEngine;

namespace XmlExtensions.Action
{
    internal class OpenURL : ActionContainer
    {
        public string url;

        protected override bool ApplyAction()
        {
            if (url != null) { Application.OpenURL(url); }
            return true;
        }
    }
}

using System;
using System.Xml;
using Verse;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlExtensions.Action
{
    internal class RestartGame : ActionContainer
    { // TODO: Close standard mod menu or xml extensiosn menu before restarting
        protected override bool ApplyAction()
        {
            if (Find.WindowStack.IsOpen(typeof(XmlExtensions_MenuModSettings)))
            {
                Find.WindowStack.RemoveWindowsOfType(typeof(XmlExtensions_MenuModSettings));
            }
            GenCommandLine.Restart();
            return true;
        }
    }
}
﻿using System.Xml;

namespace XmlExtensions
{
    public class StopwatchResume : PatchOperationExtended
    {
        protected override bool Patch(XmlDocument xml)
        {
            PatchManager.watch.Start();
            return true;
        }
    }
}
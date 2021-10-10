﻿using System.Xml;

namespace XmlExtensions
{
    public class StopwatchStart : PatchOperationExtended
    {
        protected override bool Patch(XmlDocument xml)
        {
            PatchManager.watch.Reset();
            PatchManager.watch.Start();
            return true;
        }
    }
}
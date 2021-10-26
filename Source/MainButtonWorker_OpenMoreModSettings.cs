﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace XmlExtensions.Source
{
    class MainButtonWorker_OpenMoreModSettings : MainButtonWorker
    {
        public override void Activate()
        {
            Find.WindowStack.Add(new XmlExtensions_SettingsMenu());
        }
    }
}

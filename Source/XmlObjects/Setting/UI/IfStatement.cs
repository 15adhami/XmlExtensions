using System.Collections.Generic;
using System.Xml;
using System;
using UnityEngine;
using Verse;
using XmlExtensions.Boolean;

namespace XmlExtensions.Setting
{
    internal class IfStatement : SettingContainer
    {
        public BooleanBase condition = null;
        public Frequency evalFrequency = Frequency.Realtime;
        public List<SettingContainer> caseTrue;
        public List<SettingContainer> caseFalse;

        public enum Frequency
        {
            OnLoad,
            PreOpen,
            Realtime
        }

        private bool cachedResult = false;

        protected override bool Init()
        {
            addDefaultSpacing = false;
            if (evalFrequency == Frequency.OnLoad && !condition.Evaluate(ref cachedResult, null))
            {
                return false;
            }
            if (!InitializeContainers(menuDef, caseTrue, "caseTrue"))
            {
                return false;
            }
            if (!InitializeContainers(menuDef, caseFalse, "caseFalse"))
            {
                return false;
            }
            return true;
        }

        protected override float CalculateHeight(float width)
        {
            if (evalFrequency == Frequency.Realtime && !condition.Evaluate(ref cachedResult, null))
            {
                throw new Exception("Error in evaluating <condition>");
            }
            return cachedResult ? CalculateHeightSettingsList(width, caseTrue) : CalculateHeightSettingsList(width, caseFalse);
        }

        protected override void DrawSettingContents(Rect inRect)
        {
            List<SettingContainer> settings;
            if (cachedResult) { settings = caseTrue; }
            else { settings = caseFalse; }
            if (settings != null) { DrawSettingsList(inRect, settings); }
        }

        internal override bool PreOpen()
        {
            if (evalFrequency == Frequency.PreOpen && !condition.Evaluate(ref cachedResult, null))
            {
                throw new Exception("Error in evaluating <condition>");
            }
            if (!PreOpenContainers(caseTrue, "caseTrue"))
            {
                return false;
            }
            else
                return PreOpenContainers(caseFalse, "caseFalse");
        }

        internal override bool PostClose()
        {
            if (evalFrequency == Frequency.PreOpen && !condition.Evaluate(ref cachedResult, null))
            {
                throw new Exception("Error in evaluating <condition>");
            }
            if (!PostCloseContainers(caseTrue, "caseTrue"))
            {
                return false;
            }
            else
                return PostCloseContainers(caseFalse, "caseFalse");
        }
    }
}
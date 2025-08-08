using System.Collections.Generic;
using System;
using UnityEngine;
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
            if (!InitializeContainers(caseTrue, "caseTrue"))
            {
                return false;
            }
            if (!InitializeContainers(caseFalse, "caseFalse"))
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

        protected override bool PreOpen()
        {
            if (evalFrequency == Frequency.PreOpen && !condition.Evaluate(ref cachedResult, null))
            {
                throw new Exception("Error in evaluating <condition>");
            }
            return true;
        }
    }
}
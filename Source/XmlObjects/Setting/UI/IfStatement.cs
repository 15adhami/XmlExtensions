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

        protected override bool Init(string selectedMod)
        {
            addDefaultSpacing = false;
            if (evalFrequency == Frequency.OnLoad && !condition.Evaluate(ref cachedResult, null))
            {
                return false;
            }
            if (!InitializeSettingsList(selectedMod, caseTrue, "caseTrue"))
            {
                return false;
            }
            if (!InitializeSettingsList(selectedMod, caseFalse, "caseFalse"))
            {
                return false;
            }
            return true;
        }

        protected override float CalculateHeight(float width, string selectedMod)
        {
            if (evalFrequency == Frequency.Realtime && !condition.Evaluate(ref cachedResult, null))
            {
                throw new Exception("Error in evaluating <condition>");
            }
            return cachedResult ? CalculateHeightSettingsList(width, selectedMod, caseTrue) : CalculateHeightSettingsList(width, selectedMod, caseFalse);
        }

        protected override void DrawSettingContents(Rect inRect, string selectedMod)
        {
            List<SettingContainer> settings;
            if (cachedResult) { settings = caseTrue; }
            else { settings = caseFalse; }
            if (settings != null) { DrawSettingsList(inRect, selectedMod, settings); }
        }

        internal override bool PreOpen(string selectedMod)
        {
            if (evalFrequency == Frequency.PreOpen && !condition.Evaluate(ref cachedResult, null))
            {
                throw new Exception("Error in evaluating <condition>");
            }
            if (!PreOpenSettingsList(selectedMod, caseTrue, "caseTrue"))
            {
                return false;
            }
            else
                return PreOpenSettingsList(selectedMod, caseFalse, "caseFalse");
        }
    }
}
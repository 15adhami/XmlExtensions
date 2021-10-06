using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    public abstract class CustomSettingContainer : SettingContainer
    {
        protected sealed override void DrawSettingContents(Listing_Standard listingStandard, string selectedMod)
        {
            DoSettingContents(listingStandard.GetRect(CalculateHeight(listingStandard.ColumnWidth, selectedMod)), selectedMod);
        }

        /// <summary>
        /// Code that will be run every frame for this setting.
        /// </summary>
        /// <param name="rect">The <c>Rect</c> that your setting must be displayed in. Its height is determined by <c>CalculateHeight()</c>.</param>
        /// <param name="selectedMod">The modId of the active mod in the settings window.</param>
        public virtual void DoSettingContents(Rect rect, string selectedMod) { }

        /// <summary>
        /// The height of your setting. Will be run every frame.
        /// </summary>
        /// <param name="width">The width of the <c>Rect</c> that will be given to you.</param>
        /// <param name="selectedMod">The modId of the active mod in the settings window.</param>
        /// <returns>The current height of your setting for the current frame.</returns>
        public virtual float CalculateHeight(float width, string selectedMod) { return 0; }

        protected sealed override int CalcHeight(float width, string selectedMod)
        {
            return (int)CalculateHeight(width, selectedMod);
        }
    }
}

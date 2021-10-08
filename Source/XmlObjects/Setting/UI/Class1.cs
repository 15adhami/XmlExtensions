using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace XmlExtensions
{
    public class UICheckbox : UIContainer
    {
        public override float CalculateHeight(float width)
        {
            return 22 + 2;
        }

        public override void DrawSettingContents(Rect inRect, List<object> inputs)
        {
            bool b = (bool)inputs[parameter];
            Widgets.CheckboxLabeled(inRect, "test", ref b);
            inputs[parameter] = b;
        }
    }

    public abstract class UIContainer
    {
        public int parameter = 0;

        public virtual float CalculateHeight(float width)
        {
            return 0;
        }

        public virtual void DrawSettingContents(Rect inRect, List<object> inputs)
        {
        }
    }
}

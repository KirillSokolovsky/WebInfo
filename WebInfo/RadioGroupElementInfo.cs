namespace WebInfo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class RadioGroupElementInfo : CombinedWebElementInfo
    {
        public RadioGroupElementInfo()
        {
            ElementType = WebElementTypes.RadioGroup;
        }

        public WebElementInfo Option() => Elements.FirstOrDefault(e => e.InnerKey == Keys.Option);

        public override WebElementInfo GetCopyWithoutParent(WebElementInfo webElementInfo = null)
        {
            var element = webElementInfo as RadioGroupElementInfo
                ?? new RadioGroupElementInfo();

            return base.GetCopyWithoutParent(element);
        }

        public static class Keys
        {
            public const string Option = "Option";
        }
    }
}

namespace WebInfo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class DropDownElementInfo : CombinedWebElementInfo
    {
        public DropDownElementInfo()
        {
            ElementType = WebElementTypes.DropDown;
        }

        public WebElementInfo Input() => Elements.FirstOrDefault(e => e.InnerKey == Keys.Input);
        public WebElementInfo Option() => Elements.FirstOrDefault(e => e.InnerKey == Keys.Option);

        public override WebElementInfo GetCopyWithoutParent(WebElementInfo webElementInfo = null)
        {
            var element = webElementInfo as DropDownElementInfo
                ?? new DropDownElementInfo();

            return base.GetCopyWithoutParent(element);
        }

        public static class Keys
        {
            public const string Input = "Input";
            public const string Option = "Option";
        }
    }
}

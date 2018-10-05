namespace WebInfo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class WebContext : CombinedWebElementInfo
    {
        public WebContext()
        {
            ElementType = WebElementTypes.Context;
        }

        public override WebElementInfo GetCopyWithoutParent(WebElementInfo webElementInfo = null)
        {
            var element = webElementInfo as WebContext
                ?? new WebContext();

            return base.GetCopyWithoutParent(element);
        }
    }
}

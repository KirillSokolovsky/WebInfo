namespace WebInfo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class CombinedWebElementInfo : WebElementInfo
    {
        public CombinedWebElementInfo()
        {
            ElementType = WebElementTypes.Control;
        }

        public List<WebElementInfo> Elements { get; set; }

        public override WebElementInfo GetCopyWithoutParent(WebElementInfo webElementInfo = null)
        {
            var element = webElementInfo as CombinedWebElementInfo
                ?? new CombinedWebElementInfo();

            element.Elements = Elements?.Select(e => e.GetCopyWithoutParent())
                .ToList();
            element.Elements.ForEach(e => e.Parent = element);

            return base.GetCopyWithoutParent(element);
        }

        public List<CombinedWebElementInfo> GetChildContexts()
        {
            var childrenContexts = new List<CombinedWebElementInfo>();
            foreach (var child in Elements)
            {
                if (child is CombinedWebElementInfo combined)
                    childrenContexts.Add(combined);
            }
            return childrenContexts;
        }
    }
}

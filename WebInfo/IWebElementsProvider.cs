namespace WebInfo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IWebElementsProvider
    {
        void EnterContext(string contextName);
        void LeaveContext();
        void ClearContext();

        Stack<WebContext> GetContextStack();

        WebElementInfo GetElement(string elementName);
        List<WebElementInfo> GetElements();
        List<WebElementInfo> GetElementsWithTags(params string[] tags);
        WebElementInfo GetKeyElement();
    }
}

using System.Collections.Generic;

namespace WebInfo
{
    public interface IWebElementsReadOnlyRepository
    {
        void LoadWebContexts();
        WebContext GetWebContextOrDefault(string contextName);
        List<WebContext> GetWebContexts();
    }
}
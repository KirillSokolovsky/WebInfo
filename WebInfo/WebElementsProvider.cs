namespace WebInfo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class WebElementsProvider : IWebElementsProvider
    {
        private readonly IWebElementsReadOnlyRepository _webElementRepository;
        private readonly Stack<CombinedWebElementInfo> _contextStack = new Stack<CombinedWebElementInfo>();

        public WebElementsProvider(IWebElementsReadOnlyRepository webElementRepository)
        {
            _webElementRepository = webElementRepository;
        }

        public void ClearContext()
        {
            _contextStack.Clear();
        }

        public void EnterContext(string contextName)
        {
            CombinedWebElementInfo webContext = null;
            while (_contextStack.Count > 0)
            {
                var currentContext = _contextStack.Peek();
                webContext = FindContextInDescendants(currentContext, contextName);
                if (webContext == null)
                    _contextStack.Pop();
                else break;
            }

            if (webContext == null)
                webContext = _webElementRepository.GetWebContextOrDefault(contextName);

            if (webContext == null)
                throw new Exception($"Couldn't find WebContext with name: {contextName}");

            _contextStack.Push(webContext);
        }

        private CombinedWebElementInfo FindContextInDescendants(CombinedWebElementInfo parent, string contextName)
        {
            var childContexts = parent.GetChildContexts();

            foreach (var child in childContexts)
            {
                if (child.Name == contextName) return child;
            }

            foreach (var child in childContexts)
            {
                var context = FindContextInDescendants(child, contextName);
                if (context != null) return context;
            }

            return null;
        }

        public Stack<WebContext> GetContextStack()
        {
            return null;
        }

        public WebElementInfo GetElement(string elementName)
        {
            var currentContext = _contextStack.Peek();
            var element = currentContext.Elements.FirstOrDefault(e => e.Name == elementName);

            if (element == null)
            {
                var descElements = FindElementInDescendants(currentContext, elementName);
                element = descElements.FirstOrDefault();

                if (descElements.Count > 1)
                    throw new Exception($"Found {descElements.Count} elements with name {elementName} relative to context {currentContext}");
            }

            var message = $"Couldn't find elemen with name {elementName} relative to context {currentContext}";
            if (element == null)
            {
                if (_contextStack.Count == 1)
                    throw new Exception(message);
                else
                {
                    var item = _contextStack.Pop();
                    try
                    {
                        element = GetElement(elementName);
                    }
                    catch
                    {
                        message += $"{Environment.NewLine} or in parent contexts";
                    }
                    finally
                    {
                        _contextStack.Push(item);
                    }
                }
            }

            if (element == null)
                throw new Exception(message);

            return element;
        }

        private List<WebElementInfo> FindElementInDescendants(CombinedWebElementInfo parent, string elementName)
        {
            var els = new List<WebElementInfo>();
            var queue = new Queue<CombinedWebElementInfo>();
            var childContexts = parent.GetChildContexts();

            foreach (var child in childContexts)
            {
                queue.Enqueue(child);
            }

            while (queue.Count > 0)
            {
                var currentContext = queue.Dequeue();
                var element = currentContext.Elements.FirstOrDefault(e => e.Name == elementName);
                if (element != null) els.Add(element);

                foreach (var child in currentContext.GetChildContexts())
                {
                    queue.Enqueue(child);
                }
            }

            return els;
        }

        public List<WebElementInfo> GetElements()
        {
            throw new NotImplementedException();
        }

        public List<WebElementInfo> GetElementsWithTags(params string[] tags)
        {
            throw new NotImplementedException();
        }

        public WebElementInfo GetKeyElement()
        {
            return _contextStack.Peek().Elements.FirstOrDefault(e => e.IsKey);
        }

        public void LeaveContext()
        {
            _contextStack.Pop();
        }
    }
}

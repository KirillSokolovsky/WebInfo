namespace WebInfo.Desktop.Models
{
    using WebInfo;
    using ReactiveUI;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class CombinedElementViewModel : WebElementViewModel
    {
        private ObservableCollection<WebElementViewModel> _elements;
        public ObservableCollection<WebElementViewModel> Elements
        {
            get => _elements;
            set => this.RaiseAndSetIfChanged(ref _elements, value);
        }

        public override bool Filter(string text)
        {
            var result = base.Filter(text);
            if (Elements != null)
            {
                foreach (var element in Elements)
                {
                    result = element.Filter(text) || result;
                }
            }

            if (result)
                Show();
            else Hide();

            return result;
        }

        public override void FillFromInfo(WebElementInfo info)
        {
            if (!(info is CombinedWebElementInfo combined))
                throw new Exception($"Incorrect info type: {info.GetType().Name} for model with type CombinedElementViewModel");

            base.FillFromInfo(info);

            if (combined.Elements == null) return;
            Elements = new ObservableCollection<WebElementViewModel>();
            foreach (var element in combined.Elements)
            {
                var model = WebFactory.CreateModelFromInfo(element);
                model.Parent = this;
                Elements.Add(model);
            }
        }

        public override void FillInfo(WebElementInfo info)
        {
            if (!(info is CombinedWebElementInfo combined))
                throw new Exception($"Incorrect info type: {info.GetType().Name} for model with type CombinedElementViewModel");

            base.FillInfo(combined);

            if (Elements != null)
            {
                combined.Elements = new List<WebElementInfo>();

                foreach (var element in Elements)
                {
                    var el = WebFactory.CreateInfoFromModel(element);
                    element.FillInfo(el);
                    el.Parent = combined;
                    combined.Elements.Add(el);
                }
            }
        }

        public List<string> GetExistedNames()
        {
            return Elements?.Select(e => e.Name).ToList();
        }

        public bool IsSelfOrChildren(WebElementViewModel model)
        {
            if (model == this) return true;
            if (Elements == null) return false;
            foreach (var element in Elements)
            {
                if (element == model) return true;
                if (element is CombinedElementViewModel combined)
                {
                    if (combined.IsSelfOrChildren(model)) return true;
                }
            }

            return false;
        }
    }
}

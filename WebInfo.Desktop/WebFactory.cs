namespace WebInfo.Desktop
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using WebInfo;
    using Models;
    using System.Collections.ObjectModel;

    public static class WebFactory
    {
        public static WebElementViewModel CreateModelFromInfo(WebElementInfo info)
        {
            WebElementViewModel model = null;

            if (info is WebContext webContext)
            {
                model = new WebContextViewModel();
                model.FillFromInfo(webContext);
            }
            else if (info is DropDownElementInfo dropDown)
            {
                model = new DropDownElementViewModel();
                model.FillFromInfo(dropDown);
            }
            else if (info is RadioGroupElementInfo radioGroup)
            {
                model = new RadioGroupViewModel();
                model.FillFromInfo(radioGroup);
            }
            else if (info is CombinedWebElementInfo combined)
            {
                model = new CombinedElementViewModel();
                model.FillFromInfo(combined);
            }
            else
            {
                model = new WebElementViewModel();
                model.FillFromInfo(info);
            }

            return model;
        }

        public static WebElementInfo CreateInfoFromModel(WebElementViewModel model)
        {
            WebElementInfo info = null;

            if (model is WebContextViewModel webContext)
            {
                info = new WebContext();
                webContext.FillInfo(info);
            }
            else if (model is DropDownElementViewModel dropDown)
            {
                info = new DropDownElementInfo();
                dropDown.FillInfo(info);
            }
            else if (model is RadioGroupViewModel radioGroup)
            {
                info = new RadioGroupElementInfo();
                radioGroup.FillInfo(info);
            }
            else if (model is CombinedElementViewModel combined)
            {
                info = new CombinedWebElementInfo();
                combined.FillInfo(info);
            }
            else
            {
                info = new WebElementInfo();
                model.FillInfo(info);
            }

            return info;
        }

        public const string MockString = "ChangeMe";

        public static WebElementViewModel CreateWebElementModel(WebElementViewModel model = null)
        {
            if (model == null)
                model = new WebElementViewModel { ElementType = WebElementTypes.Element };

            model.Name = MockString;
            model.Description = MockString;
            model.WebLocator =
                new WebLocatorViewModel
                {
                    LocatorValue = MockString
                };

            return model;
        }

        public static CombinedElementViewModel CreateCombinedElementModel(CombinedElementViewModel model = null)
        {
            var wasNull = model == null;
            if (wasNull)
                model = new CombinedElementViewModel { ElementType = WebElementTypes.Control };

            model.Elements = new ObservableCollection<WebElementViewModel>();
            CreateWebElementModel(model);

            if (wasNull)
                model.Name += " Control";

            return model;
        }

        public static DropDownElementViewModel CreateDropDownElementModel()
        {
            var model = new DropDownElementViewModel();
            model.ElementType = WebElementTypes.DropDown;
            CreateCombinedElementModel(model);
            model.Name += " DropDown";

            var input = CreateWebElementModel();
            input.Parent = model;
            input.Name = "DropDown Input";
            input.InnerKey = DropDownElementInfo.Keys.Input;
            input.Description = "DropDown input, used to display curren selected value or to expand options by clicking";
            model.Elements.Add(input);

            var option = CreateWebElementModel();
            option.Parent = model;
            option.Name = "DropDown Option";
            option.InnerKey = DropDownElementInfo.Keys.Option;
            option.Description = "DropDown option, used to select value in drop down";
            model.Elements.Add(option);

            return model;
        }

        public static RadioGroupViewModel CreateRadioGroupModel()
        {
            var model = new RadioGroupViewModel();
            model.ElementType = WebElementTypes.RadioGroup;
            CreateCombinedElementModel(model);
            model.Name += " RadioGroup";

            var option = CreateWebElementModel();
            option.Parent = model;
            option.Name = "RadiGroup Option";
            option.InnerKey = RadioGroupElementInfo.Keys.Option;
            option.Description = "RadiGroup option, used to select value in radio group";
            model.Elements.Add(option);

            return model;
        }

        public static WebContextViewModel CreateWebContextModel()
        {
            var model = new WebContextViewModel();
            model.ElementType = WebElementTypes.Context;
            CreateCombinedElementModel(model);
            model.Name += " Page";
            return model;
        }
    }
}

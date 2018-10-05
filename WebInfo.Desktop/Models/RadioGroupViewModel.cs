namespace WebInfo.Desktop.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using WebInfo;

    public class RadioGroupViewModel : CombinedElementViewModel
    {
        public override void FillFromInfo(WebElementInfo info)
        {
            if (!(info is CombinedWebElementInfo combined))
                throw new Exception($"Incorrect info type: {info.GetType().Name} for model with type RadioGroupViewModel");

            base.FillFromInfo(info);
        }
    }
}

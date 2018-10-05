namespace WebInfo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public static class WebElementTypes
    {
        //Base items

        /// <summary>
        /// Single web element
        /// </summary>
        public const string Element = nameof(Element);

        /// <summary>
        /// Context web element. Contains other element
        /// </summary>
        public const string Context = nameof(Context);

        /// <summary>
        /// Element that includes set of child elements
        /// </summary>
        public const string Control = nameof(Control);

        //Custom items

        public const string DropDown = nameof(DropDown);
        public const string RadioGroup = nameof(RadioGroup);
    }
}

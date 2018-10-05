namespace WebInfo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    public class WebLocatorDefaultDynamicValuerResolver : IWebLocatorDynamicValueResolver
    {
        private static Regex _dynamicLocatorRegex = new Regex(@"\{(.+?)(:.+?)?(\|.+?)?\}", RegexOptions.Compiled);
        public string ResolveDynamicLocatorValue(string locator, params (string parName, object parValue)[] values)
        {
            if (values.Length == 0) return locator;

            var locatorParts = locator.Split(new[] { "'$", "$'" }, StringSplitOptions.RemoveEmptyEntries);
            var locatorPlaceholders = new List<(int line, string varName, string format, string type)>();

            for (int i = 1; i < locatorParts.Length; i += 2)
            {
                var locatorPart = locatorParts[i];
                var match = _dynamicLocatorRegex.Match(locatorPart);

                if (match.Success)
                {
                    var varName = match.Groups[1].Value;
                    var format = match.Groups[2].Value;
                    if (!string.IsNullOrEmpty(format))
                        format = format.Substring(1);

                    var type = match.Groups[3].Value;
                    if (!string.IsNullOrEmpty(type))
                        type = type.Substring(1);

                    var parName = values[i];
                    var parValue = values[i + 1];

                    locatorPlaceholders.Add((i, varName, format, type));
                }
            }


            if (values.All(v => v.parName == null)) //by index mode
            {
                if (values.Length > locatorPlaceholders.Count)
                    throw new Exception($"Error occurred during dynamic locator resolving." +
                        $"{Environment.NewLine}Parameters count is greater then placeholder number. {values.Length} > {locatorPlaceholders.Count}" +
                        $"{Environment.NewLine}Locator: {locator}" +
                        $"{Environment.NewLine}Parameters: {string.Join(", ", values.Select(v => $"{v.parName}: {v.parValue}"))}");

                for (int i = 0; i < values.Length; i++)
                {
                    var (line, varName, format, type) = locatorPlaceholders[i];
                    var part = locatorParts[line];
                    var formatted = string.Format($"{{0:{format}}}", values[i].parValue);
                    var rp = _dynamicLocatorRegex.Replace(part, $"'{formatted}'");
                    locatorParts[line] = rp;
                }
            }
            else //by name mode
            {
                var extra = values.Where(v => !locatorPlaceholders.Any(lp => lp.varName == v.parName))
                    .Select(v => v.parName).ToList();

                if (extra.Count > 0)
                    throw new Exception($"Error occurred during dynamic locator resolving." +
                        $"{Environment.NewLine}Found extra parameters: {string.Join(", ", extra)}" +
                        $"{Environment.NewLine}Locator: {locator}" +
                        $"{Environment.NewLine}Parameters: {string.Join(", ", values.Select(v => $"{v.parName}: {v.parValue}"))}");

                var emptyNames = values.Where(v => string.IsNullOrWhiteSpace(v.parName))
                    .Select(v => v.parValue).ToList();

                if (emptyNames.Count > 0)
                    throw new Exception($"Error occurred during dynamic locator resolving." +
                        $"{Environment.NewLine}Found parameters without names." +
                        $"{Environment.NewLine}Locator: {locator}" +
                        $"{Environment.NewLine}Parameters: {string.Join(", ", values.Select(v => $"{v.parName}: {v.parValue}"))}");



                for (int i = 0; i < values.Length; i++)
                {
                    var (line, varName, format, type) = locatorPlaceholders.First(lp => lp.varName == values[i].parName);
                    var part = locatorParts[line];
                    var formatted = string.Format($"{{0:{format}}}", values[i].parValue);
                    var rp = _dynamicLocatorRegex.Replace(part, $"'{formatted}'");
                    locatorParts[line] = rp;
                }
            }

            locator = string.Join("", locatorParts);

            return locator;
        }
    }
}

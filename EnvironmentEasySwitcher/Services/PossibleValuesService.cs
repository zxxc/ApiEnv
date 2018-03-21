using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EES.ComboBox.Services
{
    public class PossibleValuesService
    {
        public const string DefaultItemName = "TestSettings";


        public string[] GetItems(string slnFile)
        {
            List<string> dropDownComboChoices = new List<string>();
            if (string.IsNullOrWhiteSpace(slnFile))
            {
                return new string[0];
            }
            string solutionDir = System.IO.Path.GetDirectoryName(slnFile);
            string[] files = Directory.GetFiles(solutionDir, "TestSettings.*.json", SearchOption.AllDirectories);


            return new[] { DefaultItemName }.Concat(
                    files
                        .Select(Path.GetFileName)
                        .Select(s => s.Replace("TestSettings.", String.Empty)
                            .Replace(".json", String.Empty)))
                .Distinct()
                .ToArray();
        }

    }
}
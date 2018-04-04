using System;
using System.IO;
using System.Linq;

namespace EES.ComboBox.Services
{
    public class PossibleValuesProvider
    {
        private readonly string mask;

        public PossibleValuesProvider(string mask)
        {
            this.mask = mask;
        }
        public string[] GetItems(string slnFile)
        {
            if (string.IsNullOrWhiteSpace(mask) || string.IsNullOrWhiteSpace(slnFile))
            {
                return new string[0];
            }
            string solutionDir = System.IO.Path.GetDirectoryName(slnFile);
            if (solutionDir == null)
            {
                return new string[0];
            }

            string[] files = Directory.GetFiles(solutionDir, mask, SearchOption.AllDirectories);

            return files.Select(Path.GetFileName)
                        .Select(s =>
                {
                    var i = mask.IndexOf("*", StringComparison.InvariantCulture);
                    var end = mask.Length - i - 1;
                    return s.Substring(i, s.Length - i - end);
                }).ToArray();
        }

    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EES.Core
{
    public class Class2
    {
        readonly HashSet<string> testRunners = new HashSet<string>
        {
            "vstest","mstest","xunit"
        };

        public string KillTestRunners()
        {
            var runners = System.Diagnostics.Process.GetProcesses().Where(s => testRunners.Contains(s.ProcessName));

            StringBuilder sb = new StringBuilder();
            foreach (var process in runners)
            {
                process.Kill();
                sb.Append(process.ProcessName);
                sb.Append(",");
            }
            if (sb.Length > 0)
            {
                sb.Length--;
            }
            return sb.ToString();
        }
    }
    public class PossibleValuesService
    {
        public const string defaultItemName = "TestSettings";


        public string[] GetItems(string slnFile)
        {
            List<string> _dropDownComboChoices = new List<string>();
            if (string.IsNullOrWhiteSpace(slnFile))
            {
                return Array.Empty<string>();
            }
            string solutionDir = System.IO.Path.GetDirectoryName(slnFile);
            string[] files = Directory.GetFiles(solutionDir, "TestSettings.*.json", SearchOption.AllDirectories);


            return new[] { defaultItemName }.Concat(
                files
                .Select(Path.GetFileName)
                .Select(s => s.Replace("TestSettings.", String.Empty)
                                    .Replace(".json", String.Empty)))
                .Distinct()
                .ToArray();
        }

    }
    public class EnvValueService
    {

        const String envName = "EnvTest";
        const EnvironmentVariableTarget environmentVariableTarget = EnvironmentVariableTarget.Machine;
        public string GetSelectedItem()
        {
            return Environment.GetEnvironmentVariable(envName, environmentVariableTarget);
        }

        public void ChangeEnv(string currentDropDownComboChoice, Action<string, string> ShowMessage)
        {
                        string runners = new Class2().KillTestRunners();

            String envValue = currentDropDownComboChoice == PossibleValuesService.defaultItemName 
                ? String.Empty 
                : currentDropDownComboChoice;

            Environment.SetEnvironmentVariable(envName, envValue, environmentVariableTarget);

            ShowMessage("TestSettings Selector", "Changed to " + currentDropDownComboChoice + (string.IsNullOrEmpty(runners) ? "" : "Killed :" + runners));
        }


    }
}

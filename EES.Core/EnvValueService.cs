using System;

namespace EES.Core
{
    public class EnvValueService
    {
        const String EnvName = "EnvTest";
        const EnvironmentVariableTarget environmentVariableTarget = EnvironmentVariableTarget.Machine;
        public string GetSelectedItem()
        {
            return Environment.GetEnvironmentVariable(EnvName, environmentVariableTarget);
        }

        public void ChangeEnv(string currentDropDownComboChoice, Action<string, string> ShowMessage)
        {
            string runners = new TestRunnersService().KillTestRunners();

            String envValue = currentDropDownComboChoice == PossibleValuesService.DefaultItemName 
                ? String.Empty 
                : currentDropDownComboChoice;

            Environment.SetEnvironmentVariable(EnvName, envValue, environmentVariableTarget);

            ShowMessage("TestSettings Selector", "Changed to " + currentDropDownComboChoice + (string.IsNullOrEmpty(runners) ? "" : "Killed :" + runners));
        }


    }
}
using System;

namespace EES.ComboBox.Services
{
    public class EnvValueService
    {
        const String EnvName = "EnvTest";
        const EnvironmentVariableTarget EnvironmentVariableTarget = System.EnvironmentVariableTarget.Machine;
        public string GetSelectedItem()
        {
            return Environment.GetEnvironmentVariable(EnvName, EnvironmentVariableTarget);
        }

        public void ChangeEnv(string currentDropDownComboChoice, Action<string, string> showMessage)
        {
            string runners = new TestRunnersService().KillTestRunners();

            String envValue = currentDropDownComboChoice == PossibleValuesService.DefaultItemName 
                ? String.Empty 
                : currentDropDownComboChoice;

            Environment.SetEnvironmentVariable(EnvName, envValue, EnvironmentVariableTarget);

            showMessage("TestSettings Selector", "Changed to " + currentDropDownComboChoice + (string.IsNullOrEmpty(runners) ? "" : "Killed :" + runners));
        }


    }
}
using System;

namespace EES.ComboBox.Services
{
    public class EnvValueService
    {
        public const string Empty = "<empty>";
        private string EnvName { get; }
        const EnvironmentVariableTarget EnvironmentVariableTarget = System.EnvironmentVariableTarget.Process;

        public EnvValueService(string envName )
        {
            EnvName = envName ;
        }
        public string GetSelectedItem()
        {
            return Environment.GetEnvironmentVariable(EnvName, EnvironmentVariableTarget);
        }

        public void ChangeEnv(string currentDropDownComboChoice, Action<string, string> showMessage)
        {
            string runners = new TestRunnersService().KillTestRunners();

            var envValue = currentDropDownComboChoice == Empty 
                           || string.IsNullOrWhiteSpace(currentDropDownComboChoice)
                ? String.Empty
                : currentDropDownComboChoice;
            
            Environment.SetEnvironmentVariable(EnvName, envValue, EnvironmentVariableTarget);

            showMessage("TestSettings Selector", "Changed to " + currentDropDownComboChoice + (string.IsNullOrEmpty(runners) ? "" : "Killed :" + runners));
        }


    }
}
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EES.ComboBox.Services
{
    public class TestRunnersService
    {
        readonly HashSet<string> _testRunners = new HashSet<string>
        {
            "vstest","mstest","xunit"
        };

        public string KillTestRunners()
        {
            var runners = System.Diagnostics.Process.GetProcesses().Where(s => _testRunners.Contains(s.ProcessName));

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
}

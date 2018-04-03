using System;

namespace EES.ComboBox
{
    static class GuidList
    {
        public const string guidComboBoxPkgString = "B5B16A7D-DC2F-4580-A7B6-0C9B11D3B0B9";
        public const string guidComboBoxCmdSetString = "808E02EE-0633-4FC3-AA8B-8280777CB44F";

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        public static readonly Guid guidComboBoxPkg = new Guid(guidComboBoxPkgString);
        public static readonly Guid guidComboBoxCmdSet = new Guid(guidComboBoxCmdSetString);


        public const string OptionsPageGeneral = "648D7F79-447D-4979-BD39-371903EAF9EA";
    };
}
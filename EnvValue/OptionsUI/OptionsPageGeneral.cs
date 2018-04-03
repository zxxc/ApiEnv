using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using EES.ComboBox;

namespace Microsoft.Samples.VisualStudio.IDE.OptionsPage
{
    /// <summary>
    // Extends the standard dialog functionality for implementing ToolsOptions pages, 
    // with support for the Visual Studio automation model, Windows Forms, and state 
    // persistence through the Visual Studio settings mechanism.
    /// </summary>
    [Guid(GuidList.OptionsPageGeneral)]
    public class OptionsPageGeneral : DialogPage
    {
        #region Constructors

        public OptionsPageGeneral()
        {
            PossibleValues = new string[]
            {
                "local"
            };
        }

        #endregion

        #region Properties

        [Category("Environment values")]
        [Description("Saved Env values")]
        public string[] SavedEnvValues { get; set; }

        [Category("Environment values")]
        [Description("Env values")]
        public string[] PossibleValues { get; set; }

        /// <summary>
        /// Gets or sets the String type custom option value.
        /// </summary>
        /// <remarks>This value is shown in the options page.</remarks>
        [Category("Environment values")]
        [Description("My string option")]
        public string OptionString { get; set; }

        #endregion Properties

        #region Event Handlers

        /// <summary>
        /// Handles "activate" messages from the Visual Studio environment.
        /// </summary>
        /// <devdoc>
        /// This method is called when Visual Studio wants to activate this page.  
        /// </devdoc>
        /// <remarks>If this handler sets e.Cancel to true, the activation will not occur.</remarks>
        protected override void OnActivate(CancelEventArgs e)
        {
            //int result = VsShellUtilities.ShowMessageBox(Site, Resources.MessageOnActivateEntered, null /*title*/, OLEMSGICON.OLEMSGICON_QUERY, OLEMSGBUTTON.OLEMSGBUTTON_OKCANCEL, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);

            //if (result == (int)VSConstants.MessageBoxResult.IDCANCEL)
            //{
            //    e.Cancel = true;
            //}

            base.OnActivate(e);
        }

        /// <summary>
        /// Handles "close" messages from the Visual Studio environment.
        /// </summary>
        /// <devdoc>
        /// This event is raised when the page is closed.
        /// </devdoc>
        protected override void OnClosed(EventArgs e)
        {
            // VsShellUtilities.ShowMessageBox(Site, Resources.MessageOnClosed, null /*title*/, OLEMSGICON.OLEMSGICON_INFO, OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }

        /// <summary>
        /// Handles "deactivate" messages from the Visual Studio environment.
        /// </summary>
        /// <devdoc>
        /// This method is called when VS wants to deactivate this
        /// page.  If this handler sets e.Cancel, the deactivation will not occur.
        /// </devdoc>
        /// <remarks>
        /// A "deactivate" message is sent when focus changes to a different page in
        /// the dialog.
        /// </remarks>
        protected override void OnDeactivate(CancelEventArgs e)
        {
            //int result = VsShellUtilities.ShowMessageBox(Site, Resources.MessageOnDeactivateEntered, null /*title*/, OLEMSGICON.OLEMSGICON_QUERY, OLEMSGBUTTON.OLEMSGBUTTON_OKCANCEL, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);

            //if (result == (int)VSConstants.MessageBoxResult.IDCANCEL)
            //{
            //    e.Cancel = true;
            //}
        }

        /// <summary>
        /// Handles "apply" messages from the Visual Studio environment.
        /// </summary>
        /// <devdoc>
        /// This method is called when VS wants to save the user's 
        /// changes (for example, when the user clicks OK in the dialog).
        /// </devdoc>
        protected override void OnApply(PageApplyEventArgs e)
        {
            //int result = VsShellUtilities.ShowMessageBox(Site, Resources.MessageOnApplyEntered, null /*title*/, OLEMSGICON.OLEMSGICON_QUERY, OLEMSGBUTTON.OLEMSGBUTTON_OKCANCEL, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);

            //if (result == (int)VSConstants.MessageBoxResult.IDCANCEL)
            //{
            //    e.ApplyBehavior = ApplyKind.Cancel;
            //}
            //else
            //{
            //    base.OnApply(e);
            //}

            //VsShellUtilities.ShowMessageBox(Site, Resources.MessageOnApply, null /*title*/, OLEMSGICON.OLEMSGICON_INFO, OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }
        #endregion Event Handlers
    }
}

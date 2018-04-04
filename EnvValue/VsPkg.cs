using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using EES.ComboBox.OptionsUI;
using EES.ComboBox.Services;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell.Settings;

[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "EES.ComboBox")]
namespace EES.ComboBox
{
    // This attribute tells the registration utility (regpkg.exe) that this class needs
    // to be registered as package.
    [PackageRegistration(UseManagedResourcesOnly = true)]

    // This attribute is used to register the informations needed to show the this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration("#100", "#102", "1.0", IconResourceID = 400)]

    // This attribute is needed to let the shell know that this package exposes VS commands (menus, buttons, etc...)
    [ProvideMenuResource("Menus.ctmenu", 1)]

    // This attribute registers a tool window exposed by this package.
    [Guid(GuidList.guidComboBoxPkgString)]
    [ProvideBindingPath]

    [ProvideOptionPageAttribute(typeof(OptionsPageGeneral), "Env Value", "General", 100, 101, true, new string[] { "Env Value dropdown items" })]
    [ProvideProfileAttribute(typeof(OptionsPageGeneral), "Env Value", "General Options", 100, 101, true, DescriptionResourceID = 100)]
    public sealed class ComboBoxPackage : Package
    {
        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public ComboBoxPackage()
        {
            //var re = new ManualAssemblyResolver(Assembly.LoadFrom("EES.Core.dll"));
        }

        /////////////////////////////////////////////////////////////////////////////
        // Overriden Package Implementation
        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initilaization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            // Add our command handlers for menu (commands must be declared in the .vsct file)
            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (null == mcs)
            {
                return;
            }

            CommandID menuMyDynamicComboCommandId = new CommandID(GuidList.guidComboBoxCmdSet, (int)PkgCmdIDList.cmdidMyDynamicCombo);
            OleMenuCommand menuMyDynamicComboCommand = new OleMenuCommand(OnMenuMyDynamicCombo, menuMyDynamicComboCommandId);
            mcs.AddCommand(menuMyDynamicComboCommand);

            CommandID menuMyDynamicComboGetListCommandId = new CommandID(GuidList.guidComboBoxCmdSet, (int)PkgCmdIDList.cmdidMyDynamicComboGetList);
            MenuCommand menuMyDynamicComboGetListCommand = new OleMenuCommand(OnMenuMyDynamicComboGetList, menuMyDynamicComboGetListCommandId);
            mcs.AddCommand(menuMyDynamicComboGetListCommand);
        }

        #endregion

        private void OnMenuMyDynamicCombo(object sender, EventArgs e)
        {
            if (null == e || e == EventArgs.Empty)
            {
                // We should never get here; EventArgs are required.
                throw new ArgumentException(Resources.EventArgsRequired);
            }

            if (!(e is OleMenuCmdEventArgs eventArgs))
            {
                // We should never get here; EventArgs are required.
                throw new ArgumentException(Resources.EventArgsRequired); // force an exception to be thrown
            }

            object input = eventArgs.InValue;
            IntPtr vOut = eventArgs.OutValue;


            DTE2 dte2 = (DTE2)GetGlobalService(typeof(SDTE));
            var a2 = dte2.Properties["Env Value", "General"];
            var envParamKey = a2.Item(nameof(OptionsPageGeneral.EnvParamKey)).Value as string;

            var envValueService = new EnvValueService(envParamKey);
            if (vOut != IntPtr.Zero && input != null)
            {
                throw new ArgumentException(Resources.BothInOutParamsIllegal); // force an exception to be thrown
            }

            if (vOut != IntPtr.Zero)
            {
                // when vOut is non-NULL, the IDE is requesting the current value for the combo

                var currentItem = envValueService.GetSelectedItem() ?? EnvValueService.Empty;

                Marshal.GetNativeVariantForObject(currentItem, vOut);
            }
            else if (input != null)
            {
                string inputString = input.ToString();

                envValueService.ChangeEnv(inputString, ShowMessage);
            }
            else
            {
                // We should never get here
                throw new ArgumentException(Resources.InOutParamCantBeNULL); // force an exception to be thrown
            }
        }

        private void OnMenuMyDynamicComboGetList(object sender, EventArgs e)
        {
            if (null == e || e == EventArgs.Empty)
            {
                // We should never get here; EventArgs are required.
                throw new ArgumentNullException(Resources.EventArgsRequired); // force an exception to be thrown
            }

            if (!(e is OleMenuCmdEventArgs eventArgs))
            {
                return;
            }

            object inParam = eventArgs.InValue;
            IntPtr vOut = eventArgs.OutValue;

            if (inParam != null)
            {
                throw new ArgumentException(Resources.InParamIllegal); // force an exception to be thrown
            }

            if (vOut != IntPtr.Zero)
            {
                //get solution file name
                string slnFile;
                DTE2 dte2 = (DTE2)GetGlobalService(typeof(SDTE));
                string[] items = new string[0];
                string searchPattern=string.Empty;
                if (dte2 != null)
                {
                    slnFile = dte2.Solution.FullName;
                    var a2 = dte2.Properties["Env Value", "General"];
                    items = a2.Item( nameof(OptionsPageGeneral.SavedEnvValues)).Value as string[];
                    searchPattern = a2.Item(nameof(OptionsPageGeneral.SearchPattern)).Value as string;
                }
                else
                {
                    DTE dte = (DTE)GetService(typeof(DTE));
                    slnFile = dte.Solution.FullName;
                }


                if (items == null)
                {
                    items = new string[0];
                }
                
                
                var worker = new PossibleValuesProvider(searchPattern);

                items = items.Concat(worker.GetItems(slnFile)).ToArray();
                

                Marshal.GetNativeVariantForObject(items, vOut);
            }
            else
            {
                throw new ArgumentException(Resources.OutParamRequired);
            }
        }

        public void ShowMessage(string title, string message)
        {
            IVsUIShell uiShell = (IVsUIShell)GetService(typeof(SVsUIShell));
            Guid clsid = Guid.Empty;
            int result = VSConstants.S_OK;
            int hr = uiShell.ShowMessageBox(0,
                                ref clsid,
                                title,
                                message,
                                null,
                                0,
                                OLEMSGBUTTON.OLEMSGBUTTON_OK,
                                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
                                OLEMSGICON.OLEMSGICON_INFO,
                                0,        // false = application modal; true would make it system modal
                                out result);
            ErrorHandler.ThrowOnFailure(hr);
        }
    }
}
/***************************************************************************
 
Copyright (c) Microsoft Corporation. All rights reserved.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using EES.ComboBox.Services;
using EnvDTE;
using EnvDTE80;

[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "EES.ComboBox")]
namespace EES.ComboBox
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>

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
    [ProvideBindingPath()]
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

            CommandID menuMyDynamicComboCommandID = new CommandID(GuidList.guidComboBoxCmdSet, (int)PkgCmdIDList.cmdidMyDynamicCombo);
            OleMenuCommand menuMyDynamicComboCommand = new OleMenuCommand(new EventHandler(OnMenuMyDynamicCombo), menuMyDynamicComboCommandID);
            mcs.AddCommand(menuMyDynamicComboCommand);

            CommandID menuMyDynamicComboGetListCommandId = new CommandID(GuidList.guidComboBoxCmdSet, (int)PkgCmdIDList.cmdidMyDynamicComboGetList);
            MenuCommand menuMyDynamicComboGetListCommand = new OleMenuCommand(new EventHandler(OnMenuMyDynamicComboGetList), menuMyDynamicComboGetListCommandId);
            mcs.AddCommand(menuMyDynamicComboGetListCommand);
        }

        #endregion

        #region Combo Box Commands

        private readonly string[] _dropDownComboChoices = new string[0];
        private string[] DropDownComboChoices => InitConfigurations();

        private string _currentDropDownComboChoice;

        private string CurrentDropDownComboChoice
        {
            get => GetCurrentDropDownComboChoice();
            set => _currentDropDownComboChoice = value;
        }



        private string GetCurrentDropDownComboChoice()
        {
            if (_currentDropDownComboChoice != null)
            {
                return _currentDropDownComboChoice;
            }

            InitConfigurations();
            var envValueService = new EnvValueService();

            _currentDropDownComboChoice = envValueService.GetSelectedItem() ?? _dropDownComboChoices.FirstOrDefault() ?? "No config";
            return _currentDropDownComboChoice;
        }

        private string[] InitConfigurations()
        {
            if (_dropDownComboChoices?.Length > 1)
            {
                return _dropDownComboChoices;
            }

            string slnFile;
            DTE2 dte2 = (DTE2)GetGlobalService(typeof(SDTE));
            if (dte2 != null)
            {
                slnFile = dte2.Solution.FullName;
            }
            else
            {
                DTE dte = (DTE)GetService(typeof(DTE));
                slnFile = dte.Solution.FullName;
            }


            if (string.IsNullOrWhiteSpace(slnFile))
            {
                return _dropDownComboChoices;
            }
            var worker = new PossibleValuesService();
            return worker.GetItems(slnFile);
        }

        private void OnMenuMyDropDownCombo(object sender, EventArgs e)
        {
            OleMenuCmdEventArgs eventArgs = e as OleMenuCmdEventArgs;

            if (eventArgs != null)
            {
                string newChoice = eventArgs.InValue as string;
                IntPtr vOut = eventArgs.OutValue;

                if (vOut != IntPtr.Zero)
                {
                    // when vOut is non-NULL, the IDE is requesting the current value for the combo
                    Marshal.GetNativeVariantForObject(CurrentDropDownComboChoice, vOut);
                }

                else if (newChoice != null)
                {
                    // new value was selected or typed in
                    // see if it is one of our items
                    bool validInput = false;
                    int indexInput;
                    for (indexInput = 0; indexInput < DropDownComboChoices.Length; indexInput++)
                    {
                        if (string.Compare(DropDownComboChoices[indexInput], newChoice, StringComparison.CurrentCultureIgnoreCase) == 0)
                        {
                            validInput = true;
                            break;
                        }
                    }

                    if (validInput)
                    {
                        CurrentDropDownComboChoice = DropDownComboChoices[indexInput];

                        new EnvValueService().ChangeEnv(CurrentDropDownComboChoice, ShowMessage);
                    }
                    else
                    {
                        throw (new ArgumentException(Resources.ParamNotValidStringInList)); // force an exception to be thrown
                    }
                }
            }
            else
            {
                // We should never get here; EventArgs are required.
                throw (new ArgumentException(Resources.EventArgsRequired)); // force an exception to be thrown
            }
        }

        #endregion
        // Helper method to show a message box using the SVsUiShell/IVsUiShell service

        private void OnMenuMyDropDownComboGetList(object sender, EventArgs e)
        {
            OleMenuCmdEventArgs eventArgs = e as OleMenuCmdEventArgs;

            if (eventArgs != null)
            {
                object inParam = eventArgs.InValue;
                IntPtr vOut = eventArgs.OutValue;

                if (inParam != null)
                {
                    throw (new ArgumentException(Resources.InParamIllegal)); // force an exception to be thrown
                }
                else if (vOut != IntPtr.Zero)
                {
                    Marshal.GetNativeVariantForObject(DropDownComboChoices, vOut);
                }
                else
                {
                    throw (new ArgumentException(Resources.OutParamRequired)); // force an exception to be thrown
                }
            }

        }

        private readonly string[] _indexComboChoices = { Resources.Lions, Resources.Tigers, Resources.Bears };
        private int _currentIndexComboChoice;

        private void OnMenuMyIndexCombo(object sender, EventArgs e)
        {
            if ((null == e) || (e == EventArgs.Empty))
            {
                // We should never get here; EventArgs are required.
                throw (new ArgumentException(Resources.EventArgsRequired)); // force an exception to be thrown
            }

            OleMenuCmdEventArgs eventArgs = e as OleMenuCmdEventArgs;

            if (eventArgs != null)
            {
                object input = eventArgs.InValue;
                IntPtr vOut = eventArgs.OutValue;

                if (vOut != IntPtr.Zero && input != null)
                {
                    throw (new ArgumentException(Resources.BothInOutParamsIllegal)); // force an exception to be thrown
                }
                if (vOut != IntPtr.Zero)
                {
                    // when vOut is non-NULL, the IDE is requesting the current value for the combo
                    Marshal.GetNativeVariantForObject(_indexComboChoices[_currentIndexComboChoice], vOut);
                }

                else if (input != null)
                {
                    int newChoice;
                    if (!int.TryParse(input.ToString(), out newChoice))
                    {
                        // user typed a string argument in command window.
                        for (int i = 0; i < _indexComboChoices.Length; i++)
                        {
                            if (string.Compare(_indexComboChoices[i], input.ToString(), StringComparison.CurrentCultureIgnoreCase) == 0)
                            {
                                newChoice = i;
                                break;
                            }
                        }
                    }

                    // new value was selected or typed in
                    if (newChoice != -1)
                    {
                        _currentIndexComboChoice = newChoice;
                        ShowMessage(Resources.MyIndexCombo, _currentIndexComboChoice.ToString(CultureInfo.CurrentCulture));
                    }
                    else
                    {
                        throw (new ArgumentException(Resources.ParamMustBeValidIndexOrStringInList)); // force an exception to be thrown
                    }
                }
                else
                {
                    // We should never get here; EventArgs are required.
                    throw (new ArgumentException(Resources.EventArgsRequired)); // force an exception to be thrown
                }
            }
            else
            {
                // We should never get here; EventArgs are required.
                throw (new ArgumentException(Resources.EventArgsRequired)); // force an exception to be thrown
            }
        }

        private void OnMenuMyIndexComboGetList(object sender, EventArgs e)
        {
            if (e == EventArgs.Empty)
            {
                // We should never get here; EventArgs are required.
                throw (new ArgumentException(Resources.EventArgsRequired)); // force an exception to be thrown
            }

            OleMenuCmdEventArgs eventArgs = e as OleMenuCmdEventArgs;

            if (eventArgs != null)
            {
                object inParam = eventArgs.InValue;
                IntPtr vOut = eventArgs.OutValue;

                if (inParam != null)
                {
                    throw (new ArgumentException(Resources.InParamIllegal)); // force an exception to be thrown
                }
                else if (vOut != IntPtr.Zero)
                {
                    Marshal.GetNativeVariantForObject(_indexComboChoices, vOut);
                }
                else
                {
                    throw (new ArgumentException(Resources.OutParamRequired)); // force an exception to be thrown
                }
            }
        }

        private string _currentMruComboChoice;

        private void OnMenuMyMruCombo(object sender, EventArgs e)
        {
            if (e == EventArgs.Empty)
            {
                // We should never get here; EventArgs are required.
                throw (new ArgumentException(Resources.EventArgsRequired)); // force an exception to be thrown
            }

            OleMenuCmdEventArgs eventArgs = e as OleMenuCmdEventArgs;

            if (eventArgs != null)
            {
                object input = eventArgs.InValue;
                IntPtr vOut = eventArgs.OutValue;

                if (vOut != IntPtr.Zero && input != null)
                {
                    throw (new ArgumentException(Resources.BothInOutParamsIllegal)); // force an exception to be thrown
                }
                else if (vOut != IntPtr.Zero)
                {
                    // when vOut is non-NULL, the IDE is requesting the current value for the combo
                    Marshal.GetNativeVariantForObject(_currentMruComboChoice, vOut);
                }

                else if (input != null)
                {
                    string newChoice = input.ToString();

                    // new value was selected or typed in
                    if (!string.IsNullOrEmpty(newChoice))
                    {
                        _currentMruComboChoice = newChoice;
                        ShowMessage(Resources.MyMRUCombo, _currentMruComboChoice);
                    }
                    else
                    {
                        // We should never get here
                        throw (new ArgumentException(Resources.EmptyStringIllegal)); // force an exception to be thrown
                    }
                }
                else
                {
                    throw (new ArgumentException(Resources.BothInOutParamsIllegal)); // force an exception to be thrown
                }
            }
            else
            {
                // We should never get here; EventArgs are required.
                throw (new ArgumentException(Resources.EventArgsRequired)); // force an exception to be thrown
            }
        }

        private void OnMenuMyDynamicCombo(object sender, EventArgs e)
        {
            if ((null == e) || (e == EventArgs.Empty))
            {
                // We should never get here; EventArgs are required.
                throw (new ArgumentException(Resources.EventArgsRequired)); // force an exception to be thrown
            }

            OleMenuCmdEventArgs eventArgs = e as OleMenuCmdEventArgs;

            if (eventArgs != null)
            {
                object input = eventArgs.InValue;
                IntPtr vOut = eventArgs.OutValue;

                var envValueService = new EnvValueService();
                if (vOut != IntPtr.Zero && input != null)
                {
                    throw (new ArgumentException(Resources.BothInOutParamsIllegal)); // force an exception to be thrown
                }
                else if (vOut != IntPtr.Zero)
                {
                    // when vOut is non-NULL, the IDE is requesting the current value for the combo
                    
                    var currentItem = envValueService.GetSelectedItem() ?? "<empty>";

                    Marshal.GetNativeVariantForObject(currentItem, vOut);
                }
                else if (input != null)
                {
                    // new zoom value was selected or typed in
                    
                    string inputString = input.ToString();

                    new EnvValueService().ChangeEnv(inputString,ShowMessage);
                    
                }
                else
                {
                    // We should never get here
                    throw (new ArgumentException(Resources.InOutParamCantBeNULL)); // force an exception to be thrown
                }
            }
            else
            {
                // We should never get here; EventArgs are required.
                throw (new ArgumentException(Resources.EventArgsRequired)); // force an exception to be thrown
            }
        }

        private void OnMenuMyDynamicComboGetList(object sender, EventArgs e)
        {
            if ((null == e) || (e == EventArgs.Empty))
            {
                // We should never get here; EventArgs are required.
                throw (new ArgumentNullException(Resources.EventArgsRequired)); // force an exception to be thrown
            }

            OleMenuCmdEventArgs eventArgs = e as OleMenuCmdEventArgs;

            if (eventArgs != null)
            {
                object inParam = eventArgs.InValue;
                IntPtr vOut = eventArgs.OutValue;

                if (inParam != null)
                {
                    throw (new ArgumentException(Resources.InParamIllegal)); // force an exception to be thrown
                }
                else if (vOut != IntPtr.Zero)
                {
                    // initialize the zoom value array if needed
                    string slnFile;
                    DTE2 dte2 = (DTE2)GetGlobalService(typeof(SDTE));
                    if (dte2 != null)
                    {
                        slnFile = dte2.Solution.FullName;
                    }
                    else
                    {
                        DTE dte = (DTE)GetService(typeof(DTE));
                        slnFile = dte.Solution.FullName;
                    }

                    var worker = new PossibleValuesService();
                    var items = worker.GetItems(slnFile);

                    Marshal.GetNativeVariantForObject(items, vOut);
                }
                else
                {
                    throw (new ArgumentException(Resources.OutParamRequired)); // force an exception to be thrown
                }
            }
        }


        // Helper method to show a message box using the SVsUiShell/IVsUiShell service
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
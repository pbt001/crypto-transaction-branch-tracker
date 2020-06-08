using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace CryptoBranchTracker.Objects.Classes
{
    public class Settings
    {
        public bool DarkMode { get; set; } = false;

        public Color ColourScheme { get; set; } = Color.FromArgb(255, 103, 58, 183);

        /// <summary>
        /// Set the default registry values for the user's colour scheme settings
        /// </summary>
        /// <param name="settingsKey"></param>
        private static void SetDefaultLocals(RegistryKey settingsKey)
        {
            try
            {
                settingsKey.SetValue(Strings.SettingsNames.SCHEME_A, 255, RegistryValueKind.DWord);
                settingsKey.SetValue(Strings.SettingsNames.SCHEME_R, 103, RegistryValueKind.DWord);
                settingsKey.SetValue(Strings.SettingsNames.SCHEME_G, 58, RegistryValueKind.DWord);
                settingsKey.SetValue(Strings.SettingsNames.SCHEME_B, 183, RegistryValueKind.DWord);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred setting default locals: {ex}");
            }
        }

        public void Save()
        {
            try
            {
                Globals.FixRegistry();

                RegistryView platformView = Environment.Is64BitOperatingSystem
                    ? RegistryView.Registry64
                    : RegistryView.Registry32;

                using (RegistryKey registryBase = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, platformView))
                {
                    if (registryBase != null)
                    {
                        using (RegistryKey applicationKey = registryBase.CreateSubKey(Strings.RegistryLocations.APPLICATION_LOCATION))
                        {
                            using (RegistryKey settingsValues = applicationKey.CreateSubKey(Strings.RegistryLocations.SETTINGS_VALUES))
                            {
                                settingsValues.SetValue(Strings.SettingsNames.DARK_MODE, this.DarkMode, RegistryValueKind.DWord);

                                //Colour scheme
                                settingsValues.SetValue(Strings.SettingsNames.SCHEME_A, this.ColourScheme.A, RegistryValueKind.DWord);
                                settingsValues.SetValue(Strings.SettingsNames.SCHEME_R, this.ColourScheme.R, RegistryValueKind.DWord);
                                settingsValues.SetValue(Strings.SettingsNames.SCHEME_G, this.ColourScheme.G, RegistryValueKind.DWord);
                                settingsValues.SetValue(Strings.SettingsNames.SCHEME_B, this.ColourScheme.B, RegistryValueKind.DWord);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred saving settings: {ex}");
            }
        }

        public static Settings GetSettings()
        {
            Settings settings = new Settings();

            try
            {
                Globals.FixRegistry();

                RegistryView platformView = Environment.Is64BitOperatingSystem
                    ? RegistryView.Registry64
                    : RegistryView.Registry32;

                using (RegistryKey registryBase = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, platformView))
                {
                    if (registryBase != null)
                    {
                        using (RegistryKey applicationKey = registryBase.CreateSubKey(Strings.RegistryLocations.APPLICATION_LOCATION))
                        {
                            using (RegistryKey settingsValues = applicationKey.CreateSubKey(Strings.RegistryLocations.SETTINGS_VALUES))
                            {
                                object darkMode = settingsValues.GetValue(Strings.SettingsNames.DARK_MODE);

                                if (darkMode != null)
                                    settings.DarkMode = Convert.ToBoolean((int)darkMode);
                                else
                                    settingsValues.SetValue(Strings.SettingsNames.DARK_MODE, false, RegistryValueKind.DWord);

                                object schemeA = settingsValues.GetValue(Strings.SettingsNames.SCHEME_A);

                                if (schemeA != null)
                                {
                                    Color clrScheme = new Color
                                    {
                                        A = Convert.ToByte((int)schemeA)
                                    };

                                    object schemeR = settingsValues.GetValue(Strings.SettingsNames.SCHEME_R);

                                    if (schemeR != null)
                                    {
                                        clrScheme.R = Convert.ToByte((int)schemeR);
                                        object schemeG = settingsValues.GetValue(Strings.SettingsNames.SCHEME_G);

                                        if (schemeG != null)
                                        {
                                            clrScheme.G = Convert.ToByte((int)schemeG);
                                            object schemeB = settingsValues.GetValue(Strings.SettingsNames.SCHEME_B);

                                            if (schemeB != null)
                                            {
                                                clrScheme.B = Convert.ToByte((int)schemeB);
                                                settings.ColourScheme = clrScheme;
                                            }
                                            else
                                                Settings.SetDefaultLocals(settingsValues);
                                        }
                                        else
                                            Settings.SetDefaultLocals(settingsValues);
                                    }
                                    else
                                        Settings.SetDefaultLocals(settingsValues);
                                }
                                else
                                    Settings.SetDefaultLocals(settingsValues);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred getting settings: {ex}");
            }

            return settings;
        }
    }
}

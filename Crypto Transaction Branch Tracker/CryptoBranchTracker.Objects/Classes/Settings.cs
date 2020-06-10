using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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

        public void Save()
        {
            try
            {
                Globals.FixJSONFile();

                JObject objSettings = Globals.GetRawSettingsData();

                if (objSettings != null)
                {
                    JEnumerable<JProperty> enProperties = objSettings.Children<JProperty>();

                    //Dark Mode
                    JProperty propDarkMode = enProperties.
                        Where(x => x.Name == Strings.SettingsNames.DARK_MODE).FirstOrDefault();

                    if (propDarkMode != null)
                        propDarkMode.Value = this.DarkMode;

                    //Scheme A
                    JProperty propSchemeA = enProperties.
                        Where(x => x.Name == Strings.SettingsNames.SCHEME_A).FirstOrDefault();

                    if (propSchemeA != null)
                        propSchemeA.Value = Convert.ToInt32(this.ColourScheme.A);

                    //Scheme R
                    JProperty propSchemeR = enProperties.
                        Where(x => x.Name == Strings.SettingsNames.SCHEME_R).FirstOrDefault();

                    if (propSchemeR != null)
                        propSchemeR.Value = Convert.ToInt32(this.ColourScheme.R);

                    //Scheme G
                    JProperty propSchemeG = enProperties.
                        Where(x => x.Name == Strings.SettingsNames.SCHEME_G).FirstOrDefault();

                    if (propSchemeG != null)
                        propSchemeG.Value = Convert.ToInt32(this.ColourScheme.G);

                    //Scheme B
                    JProperty propSchemeB = enProperties.
                        Where(x => x.Name == Strings.SettingsNames.SCHEME_B).FirstOrDefault();

                    if (propSchemeB != null)
                        propSchemeB.Value = Convert.ToInt32(this.ColourScheme.B);

                    Globals.UpdateDataFile(objSettings.Root.ToString());
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
                Globals.FixJSONFile();

                JObject objSettings = Globals.GetRawSettingsData();

                if (objSettings != null)
                {
                    JEnumerable<JProperty> enProperties = objSettings.Children<JProperty>();

                    //Dark Mode
                    JProperty propDarkMode = enProperties.
                        Where(x => x.Name == Strings.SettingsNames.DARK_MODE).FirstOrDefault();

                    settings.DarkMode = propDarkMode == null
                        ? false
                        : Convert.ToBoolean(propDarkMode.Value);

                    //Scheme A
                    JProperty propSchemeA = enProperties.
                        Where(x => x.Name == Strings.SettingsNames.SCHEME_A).FirstOrDefault();

                    int schemeA = propSchemeA == null
                        ? 255
                        : Convert.ToInt32(propSchemeA.Value);

                    //Scheme R
                    JProperty propSchemeR = enProperties.
                        Where(x => x.Name == Strings.SettingsNames.SCHEME_R).FirstOrDefault();

                    int schemeR = propSchemeR == null
                        ? 103
                        : Convert.ToInt32(propSchemeR.Value);

                    //Scheme G
                    JProperty propSchemeG = enProperties.
                        Where(x => x.Name == Strings.SettingsNames.SCHEME_G).FirstOrDefault();

                    int schemeG = propSchemeG == null
                        ? 58
                        : Convert.ToInt32(propSchemeG.Value);

                    //Scheme B
                    JProperty propSchemeB = enProperties.
                        Where(x => x.Name == Strings.SettingsNames.SCHEME_B).FirstOrDefault();

                    int schemeB = propSchemeB == null
                        ? 183
                        : Convert.ToInt32(propSchemeB.Value);

                    settings.ColourScheme = Color.FromArgb(Convert.ToByte(schemeA), Convert.ToByte(schemeR), Convert.ToByte(schemeG), Convert.ToByte(schemeB));
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

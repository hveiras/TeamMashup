using System;
using System.Configuration;
using TeamMashup.Core.Tracking;

namespace TeamMashup.Core
{
    public static class ConfigurationHelper
    {
        public static bool GetBooleanConfigurationProperty(string keyName, bool defaultValue)
        {
            bool finalValue = defaultValue;
            try
            {
                var value = ConfigurationManager.AppSettings.Get(keyName);

                if (!string.IsNullOrWhiteSpace(value))
                {
                    if (!Boolean.TryParse(value, out finalValue))
                        LogHelper.Error(string.Format("Couldn't parse the {0} key from the app.config file. Using default value: '{1}'.", keyName, defaultValue));
                }
                else
                {
                    LogHelper.Info(string.Format("{0} key is not defined on the app.config file. Using default value: '{1}'.", keyName, defaultValue));
                }
            }
            catch (Exception e)
            {
                LogHelper.Error(e);
            }
            return finalValue;
        }

        public static int GetIntConfigurationProperty(string keyName, int defaultValue)
        {
            int finalValue = defaultValue;
            try
            {
                var value = ConfigurationManager.AppSettings.Get(keyName);

                if (!string.IsNullOrWhiteSpace(value))
                {
                    if (!int.TryParse(value, out finalValue))
                        LogHelper.Error(string.Format("Couldn't parse the {0} key from the app.config file. Using default value: '{1}'.", keyName, defaultValue));
                }
                else
                {
                    LogHelper.Info(string.Format("{0} key is not defined on the app.config file. Using default value: '{1}'.", keyName, defaultValue));
                }
            }
            catch (Exception e)
            {
                LogHelper.Error(e);
            }
            return finalValue;
        }
    }
}

using System;
using Microsoft.Win32;

namespace MeshAssistant
{
    public static class Settings
    {
        public static void SetRegValue(string name, string value)
        {
            try { Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\Open Source\MeshCentral Assistant", name, value); } catch (Exception) { }
        }
        public static void SetRegValue(string name, bool value)
        {
            try { Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\Open Source\MeshCentral Assistant", name, value.ToString()); } catch (Exception) { }
        }
        /// <summary>
        /// This function querys the registry. If the key is found it returns the value as a string
        /// </summary>
        /// <param name="name">Keyname</param>
        /// <param name="value">Return on fail</param>
        /// <returns></returns>
        public static string GetRegValue(string name, string value)
        {
            try { return Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Open Source\MeshCentral Assistant", name, value).ToString(); } catch (Exception) { return value; }
        }
        /// <summary>
        /// This function querys the registry. If the key is found it returns the value as a boolean
        /// </summary>
        /// <param name="name">Keyname</param>
        /// <param name="value">Return on fail</param>
        /// <returns></returns>
        public static bool GetRegValue(string name, bool value)
        {
            try { return bool.Parse(GetRegValue(name, value.ToString())); } catch (Exception) { return value; }
        }
    }
}

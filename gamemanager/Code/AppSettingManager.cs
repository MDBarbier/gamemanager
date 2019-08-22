using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace gamemanager.Code
{
    public sealed class AppSettingManager
    {
        public string Path { get; }
        public Dictionary<string, string> AppSettings { get; set; } = new Dictionary<string, string>();

        public AppSettingManager()
        {            
            string basePath = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            if (basePath.Contains("/"))
            {
                Path = basePath + @"/wwwroot/Config/secureappsettings.json";
            }
            else
            {
                Path = basePath + @"\wwwroot\Config\secureappsettings.json";
            }
            

            LoadSettings();
        }


        /// <summary>
        /// Loads the app setting from file
        /// </summary>
        public void LoadSettings()
        {
            JObject jsonObject;

            if (!File.Exists(Path))
            {
                throw new FileNotFoundException($"AppSettings File not found at path {Path}!");
            }

            using (StreamReader r = new StreamReader(Path))
            {
                string json = r.ReadToEnd();
                jsonObject = JsonConvert.DeserializeObject<JObject>(json);
            }
            JObject gameManagerSettings = JObject.Parse(jsonObject["gamemanager"].ToString());
            AppSettings = gameManagerSettings.ToObject<Dictionary<string, string>>();
        }

        /// <summary>
        /// Returns the value of the entry in AppSettings that matches the supplied key, or throws exception
        /// </summary>
        /// <param name="keyname"></param>
        /// <returns>string value representing the app setting</returns>
        public string GetSetting(string keyname)
        {
            return AppSettings.Where(a => a.Key == keyname).Single().Value;
        }

        //Field
        public static AppSettingManager Instance { get { return Nested.instance; } }

        //Nested class
        private class Nested
        {


            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {

            }

            internal static readonly AppSettingManager instance = new AppSettingManager();
        }
    }
}

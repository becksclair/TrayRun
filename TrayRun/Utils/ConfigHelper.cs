using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;
using System;

namespace TrayRun.Utils
{
    public static class ConfigHelper
    {
        public static void CheckSettingsFolderExists()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            Directory.CreateDirectory(appData + "\\HSP\\TrayRun");
        }

        public static string GetSettingsPath()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            return appData + "\\HSP\\TrayRun\\settings.json";
        }
    }

    public static class ConfigHelper<T>
    {
        public static T GetConfig()
        {
            // Read file
            string json;
            using (var configFileStream = new FileStream(ConfigHelper.GetSettingsPath(), FileMode.OpenOrCreate, FileAccess.Read))
            {
                using (var reader = new StreamReader(configFileStream))
                {
                    json = reader.ReadToEnd();
                    reader.Close();
                    configFileStream.Close();
                }
            }

            // Parse config
            var contractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver {
                NamingStrategy = new Newtonsoft.Json.Serialization.SnakeCaseNamingStrategy()
            };
            T config = JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings
            {
                ContractResolver = contractResolver,
                Formatting = Formatting.Indented
            });
            return config;
        }

        public static void SaveConfig(T config)
        {
            // Serialize config
            var contractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver {
                NamingStrategy = new Newtonsoft.Json.Serialization.SnakeCaseNamingStrategy()
            };
            string json = JsonConvert.SerializeObject(config, new JsonSerializerSettings
            {
                ContractResolver = contractResolver,
                Formatting = Formatting.Indented
            });

            // Remove file if exists
            var settingsFile = ConfigHelper.GetSettingsPath();
            if (File.Exists(settingsFile))
            {
                File.Delete(settingsFile);
            }

            // Write config file
            using (var configFileStream = new FileStream(settingsFile, FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (var writer = new StreamWriter(configFileStream))
                {
                    writer.Write(json);
                    writer.Flush();
                    writer.Close();
                    configFileStream.Close();
                }
            }
        }
    }
}

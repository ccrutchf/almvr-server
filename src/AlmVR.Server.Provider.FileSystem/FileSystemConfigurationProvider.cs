using AlmVR.Server.Core.Providers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace AlmVR.Server.Provider.FileSystem
{
    public class FileSystemConfigurationProvider : IConfigurationProvider
    {
        private DirectoryInfo configurationDirectory;
        private Dictionary<Type, object> configurationMap;

        public FileSystemConfigurationProvider()
        {
            configurationMap = new Dictionary<Type, object>();

            configurationDirectory = new DirectoryInfo(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Configuration"));

            // Running in Docker
            if (File.Exists("/.dockerenv") && Directory.Exists("/data"))
            {
                configurationDirectory = new DirectoryInfo("/data");
            }

            if (!configurationDirectory.Exists)
            {
                configurationDirectory.Create();
            }
        }

        public async Task<T> GetConfigurationAsync<T>()
            where T : class
        {
            var configFile = new FileInfo(Path.Combine(configurationDirectory.FullName, $"{typeof(T).FullName}.json"));

            if (configurationMap.ContainsKey(typeof(T)))
                return (T)configurationMap[typeof(T)];

            T config = default(T);
            if (configFile.Exists)
            {
                string json = null;
                using (var reader = configFile.OpenText())
                {
                    json = await reader.ReadToEndAsync();
                }

                config = JsonConvert.DeserializeObject<T>(json);
            }
            else
                config = Activator.CreateInstance<T>();

            configurationMap.Add(typeof(T), config);
            return config;
        }

        public Task SetConfigurationAsync<T>()
            where T : class
        {
            throw new NotImplementedException();
        }
    }
}

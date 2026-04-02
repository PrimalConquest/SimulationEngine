using Newtonsoft.Json;
using SimulationEngine.Source.Enums.Logging;
using SimulationEngine.Source.Helpers;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SimulationEngine.Source.Systems
{
    internal static class ResourceSystem
    {
        static string _basePath = "SimulationEngine.";
        static string _resourcePath = _basePath + "Resources.";

        public static string? Get(string resource)
        {
            string file = _resourcePath + resource;
            Assembly assembly = typeof(ResourceSystem).Assembly;
            using Stream stream = assembly.GetManifestResourceStream(file);
            
            if (stream == null)
            {
                LogSystem.Log(ELogCategory.Debug, ELogLevel.Warning, $"ResourceSystem:Get There is no resource: {file}");
                return null;
            }

            using StreamReader sr = new StreamReader(stream);
                
            string content = sr.ReadToEnd();

            return content; 
        }
    }
}

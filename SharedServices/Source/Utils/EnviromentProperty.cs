using System;
using System.Collections.Generic;
using System.Text;

namespace SharedServices.Source.Utils
{
    public class EnviromentProperty
    {
        public static string Get(string EnvVar, bool required = true, string defaultValue = "")
        {
            return Environment.GetEnvironmentVariable(EnvVar) ?? ((required) ? throw new Exception($"Missing enviroment variable [{EnvVar}]") : defaultValue);
        }
    }
}

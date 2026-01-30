using Microsoft.Extensions.Configuration;

namespace Neo.Configuration
{
    /// <summary>
    /// Configuration extension methods
    /// </summary>
    public static class ConfigurationExtensions
    {
        public static string GetEnvConfigPath(this string configFileName)
        {
            var env = Environment.GetEnvironmentVariable("NEO_NETWORK");
            return string.IsNullOrWhiteSpace(env) 
                ? $"{configFileName}.json" 
                : $"{configFileName}.{env}.json";
        }
    }
}

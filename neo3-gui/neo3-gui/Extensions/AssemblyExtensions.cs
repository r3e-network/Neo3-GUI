using System.Reflection;

namespace Neo.Extensions
{
    /// <summary>
    /// Assembly extension methods
    /// </summary>
    public static class AssemblyExtensions
    {
        public static string GetVersion(this Assembly assembly)
        {
            var attr = assembly.CustomAttributes
                .FirstOrDefault(p => p.AttributeType == 
                    typeof(AssemblyInformationalVersionAttribute));
            
            if (attr == null) 
                return assembly.GetName().Version.ToString(3);
            
            return (string)attr.ConstructorArguments[0].Value;
        }
    }
}

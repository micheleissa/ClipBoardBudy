using System.IO;
using System.Reflection;

namespace CopyBud.Mutex
{
    //Taken from https://www.codeproject.com/Articles/32908/C-Single-Instance-App-With-the-Ability-To-Restore
    public static class ProgramInfo
    {
        public static string AssemblyGuid
        {
            get
            {
                var attributes = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(System.Runtime.InteropServices.GuidAttribute), false);
                return attributes.Length == 0 ? string.Empty : ((System.Runtime.InteropServices.GuidAttribute)attributes[0]).Value;
            }
        }
        public static string AssemblyTitle
        {
            get
            {
                var attributes = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length <= 0)
                    return Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().CodeBase);
                var titleAttribute = (AssemblyTitleAttribute)attributes[0];
                return titleAttribute.Title != "" ? titleAttribute.Title : Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().CodeBase);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DcsBiosSharp.Definition
{
    public static class AssemblyExtensionMethods
    {
        public static Stream GetEmbeddedFile(this Assembly assembly, string fileName)
        {
            string resourceName = assembly.GetManifestResourceNames().FirstOrDefault(str => str.EndsWith(fileName));

            return assembly.GetManifestResourceStream(resourceName);
        }
    }
}

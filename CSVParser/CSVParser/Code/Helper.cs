using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CSVParser.Code
{
    internal class Helper
    {
        /// <summary>
        /// Returns path to assemblty.
        /// </summary>
        /// <returns></returns>
        internal static string GetAssemblyPath()
        {
            var assembly = Assembly.GetExecutingAssembly();
            return new DirectoryInfo(Path.GetDirectoryName(assembly.Location)).FullName;
        }
    }
}

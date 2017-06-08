using CSVParser.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//TODO: add console encoding

namespace CSVParser
{
    class Program
    {
        static void Main(string[] args)
        {
            //validate input parameters
            if (args.Length == 0)
            {
                ReportError("Missing input parameters.");
                return;
            }
            if (args.Length != 1 && args.Length != 3)
            {
                ReportError("Invalid number of input parameters.");
                return;
            }

            //read input parameters
            string path = args[0];
            string filterColumnName = null;
            string filterColumnValue = null;
            if (args.Length == 3)
            {
                filterColumnName = args[1];
                filterColumnValue = args[2];
            }

            //read file
            CSVFileParser parser = new CSVFileParser(path);
            CommaSeparatedValues csvData = parser.Read();

            //output to console depending on input parameters
            if (filterColumnValue == null)
                Console.Write(csvData.ToString());
            else
                Console.Write(csvData.ToString(filterColumnName, filterColumnValue));

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        private static void ReportError(string message)
        {
            Console.WriteLine(message);
            Console.WriteLine("Possible usage:");
            Console.WriteLine("CSVParser.exe pathToCsvFile");
            Console.WriteLine("CSVParser.exe pathToCsvFile [filterColumnName filterValue]");
        }
    }
}

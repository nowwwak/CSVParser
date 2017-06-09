using CSVParser.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//TODO: add console encoding

/*
 * Supported encodings: ASCII, UTF-8, UNICODE
 * For all encodings \r\n is used as line separator
 * 
 */
namespace CSVParser
{
    class Program
    {
        private static string pathToCsvFileParamName = "pathToCsvFile";
        private static string filterColumnIndexParamName = "filterColumnIndex";
        private static string filterValueParamName = "filterValue";

        static void Main(string[] args)
        {
            //validate input parameters
            if (!ValidateParamsLength(args))
                return;

            //read input parameters
            string path = args[0];
            int filterColumnIndex = 0;
            string filterColumnValue = null;
            if (args.Length == 3)
            {
                if(!Int32.TryParse(args[1], out filterColumnIndex) || filterColumnIndex<1)
                {
                    ReportError($"Param {filterColumnIndexParamName} must be an integer, starting from 1.");
                    return;
                }
                    
                filterColumnValue = args[2];
            }

            try
            {
                //read file
                CSVFileParser parser = new CSVFileParser(path);
                CommaSeparatedValues csvData = parser.Read();

                //output to console depending on input parameters
                if (filterColumnValue == null)
                    Console.Write(csvData.ToString());
                else
                {
                    //validate filterColumnIndex 
                    if (filterColumnIndex < 1 || filterColumnIndex > csvData.ColumnsCount)
                        throw new IndexOutOfRangeException(string.Format("{0} is out of range.", nameof(filterColumnIndex)));
                    int filterColumnIndex0Based = filterColumnIndex - 1;
                    Console.Write(csvData.ToString(filterColumnIndex0Based, filterColumnValue));
                }

                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error during processing. Exception message: {0} ", ex.Message));
            }
        }

        private static void ReportError(string message)
        {
            Console.WriteLine(message);
            Console.WriteLine("Possible usage:");
            Console.WriteLine($"CSVParser.exe {pathToCsvFileParamName}");
            Console.WriteLine($"CSVParser.exe {pathToCsvFileParamName} {filterColumnIndexParamName} {filterValueParamName}");
        }

        private static bool ValidateParamsLength(string[] args)
        {
            if (args.Length == 0)
            {
                ReportError("Missing input parameters.");
                return false;
            }
            if (args.Length != 1 && args.Length != 3)
            {
                ReportError("Invalid number of input parameters.");
                return false;
            }

            return true;
        }
    }
}

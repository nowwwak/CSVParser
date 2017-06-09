using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSVParser.Code
{
    public class CSVFormatException : Exception
    {
        public CSVFormatException(string message, long line, long column): 
            base(string.Format("{0}{1}", $"Error in line {line} col {column}: ", message))
        {

        }
    }
}

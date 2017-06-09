using CSVParser.Code;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSVParser.Test
{
    [TestFixture]
    class CommaSeparatedValuesTest
    {
        static object[][] testCases = new[]
        {
            new object[]
            {
                new string[][]
                {
                   new string[] { "header1", "header2","header3" },
                   new string[] { " r11  ", "\"r 12\"", "\"r, 13\"" },
                   new string[] { "\"r21 \"", "r22", "\"r\"\", 23\"" },
                   new string[] { "\"r31 \"", "", "\"r\"\", 33\"" },
                   new string[] { "r41","\"r \r\n\"\", 42\"", "" },
                   new string[] { "", "", "" },
                   new string[] { "\\r", "\\r\\n", "\\n" },
                },
                1,
                "\"r \r\n\"\", 42\"",
                new string[] { "r41","\"r \r\n\"\", 42\"", "" },
            },
            new object[]
            {
                new string[][]
                {
                   new string[] { "header1", "header2","header3" },
                   new string[] { " r11  ", "\"r 12\"", "\"r, 13\"" },
                   new string[] { "\"r21 \"", "r22", "\"r\"\", 23\"" },
                   new string[] { "\"r31 \"", "", "\"r\"\", 33\"" },
                   new string[] { "r41","\"r \r\n\"\", 42\"", "" },
                   new string[] { "", "", "" },
                   new string[] { "\\r", "\\r\\n", "\\n" },
                },
                2,
                "\\n",
                new string[] { "\\r", "\\r\\n", "\\n" }
            }
        };

        [TestCaseSource("testCases")]
        public void TestGetRow(string[][] inputValues, int columnIndex, string fieldValue, string[] expectedResult)
        {
            //init CSV
            CommaSeparatedValues csv = new CommaSeparatedValues();
            for (int i = 0; i < inputValues.Length; i++)
                csv.AddRow(inputValues[i].ToList<string>());

            List<string> actualResult = csv.GetRow(columnIndex, fieldValue);

            Assert.AreEqual(expectedResult.Length, actualResult.Count);
            for (int i = 0; i < expectedResult.Length; i++)
                Assert.AreEqual(expectedResult[i], actualResult[i]);
        }
    }
}

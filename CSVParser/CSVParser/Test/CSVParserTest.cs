using CSVParser.Code;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CSVParser.Test
{
    [TestFixture]
    public class CSVParserTest
    {
        private static string GetAssemblyPath()
        {
            var assembly = Assembly.GetExecutingAssembly();
            return new DirectoryInfo(Path.GetDirectoryName(assembly.Location)).FullName;
        }

        static object[][] testCases = new[]
        {
            //test case - Unicode, mixed fields in input: many rows, multilines, escacped, non-escaped
            new object[]
            {
                @"Test\Files\inputUnicode.csv",
                new string[][]
                {
                   new string[] { "headerąść1", "header2","header3" },
                   new string[] { " r11  ", "\"r 12\"", "\"r, 13\"" },
                   new string[] { "\"r21 \"", "r22", "\"r\"\", 23\"" },
                   new string[] { "\"r31 \"", "", "\"r\"\", 33\"" },
                   new string[] { "r41","\"r \r\n\"\", 42ł;'	óżćźźć\"", "" },
                   new string[] { "", "", "" },
                   new string[] { "\\r", "\\r\\n", "\\n" },
                },
                Encoding.Unicode

            },
            //test case - UTF-8, mixed fields in input: many rows, multilines, escacped, non-escaped
            new object[]
            {
                @"Test\Files\inputUTF8.csv",
                new string[][]
                {
                   new string[] { "headerąść1", "header2","header3" },
                   new string[] { " r11  ", "\"r 12\"", "\"r, 13\"" },
                   new string[] { "\"r21 \"", "r22", "\"r\"\", 23\"" },
                   new string[] { "\"r31 \"", "", "\"r\"\", 33\"" },
                   new string[] { "r41","\"r \r\n\"\", 42ł;'	óżćźźć\"", "" },
                   new string[] { "", "", "" },
                   new string[] { "\\r", "\\r\\n", "\\n" },
                },
                Encoding.UTF8

            },

            //test case - ASCII, mixed fields in input: many rows, multilines, escacped, non-escaped
            new object[]
            {
                @"Test\Files\input.csv",
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
                Encoding.ASCII

            },
            //test case - ASCII, empty input file
            new object[]
            {
                @"Test\Files\inputEmptyFile.csv",
                new string[][] { },
                Encoding.ASCII
            },
            //test case - ASCII, only single field in input
            new object[]
            {
                @"Test\Files\inputOnlyOneField.csv",
                new string[][] { new string[] { "header1" } },
                Encoding.ASCII
            }
            ,
            //test case - ASCII, only first row in input
            new object[]
            {
                @"Test\Files\inputOnlyFirstRow.csv",
                new string[][] { new string[] { "header1","","header3" } },
                Encoding.ASCII
            }

        };

        [TestCaseSource("testCases")]
        public void ReadTest(string inputFilePath, string[][] expectedCSVFields, Encoding encoding)
        {
            CSVFileParser parser = new CSVFileParser(Path.Combine(GetAssemblyPath(), inputFilePath), encoding);
            CommaSeparatedValues csv = parser.Read();

            Assert.AreEqual(expectedCSVFields.Length > 0 ? expectedCSVFields[0].Length : 0, csv.ColumnsCount);
            Assert.AreEqual(expectedCSVFields.Length, csv.RowsCount);

            for (int i = 0; i < expectedCSVFields.Length; i++)
            {
                for (int j = 0; j < expectedCSVFields[i].Length; j++)
                {
                    Assert.AreEqual(expectedCSVFields[i][j], csv.GetField(i, j));
                }
            }
        }


        //TODO: add invalid files tests
        //TOOD: add ToString method tests
    }
}

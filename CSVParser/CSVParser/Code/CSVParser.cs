using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Supported encodings: ASCII, UTF-8, UNICODE
 * For all encodings \r\n is used as line separator
 * 
 */

namespace CSVParser.Code
{
    /// <summary>
    /// Represents a Comma-Separeted Values (CSV) file parser.
    /// </summary>
    public class CSVFileParser
    {
        #region const fields
        private const char Comma = ',';
        private const char DQuote = '"';
        private const char CR = '\r';
        private const char LF = '\n';
        #endregion

        private string path;
        //this holds the current char that was read from input stream
        //if EOF this is null
        private char? currentCharFromFile;
        //current row in file starting from 1
        private long currentRowNumber;
        //current column number from current row, starting from 1
        private long currentColumnNumber;
        //file encoding
        private Encoding encoding;
        //current row from file
        private List<string> row;

        /// <summary>
        /// Sets initial values for shared fileds before reading file
        /// </summary>
        private void SetInitialValuesBeforRead()
        {
            row = new List<string>();
            currentColumnNumber = 1;
            currentRowNumber = 1;
            currentCharFromFile = null;
        }

        /// <summary>
        /// Returns error message with information about current file position. 
        /// </summary>
        /// <param name="message">Input message.</param>
        /// <returns></returns>
        private string GetErrorMessage(string message)
        {
            return string.Format("{0}{1}", $"Error in line {currentRowNumber} col {currentColumnNumber}: ", message);
        }

        /// <summary>
        /// Adds current row to provided CommaSeparatedValues object.
        /// </summary>
        /// <param name="csvFields">CommaSeparatedValues object where new row will be added.</param>
        private void AddRowToCSV(CommaSeparatedValues csvFields)
        {
            if (csvFields.RowsCount > 0)
            {
                if (row.Count != csvFields.ColumnsCount)
                    throw new Exception(GetErrorMessage("All rows must have same length as first row."));
            }

            csvFields.AddRow(row);
            row = new List<string>();
        }

        /// <summary>
        /// Reads next character from StreamReader, stores it in currentCharFromFile field and returns its value. If end of file then currentCharFromFile will be set to null.
        /// </summary>
        /// <param name="reader">Input StreamReader.</param>
        /// <returns>Next character from StreamReader, if end of file returns null.</returns>
        private char? ReadNextCharacter(StreamReader reader)
        {
            int nextCharAsInt = reader.Read();
            if (nextCharAsInt == -1)
                currentCharFromFile = null;
            else
                currentCharFromFile = (char)nextCharAsInt;

            if (currentCharFromFile == LF)
            {
                currentRowNumber++;
                currentColumnNumber = 1;
            }
            else if (currentCharFromFile != null && currentCharFromFile != CR)
                currentColumnNumber++;

            return currentCharFromFile;
        }

        /// <summary>
        /// Read non-escaped field and returns it's value.
        /// </summary>
        /// <param name="reader">Input StreamReader.</param>
        /// <returns>Non-escaped field value.</returns>
        private string ReadNonEscapedField(StreamReader reader)
        {
            StringBuilder nonEscapedField = new StringBuilder();
            nonEscapedField.Append(currentCharFromFile);

            int nextCharacterAsInt = -1;
            while ((nextCharacterAsInt = reader.Peek()) != -1)
            {
                char nextChar = (char)nextCharacterAsInt;
                if (nextChar == Comma)
                {
                    return nonEscapedField.ToString();
                }
                else if (nextChar == LF || nextChar == CR)
                {
                    return nonEscapedField.ToString();
                }
                else if (nextChar == DQuote)
                {
                    throw new Exception(GetErrorMessage($"Invalid character '{nextChar}'."));
                }
                else
                {
                    ReadNextCharacter(reader);
                    nonEscapedField.Append(this.currentCharFromFile);
                }
            }
            return nonEscapedField.ToString();
        }

        /// <summary>
        /// Read escaped field and returns it's value.
        /// </summary>
        /// <param name="reader">Input StreamReader.</param>
        /// <returns>Escaped field value.</returns>
        private string ReadEscapedField(StreamReader reader)
        {
            StringBuilder escapedField = new StringBuilder();
            escapedField.Append(currentCharFromFile);

            while (ReadNextCharacter(reader) != null)
            {
                if (currentCharFromFile == DQuote)
                {
                    escapedField.Append(currentCharFromFile);
                    int nextCharacterAsInt = (int)reader.Peek();
                    if (nextCharacterAsInt != -1 && ((char)nextCharacterAsInt) == DQuote)
                    {
                        ReadNextCharacter(reader);
                        escapedField.Append(currentCharFromFile);
                    }
                    else
                        return escapedField.ToString();
                }

                else
                    escapedField.Append(currentCharFromFile);
            }

            throw new Exception(GetErrorMessage($"Invalid field, missing closing character: {DQuote}."));

        }

        /// <summary>
        /// Initializes a new instance of parser with default encoding.  
        /// </summary>
        /// <param name="path">Path to CSV file.</param>
        public CSVFileParser(string path) : this(path, Encoding.ASCII)
        {
            this.path = path;
        }

        /// <summary>
        /// Initializes a new instance of parser with provided encoding.  
        /// </summary>
        /// <param name="path">Path to CSV file.</param>
        /// <param name="encoding">File encoding.</param>
        public CSVFileParser(string path, Encoding encoding)
        {
            this.path = path;
            this.encoding = encoding;
        }

        /// <summary>
        /// Reads file and returns CommaSeparatedValues.
        /// </summary>
        /// <returns>CommaSeparatedValues objects with valus from file.</returns>
        public CommaSeparatedValues Read()
        {
            CommaSeparatedValues csv = new CommaSeparatedValues();
            bool canAddNewValue = true;//if true then we can add new field to CSV, else we can't add new field
            SetInitialValuesBeforRead();

            using (StreamReader reader = new StreamReader(path, encoding))
            {
                while (ReadNextCharacter(reader) != null)
                {
                    if (currentCharFromFile == DQuote)
                    {
                        string val = ReadEscapedField(reader);
                        if (canAddNewValue)
                        {
                            row.Add(val);
                            canAddNewValue = false;
                        }
                        else
                            throw new Exception(GetErrorMessage("Invalid field. Probably missing field separator"));
                    }
                    else if (currentCharFromFile == Comma)
                    {
                        if (canAddNewValue)
                            row.Add(string.Empty);//add empty value 

                        canAddNewValue = true; //this is always true                    }
                    }
                    else if (currentCharFromFile == CR)
                    {
                        if (canAddNewValue)
                            row.Add(string.Empty);//add empty value                            

                        AddRowToCSV(csv);
                        canAddNewValue = true;

                        //currenlty char is \r next should be \n
                        ReadNextCharacter(reader);
                        if (currentCharFromFile != LF)
                            throw new Exception(GetErrorMessage($"Expecting {CR} character."));
                    }
                    else
                    {
                        string val = ReadNonEscapedField(reader);
                        if (canAddNewValue)
                        {
                            row.Add(val);
                            canAddNewValue = false;
                        }
                        else
                            throw new Exception(GetErrorMessage("Invalid field. Probably missing field separator."));
                    }
                }

                if (row.Count > 0)
                    AddRowToCSV(csv);
            }

            return csv;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CSVParser.Code
{
    /// <summary>
    /// Represents Comma-separated Values (CSV)
    /// </summary>
    public class CommaSeparatedValues
    {
        private const string FieldSeparator = "|";
        //stores CSV values
        private List<List<string>> fields;
        //stores max length from all values that are currently stored
        private int maxFieldLength;

        private string RowsToString(List<List<string>> rows, int totalWidth)
        {
            StringBuilder output = new StringBuilder();
            DateTime start = DateTime.Now;
            foreach (List<string> row in rows)
            {
                output.Append(FieldSeparator);
                foreach (string s in row)
                {
                    output.Append(s.PadLeft(totalWidth));
                    output.Append(FieldSeparator);
                }
                output.AppendLine();
            }

            return output.ToString();
        }
        private void ValidateNewRow(List<string> row)
        {
            if (row == null)
                throw new ArgumentNullException(nameof(row));
            if (row.Count == 0)
                throw new ArgumentException("Empty array is not allowed.", nameof(row));

            if (fields.Count > 0)
            {
                if (fields[0].Count != row.Count)
                    throw new ArgumentException($"Array must be of length {fields[0].Count}", nameof(row));
            }

        }
        private int AddRowToListAndGetMaxLength(List<List<string>> rows, List<string> rowToAdd, int currentMaxLength)
        {
            rows.Add(rowToAdd);

            return Math.Max(currentMaxLength, rowToAdd.Max(r => r.Length));
        }

        public int ColumnsCount { get { return fields.Count > 0 ? fields[0].Count : 0; } }
        public int RowsCount { get { return fields.Count; } }

        public CommaSeparatedValues()
        {
            maxFieldLength = 0;
            fields = new List<List<string>>();
        }

        /// <summary>
        /// Validates and adds new row to CSV
        /// </summary>
        /// <param name="row">Row to be added</param>
        public void AddRow(List<string> row)
        {
            ValidateNewRow(row);
            maxFieldLength = AddRowToListAndGetMaxLength(fields, row, maxFieldLength);
        }

        /// <summary>
        /// Returns value in given row and column.
        /// </summary>
        /// <param name="rowIndex">0-based row index.</param>
        /// <param name="columnIndex">0-based column index.</param>
        /// <returns></returns>
        public string GetField(int rowIndex, int columnIndex)
        {
            if (rowIndex < 0 || rowIndex > fields.Count - 1)
                throw new ArgumentOutOfRangeException(nameof(rowIndex));

            List<string> row = fields[rowIndex];
            if (columnIndex < 0 || columnIndex > row.Count - 1)
                throw new ArgumentOutOfRangeException(nameof(columnIndex));

            return row[columnIndex];
        }

        /// <summary>
        /// Returns first row with given filterFieldValue in column filterColumnIndex. If not found returns null.
        /// </summary>
        /// <param name="filterColumnIndex">0-based column index.</param>
        /// <param name="filterFieldValue">Value to search</param>
        /// <returns>First row with given filterFieldValue in column filterColumnIndex or null if not found</returns>
        public List<string> GetRow(int filterColumnIndex, string filterFieldValue)
        {
            List<string> filteredRow = null;
            if (filterColumnIndex < 0 || filterColumnIndex >= ColumnsCount)
                throw new IndexOutOfRangeException("Parameter {0}={1} out of range");
            
            if (RowsCount > 0)
            {
                int rowIndex = -1;
                for (int i = 1; i < RowsCount; i++)
                {
                    if (fields[i][filterColumnIndex] == filterFieldValue)
                    {
                        rowIndex = i;
                        break;
                    }
                }

                if (rowIndex != -1)
                {
                    filteredRow = fields[rowIndex];
                }
            }
            return filteredRow;
        }

        /// <summary>
        /// Returns string representation of CSV.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return RowsToString(fields, maxFieldLength);
        }

        /// <summary>
        /// Returns string representation of CSV filtered to row with given filterFieldValue in selected column
        /// </summary>
        /// <param name="filterColumnIndex">0-based column index to check for filterFieldValue.</param>
        /// <param name="filterFieldValue">Filter value.</param>
        /// <returns>String representation of filtered row.</returns>
        public string ToString(int filterColumnIndex, string filterFieldValue)
        {
            List<List<string>> filteredFields = new List<List<string>>();
            int maxFieldLengthForFilteredData = 0;
            List<string> row = GetRow(filterColumnIndex, filterFieldValue);
            if (row != null)
            {
                maxFieldLengthForFilteredData = AddRowToListAndGetMaxLength(filteredFields, fields[0], maxFieldLengthForFilteredData);
                maxFieldLengthForFilteredData = AddRowToListAndGetMaxLength(filteredFields, row, maxFieldLengthForFilteredData);
            }

            return RowsToString(filteredFields, maxFieldLengthForFilteredData);
        }        
    }
}

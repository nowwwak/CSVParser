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


        public void AddRow(List<string> row)
        {
            ValidateNewRow(row);
            maxFieldLength = AddRowToListAndGetMaxLength(fields, row, maxFieldLength);
        }

        public string GetField(int rowIndex, int columnIndex)
        {
            if (rowIndex < 0 || rowIndex > fields.Count - 1)
                throw new ArgumentOutOfRangeException(nameof(rowIndex));

            List<string> row = fields[rowIndex];
            if (columnIndex < 0 || columnIndex > row.Count - 1)
                throw new ArgumentOutOfRangeException(nameof(columnIndex));

            return row[columnIndex];
        }

        public override string ToString()
        {
            return RowsToString(fields, maxFieldLength);
        }

        public string ToString(string filterColumnName, string filterFieldValue)
        {
            List<List<string>> filteredFields = new List<List<string>>();
            int maxFieldLengthForFilteredData = 0;
            if (RowsCount > 0)
            {
                int index = fields[0].IndexOf(filterColumnName);
                if (index == -1)
                    throw new Exception($"Column with name {filterColumnName} not found.");

                maxFieldLengthForFilteredData = AddRowToListAndGetMaxLength(filteredFields, fields[0], maxFieldLengthForFilteredData);

                int rowIndex = -1;
                for (int i = 1; i < RowsCount; i++)
                {
                    if (fields[i][index] == filterFieldValue)
                    {
                        rowIndex = i;
                        break;
                    }
                }

                if (rowIndex != -1)
                {
                    maxFieldLengthForFilteredData = AddRowToListAndGetMaxLength(filteredFields, fields[rowIndex], maxFieldLengthForFilteredData);
                }
            }

            return RowsToString(filteredFields, maxFieldLengthForFilteredData);
        }
    }
}

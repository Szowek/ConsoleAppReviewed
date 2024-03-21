namespace ConsoleApp
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    public class DataReader
    {
        IEnumerable<ImportedObject> ImportedObjects;

        public void ImportAndPrintData(string fileToImport, bool printData = true)
        {
            ImportedObjects = new List<ImportedObject>() {}; // deleted initialization of first null object "new ImportedObject()" due to an error in imported data clear loop further below

            var streamReader = new StreamReader(fileToImport);

            var importedLines = new List<string>();
            while (!streamReader.EndOfStream)
            {
                var line = streamReader.ReadLine();
                importedLines.Add(line);
            }

            for (int i = 0; i < importedLines.Count; i++) // changed the executing condition operator from <= to < to prevent reading out of range lines
            {
                var importedLine = importedLines[i];

                if (string.IsNullOrWhiteSpace(importedLine)) // the loop throws an error because the line is blank, included below check to skip said lines
                    continue;

                var values = importedLine.Split(';');

                if (values.Length < 7) // a check if the imported line lacks some fields
                {
                    int missingFields = 7 - values.Length;
                    Array.Resize(ref values, 7);
                    for (int j = values.Length - missingFields; j < values.Length; j++)
                    {
                        values[j] = ""; // insert blank properties into missing fields
                    }
                }

                var importedObject = new ImportedObject();
                importedObject.Type = values[0];
                importedObject.Name = values[1];
                importedObject.Schema = values[2];
                importedObject.ParentName = values[3];
                importedObject.ParentType = values[4];
                importedObject.DataType = values[5];
                importedObject.IsNullable = values[6];
                ((List<ImportedObject>)ImportedObjects).Add(importedObject);
            }

            // clear and correct imported data
            foreach (var importedObject in ImportedObjects)
            {
                /*
                 * Added .ToUpper() in ParentType to avoid the need to use the method multiple
                 * times when its compared against other properties in if statements below.
                 * 
                 * Additionaly I've created extension for string in order to use it in properties
                 * below to reduce repeatability of the code and simply make it more readable.
                 */
                importedObject.Type = importedObject.Type.ClearData().ToUpper();
                importedObject.Name = importedObject.Name.ClearData();
                importedObject.Schema = importedObject.Schema.ClearData();
                importedObject.ParentName = importedObject.ParentName.ClearData();
                importedObject.ParentType = importedObject.ParentType.ClearData().ToUpper();
            }

            // assign number of children
            for (int i = 0; i < ImportedObjects.Count(); i++)
            {
                var importedObject = ImportedObjects.ToArray()[i];
                foreach (var impObj in ImportedObjects)
                {
                    // Merged nested ifs into a single one for better readability
                    if (impObj.ParentType == importedObject.Type && impObj.ParentName == importedObject.Name)
                    {
                        importedObject.NumberOfChildren = 1 + importedObject.NumberOfChildren;
                    }
                }
            }

            foreach (var database in ImportedObjects)
            {
                if (database.Type == "DATABASE")
                {
                    Console.WriteLine($"Database '{database.Name}' ({database.NumberOfChildren} tables)");

                    // print all database's tables
                    foreach (var table in ImportedObjects)
                    {
                        // Another batch of nested ifs that were present below merged
                        if (table.ParentType == database.Type && table.ParentName == database.Name)
                        {
                            Console.WriteLine($"\tTable '{table.Schema}.{table.Name}' ({table.NumberOfChildren} columns)");

                            // print all table's columns
                            foreach (var column in ImportedObjects)
                            {
                                if (column.ParentType == table.Type && column.ParentName == table.Name)
                                {
                                    Console.WriteLine($"\t\tColumn '{column.Name}' with {column.DataType} data type {(column.IsNullable == "1" ? "accepts nulls" : "with no nulls")}");
                                }
                            }
                        }
                    }
                }
            }

            Console.ReadLine();
        }
    }

    // String extension responsible for clearing imported data
    public static class StringExtensions
    {
        public static string ClearData(this string input)
        {
            return input.Trim().Replace(" ", "").Replace(Environment.NewLine, "");
        }
    }

    /*
     * Adjusted spaced and lines in below ImportedObject class for
     * better readability and cleaner code. 
     * Previously some properties had different writing convention.
     * It is better to use the same style for the whole code as it
     * makes it easier to read and review.
     */

    class ImportedObject : ImportedObjectBaseClass
    {
        public string Name { get; set; }
        public string Schema;

        public string ParentName;
        public string ParentType { get; set; }

        public string DataType { get; set; }

        public string IsNullable { get; set; }

        public double NumberOfChildren;
    }

    class ImportedObjectBaseClass
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }
}

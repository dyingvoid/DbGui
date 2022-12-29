using System;
using System.Collections.Generic;
using System.Linq;

namespace DbGui.Models;

public class DbTest
{
    public DbTest(CsvTable csv)
    {
        Csv = csv;
    }
    
    public CsvTable Csv { get; set; }
    
    public void Test()
    {
        CheckStructureEquality(Csv.Types, Csv.Columns);
        CheckTableDimensionsEquality(Csv.Table, Csv.Columns);
        CheckColumnsDataTypeEquality(Csv);
    }
    
    private void CheckStructureEquality(Dictionary<string, Type> structure, List<string> columns)
    {
        var structureNames = structure.Keys.ToHashSet();
        var columnNames = columns.ToHashSet();

        if (!structureNames.SetEquals(columnNames))
            throw new Exception("Column names do not match names in json structure.");
    }

    private void CheckTableDimensionsEquality(List<List<string?>> table, List<string> columns)
    {
        var size = columns.Count;
        if (size <= 0)
            throw new Exception("Could not find any column in table.");

        for (int i = 0; i < table.Count; ++i)
        {
            if (table[i].Count != size)
            {
                throw new Exception($"Stroke with index {i}(or {i + 2} in file) is size of {table[i].Count}," +
                                    $"when must be {size}");
            }
        }
    }

    private void CheckColumnsDataTypeEquality(CsvTable csv)
    {
        if (csv.Table.Count <= 0) return;
        
        // Foreach column
        foreach (var columnName in csv.Columns)
        {
            var column = csv.GetColumnWithName(columnName);
            var columnType = csv.FindTypeInJsonByColumnName(columnName);
            
            try
            {
                CheckColumnDataTypeEquality(column, columnType);
            }
            catch (Exception)
            {
                LogHelper.Log($"Error in {columnName} column.");
                throw;
            }
        }
    }
    
    private static void CheckColumnDataTypeEquality(List<string?> column, Type type)
    {
        foreach (var element in column)
        {
            if (type != typeof(String))
            {
                try
                {
                    var castGenericMethod = DbReflection.ChooseGenericTypeCastMethodByTypeConstraints(type);
                    DbReflection.TryCastToType(type, castGenericMethod, element);
                }
                catch (Exception ex)
                {
                    LogHelper.Log($"Problem with data type checking.");
                    throw;
                }
            }
        }
    }
}
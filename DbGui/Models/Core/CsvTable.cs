using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DbGui.Models;

public class CsvTable : IEnumerable<List<string?>>
{
    private List<List<string?>> _table;

    public CsvTable(FileInfo csvFile, Dictionary<string, string> configuration)
    {
        Types = new Dictionary<string, Type>();
        Columns = new List<string>();
        Shape = new Tuple<long?, long?>(null, null);
        Name = csvFile.Name;
        
        try
        {
            _table = CreateTableFromFile(csvFile.FullName);
            SetColumnTypes(configuration);
            SetColumns();
            Table.RemoveAt(0);
            MakeEmptyAndSpaceElementsNull();

            var tests = new DbTest(this);
            tests.Test();
        }
        catch (Exception ex)
        {
            LogHelper.Log(ex.Message + $" Could not create CsvTable with {csvFile.Name}.");
            _table = new List<List<string?>>();
            Columns = new List<string>();
        }
        
        SetShape();
    }
    
    public string Name { get; set; }
    public List<List<string?>> Table
    {
        get => _table;
        set => _table = value;
    }
    
    /// <summary>
    /// Number of columns, number of strokes
    /// </summary>
    public Tuple<long?, long?> Shape { get; set; }
    public object this[int index] => GetColumnWithIndex(Table, index);
    public Dictionary<string, Type> Types { get; }
    public List<string> Columns { get; set; }

    private void SetColumns()
    {
        try
        {
            Columns = Table[0];
        }
        catch (Exception ex)
        {
            LogHelper.Log("Could not set table columns. Csv file is probably empty.");
            throw;
        }
    }

    private List<List<string>> CreateTableFromFile(string filePath)
    {
        return ReadFromFileToList(filePath);
    }
    
    public void SetShape()
    {
        Shape = Tuple.Create<long?, long?>(Columns.Count, Table.Count);
    }

    private static List<List<string>> ReadFromFileToList(string filePath)
    {
        if (!filePath.EndsWith(".csv"))
            throw new Exception("Can't read non csv file.");
        
        var tempCsvTable = File.ReadAllLines(filePath)
                .ToList()
                .PureForEach<List<string>, string, List<List<string>>, List<string>>
                    (line => line.Split(',').ToList());
        
        return tempCsvTable;
    }
    
    private void SetColumnTypes(Dictionary<string, string> types)
    {
        foreach (var (key, value) in types)
        {
            var type = Type.GetType(value);
            try
            {
                Types.Add(key, type);
            }
            catch (Exception)
            {
                LogHelper.Log("Could not set column types.");
                throw;
            }
        }
    }

    public Type FindTypeInJsonByColumnName(string? name)
    {
        try
        {
            var type = Types[name];
            
            if (type == null)
                throw new NullReferenceException();
            
            return type;
        }
        catch (Exception)
        {
            LogHelper.Log($"Could not find {name} column in json structure.");
            throw;
        }
    }

    public List<string?> GetColumnWithIndex(List<List<string?>> table, int index)
    {
        var column = new List<string?>();

        foreach (var stroke in table)
        {
            column.Add(stroke[index]);
        }

        return column;
    }

    public List<string?> GetColumnWithName(List<List<string?>> table, List<string> columnNames, string columnName)
    {
        var indexOfColumn = columnNames.FindIndex(name=> name == columnName);
        
        if (indexOfColumn < 0)
            throw new Exception($"Could not find column with name {columnName} in Columns");

        try
        {
            return GetColumnWithIndex(table, indexOfColumn);
        }
        catch (Exception)
        {
            LogHelper.Log($"Could not find data of {columnName} column. Check format of table");
            throw;
        }
    }

    public List<string?> GetColumnWithName(string columnName)
    {
        return GetColumnWithName(Table, Columns, columnName);
    }

    public void MakeEmptyAndSpaceElementsNull()
    {
        foreach (var stroke in _table)
        {
            for (var j = 0; j < stroke.Count; ++j)
            {
                if(stroke[j].IsEmptyOrWhiteSpace())
                    stroke[j] = null;
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    
    public IEnumerator<List<string?>> GetEnumerator()
    {
        var enumerator = _table.GetEnumerator();
        return enumerator;
    }

    public List<string?> At(int index)
    {
        return Table[index];
    }

    public void MergeByColumn(CsvTable csv, string columnName1, string columnName2)
    {
        var thisColumnIndex = Columns.FindIndex(name => name == columnName1);
        var csvColumnIndex = csv.Columns.FindIndex(name => name == columnName2);

        var elementsIntersection = GetColumnWithIndex(Table, thisColumnIndex)
            .Intersect(GetColumnWithIndex(csv.Table, csvColumnIndex));

        Table = CreateMergedTable(csv, elementsIntersection, csvColumnIndex, thisColumnIndex);

        Columns.AddRange(csv.Columns);
        Columns.Remove(columnName2);
        
        foreach (var (name, type) in csv.Types)
            Types.Add(name, type);
        
        Types.Remove(columnName2);
        
        SetShape();

        var test = new DbTest(csv);
        test.Test();
    }

    private List<List<string?>> CreateMergedTable(CsvTable csv, 
        IEnumerable<string?> elementsIntersection, 
        int csvColumnIndex, 
        int thisColumnIndex)
    {
        var mergedTables = new List<List<string?>>();

        foreach (var element in elementsIntersection)
        {
            if (element != null)
            {
                var csvStrokes = csv.Table
                    .FindAll(stroke => stroke[csvColumnIndex] == element);

                var thisStroke = Table.Find(stroke => stroke[thisColumnIndex] == element);

                foreach (var stroke in csvStrokes)
                {
                    var csvStrokeCopy = new List<string?>(stroke);
                    csvStrokeCopy.RemoveAt(csvColumnIndex);

                    var thisStrokeCopy = new List<string?>(thisStroke);

                    thisStrokeCopy.AddRange(csvStrokeCopy);
                    mergedTables.Add(thisStrokeCopy);
                }
            }
        }

        return mergedTables;
    }

    public int GetColumnWidth(string columnName)
    {
        var column = GetColumnWithName(columnName);

        int length = 0;
        foreach (var element in column)
        {
            if (element!= null && element.Length > length)
                length = element.Length;
        }

        try
        {
            var smt = Columns.Find(name => name == columnName).Length;
        }
        catch (Exception)
        {
            LogHelper.Log($"There is no column with name {columnName}");
            throw;
        }

        return Math.Max(length, Columns.Find(name => name == columnName).Length);
    }
}
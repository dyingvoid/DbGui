using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using DbGui.Models;

namespace DbGui.ViewModels;

public class TableEditorViewModel
{
    public TableEditorViewModel(DataGrid dataGrid)
    {
        TableView = dataGrid;

        string csvs = @"C:\Users\Administrator\Downloads\csvs";
        string json = @"C:\Users\Administrator\Downloads\csvs\structure.json";
        var listTables = CsvJsonFilesInteraction.LoadTables(csvs, json);
        
        FillTableView(listTables[0]);
    }
    
    public DataGrid TableView { get; set; }

    private void FillTableView(CsvTable table)
    {
        var collection = GenerateCollection(table);
        TableView.ItemsSource = collection;
    }

    private ObservableCollection<object?> GenerateCollection(CsvTable table)
    {
        var c = new ObservableCollection<object?>();
        
        foreach (var stroke in table)
        {
            var tableObject = TableEditorReflectionTypeCreator.CreateNewObject(table.Types);
            TableEditorReflectionTypeCreator.FillObjectWithData(tableObject, table.Types, stroke);
            c.Add(tableObject);
        }

        return c;
    }
}

public class MyType
{
    public string Name { get; set; }
    public string AuthorName { get; set; }
    public int YearPublished { get; set; }
    public int Case { get; set; }
    public int Shelf { get; set; }

    public MyType(List<string?> list)
    {
        Name = list[0];
        AuthorName = list[1];
        YearPublished = int.Parse(list[2]);
        Case = int.Parse(list[3]);
        Shelf = int.Parse(list[4]);
    }
}
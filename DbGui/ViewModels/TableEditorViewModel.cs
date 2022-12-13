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
            var tableObject = DbReflection.CreateNewObject(table.Types);
            DbReflection.FillObjectWithDataMetaData(tableObject, table.Types, stroke);
            c.Add(tableObject);
        }

        return c;
    }
}
using System.Linq;
using System.Windows.Controls;
using DbGui.Models;

namespace DbGui.ViewModels;

public class TableListViewModel
{
    public TableListViewModel(ListBox listOfTables)
    {
        ListOfTables = listOfTables;
        FillList();
    }
    
    public ListBox ListOfTables { get; set; }

    private void FillList()
    {
        string csvs = @"C:\Users\Administrator\Downloads\csvs";
        string json = @"C:\Users\Administrator\Downloads\csvs\structure.json";
        var listTables = CsvJsonFilesInteraction.LoadTables(csvs, json);

        var tableNames = (from csvTable in listTables select csvTable.Name);
        ListOfTables.ItemsSource = tableNames;
    }
}
using System.Collections.Generic;
using DbGui.Models;

namespace DbGui.ViewModels;

public class TablesPageViewModel
{
    public TablesPageViewModel(VladDbFile dbFile)
    {
        CsvTables = CsvJsonFilesInteraction.LoadTables(dbFile.CsvFolder, dbFile.JsonStructure);
    }
    
    public List<CsvTable> CsvTables { get; set; }
}
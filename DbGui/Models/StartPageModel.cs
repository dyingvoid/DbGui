using System;
using System.IO;
using System.Linq;

namespace DbGui.Models;

public class VladDbFile
{
    public VladDbFile(string dbFilePath)
    {
        ValidateFile(dbFilePath);
    }
    
    public DirectoryInfo CsvFolder { get; set; }
    public FileInfo JsonStructure { get; set; }

    private void ValidateFile(string dbFilePath)
    {
        var paths = System.IO.File.ReadAllLines(dbFilePath).ToList();
        if (paths.Count != 2)
            throw new Exception("File contains wrong number of paths.");

        CsvFolder = new DirectoryInfo(paths[0]);
        JsonStructure = new FileInfo(paths[1]);

        if (!CsvFolder.Exists)
            throw new Exception("Folder with csv tables does not exist.");
        if (!JsonStructure.Exists)
            throw new Exception("Json structure does not exist.");
    }
}
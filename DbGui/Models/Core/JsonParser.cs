using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace DbGui.Models;

public static class JsonParser
{
    public static Dictionary<string, Dictionary<string, string>>? Parse(FileInfo file)
    {
        using StreamReader stream = new StreamReader(file.FullName);
        string json = stream.ReadToEnd();
        var jsonDict = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(json);

        return jsonDict;
    }
}
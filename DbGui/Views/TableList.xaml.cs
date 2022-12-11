using System.Collections.Generic;
using System.Windows.Controls;

namespace DbGui.Views;

public partial class TableListUserControl : UserControl
{
    public TableListUserControl()
    {
        Table = new()
        {
            new() { "I", "am", "watching" },
            new() { "And", "I", "don't" }
        };
        
        InitializeComponent();
    }
    
    public List<List<string?>> Table { get; set; }
}
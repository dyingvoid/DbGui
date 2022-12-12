using System.Collections.Generic;
using System.Windows.Controls;
using DbGui.ViewModels;

namespace DbGui.Views;

public partial class TableListUserControl : UserControl
{
    public TableListUserControl()
    {
        InitializeComponent();

        DataContext = new TableEditorViewModel(DataGrid);
    }
}
using System.Windows.Controls;
using DbGui.ViewModels;

namespace DbGui.Views;

public partial class TableEditorUserControl : UserControl
{
    public TableEditorUserControl()
    {
        InitializeComponent();
        
        DataContext = new TableEditorViewModel(DataGrid);
    }
}
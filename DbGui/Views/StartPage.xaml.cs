using System.Windows.Controls;
using DbGui.ViewModels;

namespace DbGui.Views;

public partial class StartPage : Page
{
    public StartPage()
    {
        InitializeComponent();
        DataContext = new StartPageViewModel(SearchButton, ExceptionTextBlock);
    }
}
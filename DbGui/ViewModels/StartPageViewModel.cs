using System;
using System.Windows;
using System.Windows.Controls;
using DbGui.Models;
using Microsoft.Win32;

namespace DbGui.ViewModels;

public class StartPageViewModel
{
    private Button _searchButton;
    public StartPageViewModel(Button button, TextBlock exceptionTextBlock)
    {
        SearchButton = button;
        ExceptionTextBlock = exceptionTextBlock;
    }

    public Button SearchButton
    {
        get => _searchButton;
        set
        {
            _searchButton = value;
            _searchButton.Click += OpenVladDbFile;
        }
    }
    
    public VladDbFile? DbFile { get; set; }
    public TextBlock ExceptionTextBlock { get; set; }

    private void OpenVladDbFile(object sender, RoutedEventArgs eventArgs)
    {
        var dbFileDialog = new OpenFileDialog();
        
        if (dbFileDialog.ShowDialog() != true)
        {
            ChangeExceptionText("File was not opened.");
            return;
        }
        
        try
        {
            DbFile = new VladDbFile(dbFileDialog.FileName);
        }
        catch (Exception ex)
        {
            ChangeExceptionText($"{ex.Source} {ex.Message}");
        }
        
    }

    private void ChangeExceptionText(string exceptionInformation)
    {
        ExceptionTextBlock.Text = exceptionInformation;
    }
}
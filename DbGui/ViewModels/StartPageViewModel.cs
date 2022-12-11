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
            _searchButton.Click += SearchButtonClicked;
        }
    }

    public TextBlock ExceptionTextBlock { get; set; }

    private void SearchButtonClicked(object sender, RoutedEventArgs eventArgs)
    {
        var dbFileDialog = new OpenFileDialog();
        if (dbFileDialog.ShowDialog() == true)
        {
            try
            {
                var vladDb = new VladDbFile(dbFileDialog.FileName);
                ChangeExceptionText(vladDb.CsvFolder.FullName + " " + vladDb.JsonStructure.FullName);
            }
            catch (Exception ex)
            {
                ChangeExceptionText($"{ex.Source} {ex.Message}");
            }
        }
        else
        {
            ChangeExceptionText("File was not opened.");
        }
    }

    private void ChangeExceptionText(string exceptionInformation)
    {
        ExceptionTextBlock.Text = exceptionInformation;
    }
}
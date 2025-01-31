using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using CoreTaxXlsxReader;
using CoreTaxXmlWriter;
using CoreTaxXmlWriter.Models;

namespace CoreTaxXlsxToXmlConverter;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    public async void OpenXslxClickHandler(object sender, RoutedEventArgs args)
    {
        var inputFile = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open Xlsx File",
            AllowMultiple = false,
            FileTypeFilter = [XlsxFilePicker()]
        });

        if (inputFile.Count < 1)
        {
            return;
        }

        statusTextBlock.Text = "Start Reading Excel file";
        TaxInvoiceReader reader = new();
        TaxInvoiceBulk result = await Task.Run(() => reader.ReadFile(inputFile[0].Path.LocalPath));
        statusTextBlock.Text = "Finish Reading Excel file";

        IStorageFile? outputFile = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save Xml File",
            DefaultExtension = ".xml",
            FileTypeChoices = [XmlFilePicker()]
        });

        if (outputFile == null)
        {
            return;
        }

        statusTextBlock.Text = "Start Writing Xml file";
        TaxInvoiceWriter writer = new();
        await Task.Run(() => writer.WriteXml(result, outputFile.Path.LocalPath));
        statusTextBlock.Text = "Finish Writing Xml file";
    }

    private TaxInvoiceBulk ReadXlsx(string path)
    {
        TaxInvoiceReader reader = new();
        return reader.ReadFile(path);
    }

    private static FilePickerFileType XlsxFilePicker()
    {
        return new FilePickerFileType("Xlsx")
        {
            Patterns = ["*.xlsx"],
            MimeTypes = ["*/*"]
        };
    }

    private static FilePickerFileType XmlFilePicker()
    {
        return new FilePickerFileType("Xml")
        {
            Patterns = ["*.xml"],
            MimeTypes = ["*/*"]
        };
    }
}

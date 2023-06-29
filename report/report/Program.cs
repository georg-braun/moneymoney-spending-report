// See https://aka.ms/new-console-template for more information

using System.Transactions;
using report;

const string transactionFileName = "transactions.csv";
var transactionCsvReport = await CsvTransactionReport.InitializeAsync(transactionFileName);

var categories = transactionCsvReport.GetCategories().OrderBy(_ => _.Name).ToList();
ConsoleReport.PrintCategories(categories);
await CsvReport.WriteAsync(categories, "report.csv", new[] {2021, 2022, 2023});

Console.Read();

internal class CsvTransactionReport
{
    private readonly string _transactionFileName;
    private string[] _fileContent;

    private CsvTransactionReport(string transactionFileName)
    {
        _transactionFileName = transactionFileName;
    }

    public async Task ImportAsync()
    {
        _fileContent = await File.ReadAllLinesAsync(_transactionFileName);
    }

    public Dictionary<string, int> GetColumnDescriptions()
    {
        var columns = _fileContent[0].Split(';');

        return columns.Select((column, index) => new ColumnDescription
        {
            Name = column,
            Index = index
        }).ToDictionary(_ => _.Name, _ => _.Index);
    }

    private List<Transaction> GetSpendings()
    {
        var indexByName = GetColumnDescriptions();
        var transactions = _fileContent.Skip(1).Select(line => line.Split(';')).Select(line => new Transaction
        {
            Date = DateTime.Parse(line[indexByName["Datum"]]),
            Amount = decimal.Parse(line[indexByName["Betrag"]]),
            Description = line[indexByName["Verwendungszweck"]],
            Category = line[indexByName["Kategorie"]]
        }).Where(_ => _.Amount < 0).ToList();
        return transactions;
    }

    public List<Category> GetCategories()
    {
        return GroupByCategory(GetSpendings());
    }


    private List<Category> GroupByCategory(IEnumerable<Transaction> list)
    {
        var categories = list.GroupBy(transaction => transaction.Category).Select(grouping => new Category
        {
            Name = grouping.Key,
            Total = grouping.Sum(transaction => transaction.Amount),
            Transactions = grouping.ToList()
        }).ToList();
        return categories;
    }

    public static async Task<CsvTransactionReport> InitializeAsync(string transactionFileName)
    {
        var report = new CsvTransactionReport(transactionFileName);
        await report.ImportAsync();
        return report;
    }
}
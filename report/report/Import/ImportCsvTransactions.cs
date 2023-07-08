using report;

internal class ImportCsvTransactions
{
    private readonly string _transactionFileName;
    private string[] _fileContent;

    private ImportCsvTransactions(string transactionFileName)
    {
        _transactionFileName = transactionFileName;
    }

    private async Task ImportAsync()
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

    public static async Task<ImportCsvTransactions> InitializeAsync(string transactionFileName)
    {
        var report = new ImportCsvTransactions(transactionFileName);
        await report.ImportAsync();
        return report;
    }
}
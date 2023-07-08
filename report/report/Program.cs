using report;




var transactionFilePath = ConsoleIo.AskForTransactionFilePath();

Console.WriteLine("Loading transactions...");
var transactionCsvReport = await ImportCsvTransactions.InitializeAsync(transactionFilePath);

Console.WriteLine("Generating report...");
var categories = transactionCsvReport.GetCategories().OrderBy(_ => _.Name).ToList();

Console.WriteLine("Writing report to console...");
ExportConsoleReport.PrintCategories(categories);

var reportFilePath = transactionFilePath.Replace(".csv", "-report.csv");

Console.WriteLine("Writing report to file...");
Console.WriteLine(reportFilePath);
await ExportCsvReport.WriteAsync(categories, reportFilePath, new[] {2021, 2022, 2023});



Console.Read();


public static class ConsoleIo
{
    public static string AskForTransactionFilePath()
    {
        Console.WriteLine("Enter the path to your MoneyMoney transaction CSV file:");
        var transactionFilePath = Console.ReadLine();

        while (!Path.Exists(transactionFilePath))
        {
            Console.WriteLine("File does not exist. Please try again.");
            transactionFilePath = Console.ReadLine();
        }

        return transactionFilePath;
    }
}
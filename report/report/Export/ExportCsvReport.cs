using System.Text;

namespace report;

static class ExportCsvReport
{
    public static async Task WriteAsync(List<Category> categories, string filepath, IEnumerable<int> years)
    {
        var stringBuilder = new StringBuilder();
        const string separator = ";";
        
        // Header
        stringBuilder.AppendLine(string.Join(separator, new[] {"Kategorie"}.Concat(years.Select(_ => _.ToString()))));
        
        // Values
        categories.ForEach(category =>
        {
            stringBuilder.Append(category.Name);
            stringBuilder.Append(separator);
            
            var spendingsPerYear = category.Transactions.GroupBy(_ => _.Date.Year).ToList();
                
            years.ToList().ForEach(year =>
            {
                var spendingsForYear = spendingsPerYear.FirstOrDefault(_ => _.Key == year);
                if (spendingsForYear != null)
                {
                    var monthCount = year == DateTime.Today.Year ? DateTime.Today.Month : 12;
                    var averagePerMonth = Math.Round(spendingsForYear.Sum(_ => _.Amount) / monthCount, 2);
                    stringBuilder.Append(averagePerMonth);
                }
                stringBuilder.Append(separator);
            });

            stringBuilder.AppendLine();
        });
        
        await File.WriteAllTextAsync(filepath, stringBuilder.ToString());
    }
}
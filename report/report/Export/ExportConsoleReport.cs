namespace report;

static class ExportConsoleReport
{
    public static void PrintCategories(List<Category> categories)
    {
        categories.ForEach(category =>
        {
            Console.WriteLine($"{category.Name}: {category.Total}");

            // spendings per year
            category.Transactions.GroupBy(_ => _.Date.Year).ToList().ForEach(grouping =>
            {
                // for current year, only count months that have passed
                var monthCount = grouping.Key == DateTime.Today.Year ? DateTime.Today.Month : 12;

                Console.WriteLine(
                    $"  {grouping.Key}: {grouping.Sum(_ => _.Amount)} (average per month: {Math.Round(grouping.Sum(_ => _.Amount) / monthCount, 2)})");
            });
        });
    }
}
public record Category
{
    public string Name { get; set; }
    public decimal Total { get; set; }
    public List<Transaction> Transactions { get; set; }
}
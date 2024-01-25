namespace AbcBooks.Models
{
    public class Dashboard
    {
        public float TotalRevenue { get; set; }
        public int LowStock { get; set; }
        public int BlockedUsers { get; set; }
        public int PendingOrders { get; set; }
    }
}

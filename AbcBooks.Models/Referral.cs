namespace AbcBooks.Models;

public class Referral
{
    public int Id { get; set; }
    public string ReferrerId { get; set; }
    public string ReferredId { get; set; }
    public bool IsFulfilled { get; set; }
}

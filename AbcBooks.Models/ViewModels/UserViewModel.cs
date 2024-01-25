namespace AbcBooks.Models.ViewModels;

public class UserViewModel
{
    public IEnumerable<ApplicationUser> Users { get; set; }
    public string SearchString { get; set; }
}

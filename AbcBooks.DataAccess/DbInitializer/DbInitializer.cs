using AbcBooks.Models;
using AbcBooks.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AbcBooks.DataAccess.DbInitializer
{
	public class DbInitializer : IDbInitializer
	{
		private readonly UserManager<IdentityUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly ApplicationDbContext _db;

		public DbInitializer(
			ApplicationDbContext db, 
			RoleManager<IdentityRole> roleManager, 
			UserManager<IdentityUser> userManager)
		{
			_db = db;
			_roleManager = roleManager;
			_userManager = userManager;
		}

		public void Initialize()
		{
			try
			{
				if (_db.Database.GetPendingMigrations().Count() > 0)
				{
					_db.Database.Migrate();
				}
			}
			catch (Exception)
			{
                Console.WriteLine("Could not migrate!");
            }

			if (!_roleManager.Roles.Any()) 
			{
				_roleManager.CreateAsync(new IdentityRole(ProjectConstants.ADMIN)).GetAwaiter().GetResult();
				_roleManager.CreateAsync(new IdentityRole(ProjectConstants.CUSTOMER)).GetAwaiter().GetResult();

				try
				{
					string password = Environment.GetEnvironmentVariable("ABCADMIN")!;
					_userManager.CreateAsync(new ApplicationUser
					{
						UserName = "admin@mail.com",
						Email = "admin@mail.com",
						EmailConfirmed = true,
						FirstName = "ADMIN",
						LastName = "ACC",
						IsBlocked = false
					}, password).GetAwaiter().GetResult();
				}
				catch (Exception e)
				{
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                }

				try
				{
					ApplicationUser user = _db.ApplicationUsers.First(u => u.Email == "admin@mail.com");

					_userManager.AddToRoleAsync(user, ProjectConstants.ADMIN).GetAwaiter().GetResult();
				}
				catch (Exception e)
				{
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                }
			}

			if (!_db.DiscountTypes.Any())
			{
				_db.DiscountTypes.Add(new DiscountType { Value = "FLAT" });
				_db.SaveChanges();
				_db.DiscountTypes.Add(new DiscountType { Value = "PERCENTAGE" });
				_db.SaveChanges();
			}
		}
	}
}

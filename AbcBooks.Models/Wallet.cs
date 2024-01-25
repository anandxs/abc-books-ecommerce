using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbcBooks.Models
{
	public class Wallet
	{
        public int Id { get; set; }
        public string ApplicationUserId { get; set; } = null!;
		[ForeignKey(nameof(ApplicationUserId))]
		public ApplicationUser ApplicationUser { get; set; } = null!;
        public float Balance { get; set; }
    }
}

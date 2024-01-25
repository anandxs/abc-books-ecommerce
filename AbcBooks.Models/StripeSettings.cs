using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbcBooks.Models
{
	public class StripeSettings
	{
        public string PublishableKey { get; set; } = null!;
        public string SecreteKey { get; set; } = null!;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbcBooks.Utilities.Interfaces
{
    public interface ISmsSender
    {
        void SendSmsAsync(string number, string message);
    }
}

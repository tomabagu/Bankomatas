using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bankomatas.Models
{
    internal class Card:CardNumber
    {
        public Card(Guid cardId, int password, decimal balance) : base(cardId)
        {
            Password = password;
            Balance = balance;
        }

        public int Password { get; set; }
        public decimal Balance { get; set; }

        public override string ToString()
        {
            return $"{CardId},{Password},{Balance}";
        }
    }
}

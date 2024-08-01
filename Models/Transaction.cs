using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bankomatas.Models
{
    internal class Transaction:CardNumber
    {
        public Transaction(Guid cardId,DateTime date, string action, decimal amount) : base(cardId)
        {
            Date = date;
            Action = action;
            Amount = amount;
        }

        public DateTime Date { get; set; }
        public string Action { get; set; }
        public decimal Amount { get; set; }

        public override string ToString()
        {
            return $"{CardId},{Date.ToString("yyyy-MM-dd HH:mm:ss")},{Action},{Amount}";
        }
    }
}

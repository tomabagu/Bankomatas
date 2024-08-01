using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bankomatas.Models
{
    internal class CardNumber
    {
        public CardNumber(Guid cardId)
        {
            CardId = cardId;
        }

        public Guid CardId { get; set; }
    }
}

using Bankomatas.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bankomatas.Services
{

    internal class BankAccountService : IBankAccountService
    {
        private BankFileService bankFileService;

        public BankAccountService(BankFileService bankFileService)
        {
            this.bankFileService = bankFileService;
        }

        //ištraukiami kortelės duomenys iš failo pagal guid
        public Card? InsertCard(string cardId)
        {
            Card card = null;
            try
            {
                card = bankFileService.GetCardData(new Guid(cardId));
            }
            catch (FormatException ex)
            {
                Console.WriteLine("Neteisingas kortelės id");
            }
            return card;

        }
        public Card ReturnCard()
        {
            return null;
        }

        //patikrinimas ar sutampa įvestas slaptažodis su kortelės
        public bool CheckPasswordValid(Card card, int password)
        {
            if (card.Password == password)
            {
                return true;
            }
            return false;
        }

        //išsaugo trankaciją
        public void SaveTransaction(Transaction transaction)
        {
            bankFileService.SaveTransaction(transaction);
        }

        //išsaugo atnaujinus kortelės duomenis į failą
        public void UpdateCardBalance(Card cardData)
        {
            List<Card> cards = bankFileService.GetAllCardData();
            cards.ForEach(card =>
            {
                if (card.CardId.Equals(cardData.CardId))
                {
                    card.Balance = cardData.Balance;
                }
            });
            bankFileService.SaveCardData(cards);
        }

        //ištraukia šiandienos transakcijų skaičių tos kortelės
        public int GetCardTransactionsCount(Card card)
        {
            List<Transaction> transactions = bankFileService.GetCardTransactions(card.CardId);
            return transactions.Where(transaction => transaction.Date.Date == DateTime.Today).Count();
        }

        //ištraukia 5 paskutines tansakcijos iš nurodytos kortelės
        public List<Transaction> GetFiveLastTransactions(Card card)
        {
            List<Transaction> transaction = bankFileService.GetCardTransactions(card.CardId);
            // Check if the list has fewer than five elements
            int count = transaction.Count;
            if (count <= 5)
            {
                return transaction.AsEnumerable().Reverse().ToList();
            }

            // Use LINQ to skip the elements before the last five and take the last five
            return transaction.Skip(count - 5).Take(5).Reverse().ToList();

        }
    }
}

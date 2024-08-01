using Bankomatas.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bankomatas.Services
{
    internal class BankFileService : IBankFileService
    {
        private readonly string _filePathCard;
        private readonly string _filePathTransactions;

        public BankFileService(string filePathCard, string filePathTransactions)
        {
            _filePathCard = filePathCard;
            _filePathTransactions = filePathTransactions;
        }


        //kortelės duomenys iš failo pagal guid
        public Card GetCardData(Guid cardId)
        {
            string[] lines;
            try
            {
                lines = File.ReadAllLines(_filePathCard, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            
            foreach (var item in lines)
            {
                var data = item.Split(',');
                if (data[0].Equals(cardId.ToString()))
                {
                    return new Card(new Guid(data[0]), int.Parse(data[1]), decimal.Parse(data[2]));
                }
            }
            return null;
        }

        //ištraukiami visų bankinių kortelių duomenys iš failo
        public List<Card> GetAllCardData()
        {
            try
            {
                var lines = File.ReadAllLines(_filePathCard, Encoding.UTF8);
                List<Card> cards = new List<Card>();
                foreach (var item in lines)
                {
                    var data = item.Split(',');
                    {
                        cards.Add(new Card(new Guid(data[0]), int.Parse(data[1]), decimal.Parse(data[2])));
                    }
                }
                return cards;
            }
            catch (Exception ex)
            {
            throw new Exception(ex.Message); 
            }
        }

        //išsaugomi banko kortelių duomenys faile
        public void SaveCardData(List<Card> cards)
        {
            List<string> cardStrings = new List<string>();
            cards.ForEach(card =>
            {
                cardStrings.Add(card.ToString());
            });
            File.WriteAllLines(_filePathCard, cardStrings);
        }

        //visos transakcijos tam tikros kortelės pagal guid
        public List<Transaction> GetCardTransactions(Guid cardId)
        {
            var lines = File.ReadAllLines(_filePathTransactions, Encoding.UTF8);
            List<Transaction> transactions = new List<Transaction>();
            foreach (var item in lines)
            {
                var data = item.Split(',');
                if (data[0].Equals(cardId.ToString()))
                {
                    transactions.Add(new Transaction(new Guid(data[0]), DateTime.Parse(data[1]), data[2], decimal.Parse(data[3])));
                }
            }
            return transactions;
        }

        //išsaugoma nauja transakcija faile
        public void SaveTransaction(Transaction transaction)
        {
            File.AppendAllText(_filePathTransactions, Environment.NewLine + transaction.ToString());
        }
    }
}

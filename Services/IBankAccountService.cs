using Bankomatas.Models;

namespace Bankomatas.Services
{
    internal interface IBankAccountService
    {
        bool CheckPasswordValid(Card card, int password);
        int GetCardTransactionsCount(Card card);
        Card? InsertCard(string cardId);
        void UpdateCardBalance(Card cardData);
        Card ReturnCard();
        void SaveTransaction(Transaction transaction);
        List<Transaction> GetFiveLastTransactions(Card card);
    }
}
using Bankomatas.Models;

namespace Bankomatas.Services
{
    internal interface IBankFileService
    {
        List<Card> GetAllCardData();
        Card GetCardData(Guid cardId);
        List<Transaction> GetCardTransactions(Guid cardId);
        void SaveCardData(List<Card> cards);
        void SaveTransaction(Transaction transaction);
    }
}
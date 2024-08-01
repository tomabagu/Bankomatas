using Bankomatas.Models;

namespace Bankomatas.Services
{
    internal interface IAtmService
    {
        void DepositMoney(Card card, int amount);
        int PinInputThreeTries(BankAccountService bankAccountService, Card card, int tries, int pinInput, out bool valid);
        Dictionary<int, int> ReadDenominationATM();
        decimal ShowBalance(Card card);
        void ShowLastFiveTransactions(Card card);
        void ShowMenu(Card card);
        void WithdrawMoney(Card card, int amount);
    }
}
using Bankomatas.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bankomatas.Services
{
    internal class AtmService : IAtmService
    {
        private readonly string _filePathMoneyDenominationATM;
        private BankAccountService bankAccountService;

        public AtmService(string filePathMoneyDenominationATM, BankAccountService bankAccountService)
        {
            _filePathMoneyDenominationATM = filePathMoneyDenominationATM;
            this.bankAccountService = bankAccountService;
        }

        public void ShowMenu(Card card)
        {
            int pasirinkimasInt = 0;
            while (pasirinkimasInt != 5)
            {
                Console.Clear();
                Console.WriteLine($"Turima pinigų suma {card.Balance}");
                Console.WriteLine();
                Console.WriteLine("1. Parodyti balansą");
                Console.WriteLine("2. Parodyti 5 paskutines transakcijas");
                Console.WriteLine("3. Išsiimti pinigų sumą");
                Console.WriteLine("4. Įdėti pinigų");
                Console.WriteLine("5. Grąžinti kortelę");
                do
                {
                    Console.WriteLine("Pasirinkite meniu punktą 1-5");
                } while (!int.TryParse(Console.ReadLine(), out pasirinkimasInt) || pasirinkimasInt < 1 || pasirinkimasInt > 5);
                switch (pasirinkimasInt)
                {
                    case 1:
                        Console.WriteLine(ShowBalance(card)); //sąskaitos likučio atvaizdavimas
                        Console.ReadKey();
                        break;
                    case 2:
                        ShowLastFiveTransactions(card); //paskutinės 5 transakcijos
                        Console.ReadKey();
                        break;
                    case 3:
                        int amount = MoneyInputFromKeyboard("išsiimti pinigų");
                        WithdrawMoney(card, amount); //išsiimti pinigus
                        Console.ReadKey();
                        break;
                    case 4:
                        int depositAmount = MoneyInputFromKeyboard("įdėti pinigų");
                        DepositMoney(card, depositAmount);
                        Console.ReadKey();
                        break;
                    case 5://išsiimti kortelę
                        pasirinkimasInt = 5;
                        break;
                }
            }
        }

        //Patikriname ar slaptažodis sutampa ir jeigu ne skaitliuką sumažiname ir grąžiname false per out parametrą
        public int PinInputThreeTries(BankAccountService bankAccountService, Card card, int tries, int pinInput, out bool valid)
        {
            bool isValid = bankAccountService.CheckPasswordValid(card, pinInput);
            if (!isValid)
            {
                tries--;
            }
            valid = isValid;
            return tries;
        }

        //Tikriname ar įvesta pinigų suma teisingai (tik skaičiai)
        private int MoneyInputFromKeyboard(string action)
        {
            int amount = 0;
            do
            {
                Console.WriteLine($"Įveskite sumą kurią norite {action}");
            } while (!int.TryParse(Console.ReadLine(), out amount));
            return amount;
        }
        public decimal ShowBalance(Card card)
        {
            return card.Balance;
        }

        //pinigų išėmimas
        public void WithdrawMoney(Card card, int amount)
        {
            int transactionCount = bankAccountService.GetCardTransactionsCount(card); //išsitraukiame šiandienos transakcijų skaičių kortelės
            List<string> errorMessages = ValidAmountWithdrawn(card, amount, transactionCount, out bool valid);//patikriname ar galime išsiimti pinigus
            if (valid)
            {
                bankAccountService.SaveTransaction(new Transaction(card.CardId, DateTime.Now, "Withdraw", amount)); //išsaugome transakcija
                card.Balance = card.Balance - amount;//atimame nuimtą pinigų sumą iš sąskaitos
                bankAccountService.UpdateCardBalance(card);//išsaugome atnaujintą sąskaitos likutį į failą
                Dispense(amount); //išimame kupiūras iš bankomato
            }
            else
            {
                errorMessages.ForEach(Console.WriteLine);
            }
        }

        public void DepositMoney(Card card, int amount)
        {
            int transactionCount = bankAccountService.GetCardTransactionsCount(card); //išsitraukiame šiandienos transakcijų skaičių kortelės
            List<string> errorMessages = ValidAmountDeposit(card, amount, transactionCount, out bool valid);//patikriname ar galime išsiimti pinigus
            if (valid)
            {
                bankAccountService.SaveTransaction(new Transaction(card.CardId, DateTime.Now, "Deposit", amount)); //išsaugome transakcija
                card.Balance = card.Balance + amount;//pridedame pinigų sumą į sąskaitą
                bankAccountService.UpdateCardBalance(card);//išsaugome atnaujintą sąskaitos likutį į failą
                CollectDenomination(amount); //įdedame kupiūras į bankomatą
            }
            else
            {
                errorMessages.ForEach(Console.WriteLine);
            }
        }

        //validacijos
        private List<string> ValidAmountWithdrawn(Card card, int amount, int transactionCount, out bool valid)
        {
            List<string> errorMessages = new List<string>();
            valid = true;
            if (transactionCount > 9)
            {
                valid = false;
                errorMessages.Add($"Viršytas dienos transakcijų limitas {transactionCount}");
            }
            if (amount < 5)
            {
                valid = false;
                errorMessages.Add($"Įveskite sumą ne mažiau kaip 5 eur");
            }
            if (card.Balance < amount)
            {
                valid = false;
                errorMessages.Add($"Įvesta išėmimo suma viršyja sąskaitos likutį {amount} > {card.Balance}");
            }
            if (amount % 5 != 0)
            {
                valid = false;
                errorMessages.Add($"Bankomatas išduoda tik kupiūras, todėl suma turi dalintis iš 5");
            }
            if (!CanDispense(amount))
            {
                valid = false;
                errorMessages.Add($"Bankomatas neturi pakankamai kupiūrų, kad išduotų prašomą sumą");
            }
            if (amount>1000)
            {
                valid=false;
                errorMessages.Add($"Viršytas pinigų išėmimo limitas, maksimumas 1000 eurų");
            }
            return errorMessages;
        }

        private List<string> ValidAmountDeposit(Card card, int amount, int transactionCount, out bool valid) //pinigų įdėjjimo validacija
        {
            List<string> errorMessages = new List<string>();
            valid = true;
            if (transactionCount > 9)
            {
                valid = false;
                errorMessages.Add($"Viršytas dienos transakcijų limitas {transactionCount}");
            }
            if (amount < 5)
            {
                valid = false;
                errorMessages.Add($"Įveskite sumą ne mažiau kaip 5 eur");
            }

            if (amount % 5 != 0)
            {
                valid = false;
                errorMessages.Add($"Bankomatas priima tik kupiūras, todėl suma turi dalintis iš 5");
            }
            return errorMessages;
        }

        //išsitraukiame paskutines 5 transakcijas ir su foreach atvaizduojame konsolėje
        public void ShowLastFiveTransactions(Card card)
        {
            List<Transaction> transactions = bankAccountService.GetFiveLastTransactions(card);
            transactions.ForEach(transaction =>
            {
                Console.WriteLine($"{transaction.Date.ToString("yyyy-MM-dd HH:mm:ss")} {transaction.Action} {transaction.Amount}");
            });
        }

        // nuskaitome esančias kupiūras bankomate (iš failo)
        public Dictionary<int, int> ReadDenominationATM()
        {
            string[] lines;
            try
            {
                lines = File.ReadAllLines(_filePathMoneyDenominationATM, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            Dictionary<int, int> availableDenomination = new Dictionary<int, int>();

            foreach (var denomination in lines)
            {
                var data = denomination.Split(',');
                availableDenomination.Add(int.Parse(data[0]), int.Parse(data[1]));
            }
            return availableDenomination;
        }

        //patikrinimas ar bankomatas su turimomis kupiūromis gali išduoti pinigus
        private bool CanDispense(int amount)
        {
            int[] denominations = { 500, 200, 100, 50, 20, 10, 5 };
            Dictionary<int, int> availableDenominations = ReadDenominationATM();
            foreach (var denomination in denominations)
            {
                int numBills = amount / denomination;
                if (numBills > 0)
                {
                    if (availableDenominations[denomination] >= numBills)
                    {
                        amount -= numBills * denomination;
                        availableDenominations[denomination] -= numBills;
                    }
                    else
                    {
                        amount -= availableDenominations[denomination] * denomination;
                        availableDenominations[denomination] = 0;
                    }
                }
            }
            return amount == 0;
        }

        //bankomatas išduoda kupiūras ir faile išsaugo atnaujintus kupiūrų duomenis
        private void Dispense(int amount)
        {
            int[] denominations = { 500, 200, 100, 50, 20, 10, 5 };
            Dictionary<int, int> availableDenominations = ReadDenominationATM();
            foreach (var denomination in denominations)
            {
                int numBills = amount / denomination;
                if (numBills > 0)
                {
                    if (availableDenominations[denomination] >= numBills)
                    {
                        amount -= numBills * denomination;
                        availableDenominations[denomination] -= numBills;
                    }
                    else
                    {
                        amount -= availableDenominations[denomination] * denomination;
                        availableDenominations[denomination] = 0;
                    }
                }
            }
            WriteDenominationATM(availableDenominations);
        }

        private void CollectDenomination(int amount)
        {
            int[] denominations = { 500, 200, 100, 50, 20, 10, 5 };
            Dictionary<int, int> availableDenominations = ReadDenominationATM();
            foreach (var denomination in denominations)
            {
                int numBills = amount / denomination;
                if (numBills > 0)
                {
                    amount -= numBills * denomination;
                    availableDenominations[denomination] += numBills;
                }
            }

            WriteDenominationATM(availableDenominations);
        }

        private void WriteDenominationATM(Dictionary<int, int> availableDenominations)
        {
            List<string> denominations = new List<string>();
            foreach (var denomination in availableDenominations)
            {
                denominations.Add($"{denomination.Key},{denomination.Value}");
            }
            File.WriteAllLines(_filePathMoneyDenominationATM, denominations.ToArray());
        }


    }
}

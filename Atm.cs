using Bankomatas.Models;
using Bankomatas.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bankomatas
{
    internal class Atm
    {
        public void UseAtm()
        {
            Console.WriteLine("Please insert your card");
            BankFileService bankFileService = new BankFileService("../../../CardsInfo.txt", "../../../Transactions.txt");
            BankAccountService bankAccountService = new BankAccountService(bankFileService); //dependency injection, kai per konstruktorių yra perduodamas kitas servisas
            Card? card = bankAccountService.InsertCard(Console.ReadLine());
            if (card == null)
            {
                Console.WriteLine("Invalid card id");
            }
            else
            {
                int tries = 3;
                bool valid = false;
                AtmService atmService = new AtmService("../../../MoneyDenominationATM.txt",bankAccountService); //dependency injection, kai per konstruktorių yra perduodamas kitas servisas
                do
                {
                    Console.WriteLine("Enter PIN number");
                    int pinInput = PinInputFromKeyboard(); //įvestis iš klaviatūros
                    tries = atmService.PinInputThreeTries(bankAccountService, card, tries, pinInput, out valid);
                    if (!valid)
                    {
                        Console.WriteLine($"Bad password (tries left {tries})");
                    }
                }
                while (tries != 0 && !valid); //do while ciklas kuris iteruoja kol nėra įvedamas teisingas PIN arba max 3 bandymai
                if (valid)
                {
                    atmService.ShowMenu(card);
                }
                else
                {
                    Console.WriteLine("Exit");
                }
            }
        }
        //ivestis iš klaviatūros su patikrinimu ar įvestas skaičius
        private static int PinInputFromKeyboard()
        {
            bool validInput;
            int pinInput = 0;
            do
            {
                validInput = int.TryParse(Console.ReadLine(), out pinInput);
            } while (!validInput);
            return pinInput;
        }

    }
}

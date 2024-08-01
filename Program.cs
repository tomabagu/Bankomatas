using Bankomatas.Services;
using Bankomatas.Models;
using System.ComponentModel;

namespace Bankomatas
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Atm atm = new Atm();
            atm.UseAtm();
        }
    }
}

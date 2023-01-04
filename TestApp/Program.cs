
using TaxCalculatorInterviewTests;

namespace TestProject
{
    public class Program
    {

        static void Main(string[] args)
        {
            TaxCalculator customer1 = new TaxCalculator();

            Console.WriteLine($"Customer 1 Default Tax for Transport: {customer1.GetCurrentTaxRate(Commodity.Transport)}");

            customer1.SetCustomTaxRate(Commodity.Transport, 12.5);
            customer1.SetCustomTaxRate(Commodity.Transport, 13.5);
            customer1.SetCustomTaxRate(Commodity.Transport, 14.5);

            Console.WriteLine($"Customer 1 Transport Tax: {customer1.GetCurrentTaxRate(Commodity.Transport)}");


            TaxCalculator customer2 = new TaxCalculator();

            Console.WriteLine($"Customer 2 Default Tax For Transport: {customer2.GetCurrentTaxRate(Commodity.Transport)}");

            customer2.SetCustomTaxRate(Commodity.Transport, 1.5);
            customer2.SetCustomTaxRate(Commodity.Transport, 3.5);
            customer2.SetCustomTaxRate(Commodity.Transport, 20.5);

            Console.WriteLine($"Customer 2 Transport Tax: {customer2.GetCurrentTaxRate(Commodity.Transport)}");



        }
    }
}

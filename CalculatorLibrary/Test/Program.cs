using CalculatorLibrary;
using CalculatorLibrary.Element_Number;

namespace Test
{
    internal class Program
    {
        static void Main()
        {
            Mathematics mathematics = new();
            INumber number = mathematics.GetNumber("[lol]-2+5+(5*2)+1");
            Console.WriteLine(number.ToString());
            Console.WriteLine(number.Get());
            Console.WriteLine("End");
            Console.ReadLine();
        }
    }
}

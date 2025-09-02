using System.Reflection.Metadata;
using System.Threading.Channels;

namespace Stage0
{
    partial class Program
    {
        private static void Main(string[] args)
        {
            Welcome3271();
            Welcome2635();
            Console.ReadKey();

        }
        static partial void Welcome2635();
        private static void Welcome3271()
        {
            Console.WriteLine("Enter your name:");
            string? userName = Console.ReadLine();
            Console.WriteLine("{0},welcome to my first console application", userName);
        }
    }
}
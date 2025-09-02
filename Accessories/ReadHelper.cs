using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Accessories
{
    public class ReadHelper
    {
        public static int ReadInt(string message)
        {
            Console.WriteLine(message);
            string? numInput = Console.ReadLine();
            int number;
            while (!int.TryParse(numInput, out number))
            {
                Console.WriteLine("The value you entered is invalid. Please enter it again!");
                numInput = Console.ReadLine();
            }
            return number;

        }
        public static double ReadDouble(string message)
        {
            Console.WriteLine(message);
            string? ageInput = Console.ReadLine();
            double data;
            while (!double.TryParse(ageInput, out data))
            {
                Console.WriteLine("The value you entered is invalid. Please enter it again!");
                ageInput = Console.ReadLine();
            }
            return data;

        }
        public static string ReadString(string message)
        {
            Console.WriteLine(message);
            string? ageInput = Console.ReadLine();
            while (ageInput is null)
            {
                ageInput = Console.ReadLine();
            }
            return ageInput;

        }

        public static DateTime ReadDate(string message)
        {
            Console.WriteLine(message);
            string? dateInput = Console.ReadLine();
            DateTime data;
            while (!DateTime.TryParse(dateInput, out data))
            {
                Console.WriteLine("The value you entered is invalid. Please enter it again!");
                dateInput = Console.ReadLine();
            }
            return data;

        }
        public static bool ReadBool(string message)
        {
            Console.WriteLine(message);
            string? dateInput = Console.ReadLine();
            bool data;
            while (!bool.TryParse(dateInput, out data))
            {
                Console.WriteLine("The value you entered is invalid. Please enter it again!");
                dateInput = Console.ReadLine();
            }
            return data;

        }
        public static bool? ReadBoolNull(string message) 
        {
            Console.WriteLine(message);

            string? dataInput = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(dataInput))
                return null;
            bool data;
            while (!bool.TryParse(dataInput, out data)&&dataInput!="")
            {
                Console.WriteLine("The value you entered is invalid. Please enter it again!");
                dataInput = Console.ReadLine();
            }
            return data;

        }
        public static T? ReadEnumNull<T>(string message) where T : struct, Enum
        {
            Console.WriteLine(message);
            string? enumInput = Console.ReadLine();

            // אם הקלט ריק, נחזיר null מיד
            if (string.IsNullOrWhiteSpace(enumInput))
                return null;
            object? data;
            while (!Enum.TryParse(typeof(T), enumInput, out data)&& enumInput!="")
            {
                Console.WriteLine("The value you entered is invalid. Please enter it again!");
                enumInput = Console.ReadLine();
            }
            return data==null?null:(T)data;

        }


        public static T ReadEnum<T>(string message)
        {
            Console.WriteLine(message);
            string? enumInput = Console.ReadLine();
            object? data;
            while (!Enum.TryParse(typeof(T), enumInput, out data))
            {
                Console.WriteLine("The value you entered is invalid. Please enter it again!");
                enumInput = Console.ReadLine();
            }
            return (T)data;

        }
        public static string ReadOrDefault(string? input, string defaultValue)
        {
            return string.IsNullOrWhiteSpace(input) ? defaultValue : input;
        }
    }
}
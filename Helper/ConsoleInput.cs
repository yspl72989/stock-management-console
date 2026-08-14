using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockCli.Helper
{
    public static class ConsoleInput
    {
        public static string ReadRequiredString(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);

                var input = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(input))
                {
                    return input.Trim();
                }

                Console.WriteLine("Input cannot be empty");
            }
        }
        public static int ReadPositiveInt(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                var input = Console.ReadLine();

                if (int.TryParse(
                    input,
                    NumberStyles.Integer,
                    CultureInfo.InvariantCulture,
                    out var value)
                    && value > 0)
                {
                    return value;
                }

                Console.WriteLine("Please enter a whole number greater than zero.");
            }
        }

        public static decimal ReadPositiveDecimal(string prompt)
        {
            while (true)
            {
                Console.WriteLine(prompt);
                var input = Console.ReadLine();

                if (decimal.TryParse(
                    input,
                    NumberStyles.Number,
                    CultureInfo.InvariantCulture,
                    out var value)
                    && value > 0)
                {
                    return value;
                }
                Console.WriteLine(
                    "Please enter a number greater than zero"
                    );



            }
        }

        public static decimal ReadQuantity(string unit)
        {
            while (true)
            {
                Console.Write($"Quantity({unit}): ");

                var input = Console.ReadLine();

                if (!decimal.TryParse(
                    input,
                    NumberStyles.Number,
                    CultureInfo.InvariantCulture,
                    out var quantity)
                    )
                {
                    Console.WriteLine(
                        "Please enter a valid number."
                        );
                    continue;
                }
                if (quantity <= 0)
                {
                    Console.WriteLine(
                        "Quantity must be greater than zero.");
                    continue;
                }

                if (unit is "Bag" or "Piece"
                    && quantity != decimal.Truncate(quantity))
                {
                    Console.WriteLine(
                        $"{unit} quantity must be a whole number.");

                    continue;
                }

                return quantity;
            }
        }

        public static string ReadUnit()
        {
            while (true)

            {
                Console.WriteLine();
                Console.WriteLine("Select unit:");
                Console.WriteLine("1. Kg");
                Console.WriteLine("2. Bag");
                Console.WriteLine("3. Piece");
                Console.WriteLine("Choose unit: ");

                var input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        return "Kg";
                    case "2":
                        return "Bag";
                    case "3":
                        return "Piece";
                    default:
                        Console.WriteLine(
                            "Invalid unit. Please choose 1,2 or 3.");
                        break;
                }


            }
        }

        public static bool ReadYesNo(string promt)
        {
            while (true)
            {
                Console.WriteLine($"{promt}(Y/N): ");

                var input = Console.ReadLine()
                    ?.Trim()
                    .ToUpperInvariant();
                if (input == "Y")
                {
                    return true;
                }
                if (input == "N")
                {
                    return false;
                }
                Console.WriteLine("Please enter Y or N.");
            }
        }

    }
}




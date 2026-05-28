using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class Program
{
    static void Main(string[] args)
    {
        Inventory inventory = new Inventory();

        bool running = true;

        while (running)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n====================================");
            Console.WriteLine("     INVENTORY MANAGEMENT SYSTEM");
            Console.WriteLine("====================================");
            Console.ResetColor();

            Console.WriteLine("1. Add Product");
            Console.WriteLine("2. Update Product");
            Console.WriteLine("3. Delete Product");
            Console.WriteLine("4. View All Products");
            Console.WriteLine("5. Generate Report");
            Console.WriteLine("6. Exit");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Choose an option: ");
            Console.ResetColor();

            string choice = Console.ReadLine()?.Trim() ?? "";

            Console.WriteLine();

            if (choice.Equals("exit", StringComparison.OrdinalIgnoreCase))
            {
                running = false;

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Exiting application...");
                Console.ResetColor();

                continue;
            }

            switch (choice)
            {
                case "1":
                    inventory.AddProduct();
                    break;

                case "2":
                    inventory.UpdateProduct();
                    break;

                case "3":
                    inventory.DeleteProduct();
                    break;

                case "4":
                    inventory.ViewProducts();
                    break;

                case "5":
                    inventory.GenerateReport();
                    break;

                case "6":
                    running = false;

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Exiting application...");
                    Console.ResetColor();
                    break;

                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid choice. Please choose between 1 and 6.");
                    Console.ResetColor();
                    break;
            }
        }
    }
}

using System.Text;
using System.Text.Json;
using System.IO;
using System.Linq;

public class Inventory
{
    private List<Product> products;
    private readonly string filePath = "inventory.json";

    public Inventory()
    {
        products = new List<Product>();
        LoadFromFile();
    }

    public void AddProduct()
    {
        ShowHeading("Add Product");
        ShowExitInstruction();

        int? id = ReadInt("Enter product ID: ");

        if (id == null)
        {
            ShowError("Returning to main menu.");
            return;
        }

        Product? existingProduct = products.FirstOrDefault(p => p.ID == id.Value);

        if (existingProduct != null)
        {
            ShowError("A product with this ID already exists.");
            return;
        }

        string? name = ReadInput("Enter product name: ");

        if (name == null)
        {
            ShowError("Returning to main menu.");
            return;
        }

        int? quantity = ReadInt("Enter quantity: ");

        if (quantity == null)
        {
            ShowError("Returning to main menu.");
            return;
        }

        decimal? price = ReadDecimal("Enter price: ");

        if (price == null)
        {
            ShowError("Returning to main menu.");
            return;
        }

        Product product = new Product(id.Value, name, quantity.Value, price.Value);
        products.Add(product);

        SaveToFile();

        ShowSuccess("Product added successfully.");
    }

    public void UpdateProduct()
    {
        ShowHeading("Update Product");
        ShowExitInstruction();

        int? id = ReadInt("Enter product ID to update: ");

        if (id == null)
        {
            ShowError("Returning to main menu.");
            return;
        }

        Product? product = products.FirstOrDefault(p => p.ID == id.Value);

        if (product == null)
        {
            ShowError("Product not found.");
            return;
        }

        Console.WriteLine("\nCurrent product information:");
        product.DisplayProductInfo();

        Console.WriteLine("Press Enter to keep the current value.");

        string? newName = ReadOptionalString($"Enter new name ({product.Name}): ", product.Name);

        if (newName == null)
        {
            ShowError("Returning to main menu.");
            return;
        }

        int? newQuantity = ReadOptionalInt($"Enter new quantity ({product.Quantity}): ", product.Quantity);

        if (newQuantity == null)
        {
            ShowError("Returning to main menu.");
            return;
        }

        decimal? newPrice = ReadOptionalDecimal($"Enter new price ({product.Price:C}): ", product.Price);

        if (newPrice == null)
        {
            ShowError("Returning to main menu.");
            return;
        }

        Console.WriteLine("\n--- Changes to be applied ---");
        Console.WriteLine($"ID: {product.ID}");
        Console.WriteLine($"Name: {product.Name} -> {newName}");
        Console.WriteLine($"Quantity: {product.Quantity} -> {newQuantity.Value}");
        Console.WriteLine($"Price: {product.Price:C} -> {newPrice.Value:C}");
        Console.WriteLine($"Total Value: {(product.Quantity * product.Price):C} -> {(newQuantity.Value * newPrice.Value):C}");

        bool? confirm = ConfirmAction("Apply these changes");

        if (confirm == null)
        {
            ShowError("Returning to main menu.");
            return;
        }

        if (confirm == false)
        {
            ShowError("Update cancelled.");
            return;
        }

        product.Name = newName;
        product.Quantity = newQuantity.Value;
        product.Price = newPrice.Value;

        SaveToFile();

        ShowSuccess("Product updated successfully.");
    }

    public void DeleteProduct()
    {
        ShowHeading("Delete Product");
        ShowExitInstruction();

        int? id = ReadInt("Enter product ID to delete: ");

        if (id == null)
        {
            ShowError("Returning to main menu.");
            return;
        }

        Product? product = products.FirstOrDefault(p => p.ID == id.Value);

        if (product == null)
        {
            ShowError("Product not found.");
            return;
        }

        Console.WriteLine("\nProduct found:");
        product.DisplayProductInfo();

        bool? confirm = ConfirmAction("Are you sure you want to delete this product");

        if (confirm == null)
        {
            ShowError("Returning to main menu.");
            return;
        }

        if (confirm == false)
        {
            ShowError("Deletion cancelled.");
            return;
        }

        products.Remove(product);
        SaveToFile();

        ShowSuccess("Product deleted successfully.");
    }

    private bool? ConfirmAction(string message)
    {
        while (true)
        {
            string? input = ReadInput($"{message} (y/n): ");

            if (input == null)
            {
                return null;
            }

            input = input.Trim().ToLower();

            if (input == "y" || input == "yes")
            {
                return true;
            }

            if (input == "n" || input == "no")
            {
                return false;
            }

            ShowError("Invalid choice. Please enter y or n.");
        }
    }

    public void ViewProducts()
    {
        ShowHeading("Product List");

        if (products.Count == 0)
        {
            ShowError("No products in inventory.");
            return;
        }

        Console.WriteLine("{0,-5} {1,-20} {2,-10} {3,-12} {4,-12}",
            "ID", "Name", "Quantity", "Price", "Total Value");

        ShowLine();

        foreach (Product product in products)
        {
            Console.Write("{0,-5} {1,-20} ",
                product.ID,
                product.Name);

            WriteColoredQuantity(product.Quantity, 10);

            Console.WriteLine(" {0,-12:C} {1,-12:C}",
                product.Price,
                product.Quantity * product.Price);
        }
    }

    public void GenerateReport()
    {
        ShowHeading("Inventory Report");

        if (products.Count == 0)
        {
            ShowError("No products available.");
            return;
        }

        decimal totalInventoryValue = products.Sum(p => p.Quantity * p.Price);
        int totalQuantity = products.Sum(p => p.Quantity);

        Console.WriteLine($"Total products: {products.Count}");
        Console.WriteLine($"Total quantity in stock: {totalQuantity}");
        Console.WriteLine($"Total inventory value: {totalInventoryValue:C}");

        Console.WriteLine();

        Console.WriteLine("{0,-5} {1,-20} {2,-10} {3,-12} {4,-12}",
            "ID", "Name", "Quantity", "Price", "Total Value");

        ShowLine();

        foreach (Product product in products)
        {
            Console.Write("{0,-5} {1,-20} ",
                product.ID,
                product.Name);

            WriteColoredQuantity(product.Quantity, 10);

            Console.WriteLine(" {0,-12:C} {1,-12:C}",
                product.Price,
                product.Quantity * product.Price);
        }

        Console.WriteLine();

        bool? saveReport = ConfirmAction("Do you want to save this report to a file");

        if (saveReport == null)
        {
            ShowError("Returning to main menu.");
            return;
        }

        if (saveReport == true)
        {
            string reportText = BuildReportText();
            SaveReportToFile(reportText);
        }
        else
        {
            ShowError("Report was not saved.");
        }
    }

    private void SaveToFile()
    {
        string json = JsonSerializer.Serialize(products, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        File.WriteAllText(filePath, json);
    }

    private void LoadFromFile()
    {
        if (!File.Exists(filePath))
        {
            products = new List<Product>();
            return;
        }

        string json = File.ReadAllText(filePath);

        if (string.IsNullOrWhiteSpace(json))
        {
            products = new List<Product>();
            return;
        }

        products = JsonSerializer.Deserialize<List<Product>>(json) ?? new List<Product>();
    }

    private int? ReadInt(string message)
    {
        int value;

        while (true)
        {
            string? input = ReadInput(message);

            if (input == null)
            {
                return null;
            }

            if (int.TryParse(input, out value) && value >= 0)
            {
                return value;
            }

            ShowError("Invalid input. Please enter a valid whole number.");
        }
    }

    private decimal? ReadDecimal(string message)
    {
        decimal value;

        while (true)
        {
            string? input = ReadInput(message);

            if (input == null)
            {
                return null;
            }

            if (decimal.TryParse(input, out value) && value >= 0)
            {
                return value;
            }

            ShowError("Invalid input. Please enter a valid price.");
        }
    }
    private void ShowSuccess(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    private void ShowError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    private void ShowQuestion(string message)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write(message);
        Console.ResetColor();
    }

    private void ShowHeading(string heading)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"\n=== {heading} ===");
        Console.ResetColor();
    }

    private void ShowLine()
    {
        Console.WriteLine(new string('-', 70));
    }
    private string? ReadInput(string message)
    {
        ShowQuestion(message);

        string input = Console.ReadLine() ?? "";

        if (input.Trim().Equals("exit", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        return input;
    }
    private void ShowExitInstruction()
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("Type \"exit\" at any time to return to the main menu.");
        Console.ResetColor();
    }
    private string? ReadOptionalString(string message, string currentValue)
    {
        ShowQuestion(message);

        string input = Console.ReadLine() ?? "";

        if (input.Trim().Equals("exit", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(input))
        {
            return currentValue;
        }

        return input;
    }
    private int? ReadOptionalInt(string message, int currentValue)
    {
        int value;

        while (true)
        {
            ShowQuestion(message);

            string input = Console.ReadLine() ?? "";

            if (input.Trim().Equals("exit", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(input))
            {
                return currentValue;
            }

            if (int.TryParse(input, out value) && value >= 0)
            {
                return value;
            }

            ShowError("Invalid input. Please enter a valid whole number.");
        }
    }
    private decimal? ReadOptionalDecimal(string message, decimal currentValue)
    {
        decimal value;

        while (true)
        {
            ShowQuestion(message);

            string input = Console.ReadLine() ?? "";

            if (input.Trim().Equals("exit", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(input))
            {
                return currentValue;
            }

            if (decimal.TryParse(input, out value) && value >= 0)
            {
                return value;
            }

            ShowError("Invalid input. Please enter a valid price.");
        }
    }
    private void WriteColoredQuantity(int quantity, int width)
    {
        if (quantity == 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
        }
        else if (quantity < 10)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Green;
        }

        Console.Write(quantity.ToString().PadRight(width));

        Console.ResetColor();
    }
    private void SaveReportToFile(string reportText)
    {
        string fileName = $"InventoryReport_{DateTime.Now:yyyyMMdd_HHmmss}.txt";

        File.WriteAllText(fileName, reportText);

        ShowSuccess($"Report saved successfully as {fileName}");
    }
    private string BuildReportText()
    {
        decimal totalInventoryValue = products.Sum(p => p.Quantity * p.Price);
        int totalQuantity = products.Sum(p => p.Quantity);

        StringBuilder report = new StringBuilder();

        report.AppendLine("=== Inventory Report ===");
        report.AppendLine($"Generated: {DateTime.Now}");
        report.AppendLine();
        report.AppendLine($"Total products: {products.Count}");
        report.AppendLine($"Total quantity in stock: {totalQuantity}");
        report.AppendLine($"Total inventory value: {totalInventoryValue:C}");
        report.AppendLine();

        report.AppendLine(string.Format("{0,-5} {1,-20} {2,-10} {3,-12} {4,-12}",
            "ID", "Name", "Quantity", "Price", "Total Value"));

        report.AppendLine(new string('-', 70));

        foreach (Product product in products)
        {
            report.AppendLine(string.Format("{0,-5} {1,-20} {2,-10} {3,-12:C} {4,-12:C}",
                product.ID,
                product.Name,
                product.Quantity,
                product.Price,
                product.Quantity * product.Price));
        }

        return report.ToString();
    }
}
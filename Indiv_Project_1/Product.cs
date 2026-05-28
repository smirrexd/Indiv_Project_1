public class Product
{
    public int ID { get; set; }
    public string Name { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }

    public Product()
    {
        Name = "";
    }

    public Product(int id, string name, int quantity, decimal price)
    {
        ID = id;
        Name = name;
        Quantity = quantity;
        Price = price;
    }

    public void DisplayProductInfo()
    {
        Console.WriteLine("--------------------------------");
        Console.WriteLine($"ID:          {ID}");
        Console.WriteLine($"Name:        {Name}");
        Console.WriteLine($"Quantity:    {Quantity}");
        Console.WriteLine($"Price:       {Price:C}");
        Console.WriteLine($"Total Value: {(Quantity * Price):C}");
        Console.WriteLine("--------------------------------");
    }
}
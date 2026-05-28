# Inventory Management System

## Project Overview

This is a C# Console Application for managing inventory in a small business.  
The application allows the user to add, update, delete, view, and report products.

The system uses a JSON file to save and load product data, which means that the inventory is stored even after the application is closed.

---

## Features

- Add new products
- Update existing products
- Delete products
- View all products
- Generate inventory reports
- Save reports to a text file
- Store product data in a JSON file
- User-friendly console colors
- Confirmation before updating or deleting products
- Option to type `exit` at any time to return to the main menu

---

## Technologies Used

- C#
- .NET Console Application
- JSON file handling
- `System.Text.Json`
- File handling with `File.ReadAllText()` and `File.WriteAllText()`

---

## Project Structure

```text
InventoryManagementSystem
│
├── Program.cs
├── Product.cs
├── Inventory.cs
├── inventory.json
└── README.md

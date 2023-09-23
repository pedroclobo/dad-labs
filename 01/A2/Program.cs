using System;

class Program
{
    static void Main(string[] args)
    {
        IListaNomes nameManager = new NameManager();

        while (true)
        {
            Console.WriteLine("Menu:");
            Console.WriteLine("1. Add Name");
            Console.WriteLine("2. List Names");
            Console.WriteLine("3. Erase List");
            Console.WriteLine("4. Quit");

            Console.Write("Choose an option: ");
            string option = Console.ReadLine();

            switch (option)
            {
                case "1":
                    Console.Write("Type the name to be added: ");
                    string nome = Console.ReadLine();
                    nameManager.AddName(nome);
                    Console.WriteLine($"Name '{nome}' was added to the list.");
                    break;
                case "2":
                    Console.WriteLine("Names in the list:");
                    Console.WriteLine(nameManager.GetNames());
                    break;
                case "3":
                    nameManager.EraseNames();
                    Console.WriteLine("The name's list has been cleared.");
                    break;
                case "4":
                    Console.WriteLine("Quitting.");
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }

            Console.WriteLine();
        }
    }
}

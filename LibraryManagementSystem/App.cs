public class App{

    public void ShowMenu(){
        Console.WriteLine("\nChoose the action you want to do: ");
        Console.WriteLine("1. Display Book Record");
        Console.WriteLine("2. Borrow Book");
        Console.WriteLine("3. Display Borrower Record");
        Console.WriteLine("4. Return Book");
        Console.WriteLine("5. Exit the application");

        PerformFunctions(Console.ReadLine());
    }

    public void PerformFunctions(string selectedAction){
        //initialise the lists by getting details beforehand
        Utilities.GetBooks();

        switch (selectedAction){
            case "1":
                Utilities.DisplayBooks();
                break;
            case "2":
                Utilities.BorrowBooks();
                break;
            case "3":
                Utilities.DisplayBorrowerDetails();
                break;
            case "4":
                Utilities.ReturnBooks();
                break;
            case "5":
                Console.WriteLine("Thank you for your presence in our library");
                Environment.Exit(0);
                break;
            default:
                Console.WriteLine("Please enter a valid choice from 1-5");
                break;
        }
        ShowMenu();
    }
}
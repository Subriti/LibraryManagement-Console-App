using System.ComponentModel.DataAnnotations;
using System.Text.Json;

public static class Utilities
{
    static List<string[]> list = new List<string[]>();

    static List<string> bookNames = new List<string>();
    static List<string> bookCost = new List<string>();

    public static void GetBooks()
    {
        string booksFilePath = GetBooksFilePath();
        if (!File.Exists(booksFilePath))
        {
            Console.WriteLine("No Books in the Library.");
        }

        var data = File.ReadAllText(booksFilePath);

        //splitting each line into a list
        var eachBook = data.Split("\n");

        for (int x = 0; x < eachBook.Length; x++)
        {
            //extracting each attribute into a list
            var bookDetail = eachBook[x].Split(',');

            //stored into a global variable
            list.Add(bookDetail);
        }
    }

    public static void DisplayBooks()
    {
        Console.WriteLine("\n \t\t|| Detail of Books available in our store ||");
        Console.WriteLine("_______________________________________________________________________________");
        Console.WriteLine("\nBOOK NAME\t\tAUTHOR NAME\t\tQUANTITY\tPRICE\n");

        //extracting individual book detail
        foreach (var bookDetail in list)
        {
            for (int i = 0; i < bookDetail.Length; i++)
            {
                Console.Write(bookDetail[i] + "\t\t");
            }
            //new line for each book detail
            Console.WriteLine("");
        }           
    }

    public static void BorrowBooks()
    {
        Console.Write("Input your Name: ");
        string borrowerName = Console.ReadLine();

        if (borrowerName.All(char.IsLetter))
        {
            string borrowersFilePath = GetBorrowersFilePath(borrowerName);

            //display books
            DisplayBookNames();

            //getting number of books for borrow
            int bookQuantity;
            int errorCounter = 0;
            List<string> bookName = new List<string>();
            int count = 0;

            Console.Write("\n Enter number of books you want to borrow: ");
            if (int.TryParse(Console.ReadLine(), out bookQuantity))
            {
                if (bookQuantity < 0)
                {
                    Console.WriteLine("Please input positive integer");
                }
                else
                {

                    for (int i = 0; i < bookQuantity; i++)
                    {
                        Console.WriteLine("\n Enter name of the book you would like to borrow: ");
                        var book = Console.ReadLine();

                        foreach (string name in bookName)
                        {
                            //checking for redundant entries
                            if (book == name)
                            {
                                Console.WriteLine("You can't borrow same book twice.");
                                errorCounter = 1;
                            }
                        }

                        if (errorCounter == 0)
                        {
                            //checking if the book entered exists in the library
                            if (!bookNames.Contains(book))
                            {
                                Console.WriteLine($"The Book {book} is not available in our library. Please type in properly.");
                                return;
                            }

                            //checking if quantity of book is available
                            for (int b = 0; b < list.Count; b++)
                            {
                                //matching book name
                                if (book == list[b][0])
                                {
                                    if (int.Parse(list[b][2]) == 0)
                                    {
                                        Console.WriteLine($"\n {book} is not available at the moment.\nPlease place an order for some other book :)");
                                        errorCounter = 1;
                                    }
                                    else
                                    {
                                        bookName.Add(book);

                                        //updating the stock

                                    }
                                }
                            }




                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("Please input valid integers");
            }


            //current date time
            DateTime dateTime = DateTime.Now;

            //for return date
            DateTime returnDateTime = dateTime.AddDays(10);

            //Writing Borrower Name and Date-Time of issue to the file
            string details = $"Borrowed By: {borrowerName}\n\nDate and Time of Issue: {dateTime}\nLast Date of Return: {returnDateTime}\n";
            Console.WriteLine(details);
            //File.WriteAllText(borrowersFilePath, details);

        }
        else
        {
            Console.WriteLine("Please input alphabet from: A-Z");
            BorrowBooks();
        }
    }

    public static void DisplayBorrowerDetails()
    {
        Console.WriteLine("Input Borrower's Name: ");
        string borrowerName = Console.ReadLine();

        try
        {
            string borrowersFilePath = GetBorrowersFilePath(borrowerName);
            var json = File.ReadAllText(borrowersFilePath);
            Console.WriteLine(json);
        }
        catch
        {
            Console.WriteLine("No details of the borrower could be found in the Library.");
        }
    }

    public static void ReturnBooks()
    {


    }

    //specifying the location to store the txt files
    public static string GetFilePath()
    {
        return @"C:\Users\Subriti\HelloWorldFromVSCode\";
    }

    //specifying the name and location for storing User data
    public static string GetBooksFilePath()
    {
        return Path.Combine(GetFilePath(), "LibraryBooks.txt");
    }

    //specifying the name and location for storing Items data
    public static string GetBorrowersFilePath(string borrowerName)
    {
        return Path.Combine(GetFilePath(), $"Borrower-{borrowerName.ToUpper()}.txt");
    }

    public static void UploadingWithStreamWriter()
    {
        // Create a string array with the lines of text
        string[] lines = { "First line", "Second line", "Third line" };

        // Set a variable to the Documents path.
        string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        // Write the string array to a new file named "WriteLines.txt".
        using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, "WriteLines.txt")))
        {
            foreach (string line in lines)
                outputFile.WriteLine(line);
        }
    }

    public static void WriteToFile(string details)
    {

    }

    public static void DisplayBookNames()
    {
        List<string> bookName = new List<string>();
        List<string> costs = new List<string>();

        Console.WriteLine("\n----- Books available in our store -----\n");

        //extracting individual book detail
        foreach (var bookDetail in list)
        {
           bookName.Add(bookDetail[0]);
           costs.Add(bookDetail[3].Trim('$'));
        }

        bookNames = bookName;
        bookCost = costs;

        //print booknames
        foreach (string a in bookName)
        {
            Console.WriteLine($"\t{a}");
        }
    }
}
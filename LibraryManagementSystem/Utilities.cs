using System.Text;
public static class Utilities
{
    static List<string[]> list = new List<string[]>();

    static List<string> bookNames = new List<string>();
    static List<string> bookCost = new List<string>();

    public static void GetBooks()
    {
        list.Clear();
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
            List<string> borrowBookName = new List<string>();
            int count = 0;
            float total = 0.0F;

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
                        Console.WriteLine("\nEnter name of the book you would like to borrow: ");
                        var book = Console.ReadLine();

                        foreach (string name in borrowBookName)
                        {
                            //checking for redundant entries for borrowing
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
                                        Console.WriteLine($"\n{book} is not available at the moment.\nPlease place an order for some other book :)");
                                        return;
                                    }
                                    else
                                    {
                                        //current date time
                                        DateTime dateTime = DateTime.Now;

                                        //for return date
                                        DateTime returnDateTime = dateTime.AddDays(10);

                                        //Writing Borrower Name and Date-Time of issue to the file
                                        string details = $"Borrowed By: {borrowerName}\n\nDate and Time of Issue: {dateTime}\nLast Date of Return: {returnDateTime}\n";
                                        File.WriteAllText(borrowersFilePath, details);

                                        borrowBookName.Add(book);

                                        //Writing to the Borrower's file the Borrowed Book'Name
                                        foreach (var name in borrowBookName)
                                        {
                                            details = $"\nName of the Book: {name}";
                                            File.AppendAllText(borrowersFilePath, details);
                                        }

                                        //updating the stock
                                        var stockInt = int.Parse(list[b][2]) - 1;
                                        list[b][2] = stockInt.ToString();
                                        Console.WriteLine(list[b][2]);

                                        //write to libraryBooks data
                                        WriteToFile();

                                        //for calculating cost
                                        for (int c = 0; c < bookCost.Count; c++)
                                        {
                                            if (book.ToUpper() == bookNames[c].ToUpper())
                                            {
                                                total += float.Parse(bookCost[c]);
                                            }
                                        }
                                        if (total != 0.0F && count == (bookQuantity - 1))
                                        {
                                            Console.WriteLine("\nYour total is $"+ total);
                                        }

                                        //Writing the cost of the book to the file
                                        details = $"\nYour total is: ${total}\n";

                                        Console.WriteLine(details);

                                        File.AppendAllText(borrowersFilePath, details);
                                    }
                                }
                            }
                        }
                    }
                    Console.WriteLine("\n ----- Thankyou for borrowing from us ! -----");
                }
            }
            else
            {
                Console.WriteLine("Please input valid integers");
            }
        }
        else
        {
            Console.WriteLine("Please input alphabet from: A-Z");
            BorrowBooks();
        }
    }

    public static string DisplayBorrowerDetails()
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

        return borrowerName;
    }
    /*
        //used for serialising object to json
        public static string ConvertToJSON()
        {
            string json = JsonConvert.SerializeObject(this);
            return json;
        }*/

    public static void ReturnBooks()
    {
        //check returner
        string returnerName = DisplayBorrowerDetails();

        //current date time
        DateTime dateTime = DateTime.Now;

        //Writing Borrower Name and Date-Time of return to the file
        string details = $"Borrowed By: {returnerName}\n\nDate and Time of Return: {dateTime}\n";

        File.WriteAllText(GetReturnersFilePath(returnerName), details);


        Console.WriteLine("Initialising the return.. Updating Stocks..");


        var json = File.ReadAllText(GetBorrowersFilePath(returnerName));
        var detail = json.Split(":");
        foreach (var d in detail)
        {
            if (d.Contains('$'))
            {
                details = $"\nThe total order: {d}\n";
                File.AppendAllText(GetReturnersFilePath(returnerName), details);
            }

            //searching for the book name in borrower record
            for (int b = 0; b < list.Count; b++)
            {
                if (d.Contains(list[b][0]))
                {
                    //Appending the book name
                    details = $"\nName of the Book: {list[b][0]}";
                    File.AppendAllText(GetReturnersFilePath(returnerName), details);

                    //updating the stock
                    var stockInt = int.Parse(list[b][2]) + 1;
                    list[b][2] = stockInt.ToString();

                    //write to libraryBooks data
                    WriteToFile();
                }
            }
        }
        Console.WriteLine("\nThe return was successful. Do visit again :)");
    }

    //specifying the location to store the txt files
    public static string GetFilePath()
    {
        return @"C:\Users\Subriti\HelloWorldFromVSCode\";
    }

    //specifying the name and location for storing Book data
    public static string GetBooksFilePath()
    {
        return Path.Combine(GetFilePath(), "LibraryBooks.txt");
    }

    //specifying the name and location for storing Borrower data
    public static string GetBorrowersFilePath(string borrowerName)
    {
        return Path.Combine(GetFilePath(), $"Borrower-{borrowerName.ToUpper()}.txt");
    }

    //specifying the name and location for storing Returner data
    public static string GetReturnersFilePath(string returnerName)
    {
        return Path.Combine(GetFilePath(), $"Returner-{returnerName.ToUpper()}.txt");
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

    public static void WriteToFile()
    {
        // reconstructing the LibraryBooks data for .txt file
        string books;
        StringBuilder builder = new StringBuilder();
        foreach (var name in list)
        {
            int ind = 0;
            foreach (var n in name)
            {
                builder.Append(n);
                ind++;
                if (ind < 4)
                {
                    builder.Append(",");
                }
            }
            builder.Append("\n");

            books = builder.ToString();

            //Writing the Stock-file with updated value of books
            File.WriteAllText(GetBooksFilePath(), books.TrimEnd());
        }
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
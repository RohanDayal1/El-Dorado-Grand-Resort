public static class BooksUpdate
{
    private static BooksLogic _booksLogic = new();

    public static void Start()
    {
        Console.WriteLine("What is the book ID?");
        Int64 id = Convert.ToInt64(Console.ReadLine()!);

        Console.WriteLine("What is the new author?");
        string author = Console.ReadLine()!;

        Console.WriteLine("What is the new title?");
        string title = Console.ReadLine()!;

        Console.WriteLine("What is the new genre?");
        string genre = Console.ReadLine()!;

        _booksLogic.UpdateBook(id, author, title, genre);
    }
}
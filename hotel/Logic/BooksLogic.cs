//This class is not static so later on we can use inheritance and interfaces
public static class BooksLogic
{

    //Static properties are shared across all instances of the class
    //This can be used to get the current logged in account from anywhere in the program
    //private set, so this can only be set by the class itself

    public static void UpdateBook(Int64 id, string author, string title, string genre)
    {
        BookModel book = new BookModel(id, author, title, genre);
        BooksAccess.Update(book);
    }
}
public class BookModel
{
    public Int64 Id { get; set; }
    public string Author { get; set; }
    public string Title { get; set; }

    public BookModel(Int64 id, string author, string title)
    {
        Id = id;
        Author = author;
        Title = title;
    }
}
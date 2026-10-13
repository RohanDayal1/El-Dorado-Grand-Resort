using Microsoft.Data.Sqlite;
using Dapper;

public static class BooksAccess
{
    private static readonly SqliteConnection _connection = new($"Data Source=DataSources/project.db;Foreign Keys=True");
    private const string _table = "Books";

    public static void Write(BookModel book)
    {
        string sql = $"INSERT INTO {_table} (author, title) VALUES (@Author, @Title)";
        _connection.Execute(sql, book);
    }

    public static BookModel? GetById(int id)
    {
        string sql = $"SELECT * FROM {_table} WHERE id = @Id";
        return _connection.QueryFirstOrDefault<BookModel>
            (sql, new { Id = id });
    }

    public static void Update(BookModel book)
    {
        string sql = $"UPDATE {_table} SET author = @Author, title = @Title WHERE id = @Id";
        _connection.Execute(sql, book);
    }

    public static void Delete(BookModel book)
    {
        string sql = $"DELETE FROM {_table} WHERE id = @Id";
        _connection.Execute(sql, new { book.Id });
    }
}

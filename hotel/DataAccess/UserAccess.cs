using Microsoft.Data.Sqlite;
using Dapper;

public static class UserAccess
{
    private static readonly SqliteConnection _connection = new($"Data Source=DataSources/project.db");
    private const string _table = "Users";

    public static UserModel? GetById(int id)
    {
        string sql = $"SELECT * FROM {_table} WHERE id = @Id";
        return _connection.QueryFirstOrDefault<UserModel>(sql, new { Id = id });
    }

    public static UserModel? GetByEmail(string email)
    {
        string sql = $"SELECT * FROM {_table} WHERE email = @Email";
        return _connection.QueryFirstOrDefault<UserModel>(sql, new { Email = email });
    }

    public static void Create(UserModel user)
    {
        string sql = $"INSERT INTO {_table} (name, lastName, email, phoneNumber, birthDate) VALUES (@Name, @LastName, @Email, @PhoneNumber, @BirthDate)";
        _connection.Execute(sql, user);
    }

    public static void Update(UserModel user)
    {
        string sql = $"UPDATE {_table} SET name = @Name, lastName = @LastName, email = @Email, phoneNumber = @PhoneNumber, birthDate = @BirthDate WHERE id = @Id";
        _connection.Execute(sql, user);
    }

    public static void Delete(UserModel user)
    {
        string sql = $"DELETE FROM {_table} WHERE id = @Id";
        _connection.Execute(sql, user);
    }
}
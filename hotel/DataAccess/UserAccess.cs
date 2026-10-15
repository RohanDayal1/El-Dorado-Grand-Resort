using Microsoft.Data.Sqlite;
using Dapper;

public static class UserAccess
{
    private static readonly SqliteConnection _connection = new($"Data Source=DataSources/el_dorado.db");
    private const string _table = "User";

    public static UserModel? GetById(int id)
    {
        string sql = $"SELECT * FROM {_table} WHERE User_Id = @Id";
        return _connection.QueryFirstOrDefault<UserModel>(sql, new { Id = id });
    }

    public static UserModel? GetByEmail(string email)
    {
        string sql = $"SELECT * FROM {_table} WHERE Email = @Email";
        return _connection.QueryFirstOrDefault<UserModel>(sql, new { Email = email });
    }

    public static void Create(UserModel user)
    {
        string sql = $"INSERT INTO {_table} (First_Name, Last_Name, Email, Phone, Date_Of_Birth) VALUES (@Name, @LastName, @Email, @PhoneNumber, @BirthDate)";
        _connection.Execute(sql, user);
    }

    public static void Update(UserModel user)
    {
        string sql = $"UPDATE {_table} SET First_Name = @Name, Last_Name = @LastName, Email = @Email, Phone = @PhoneNumber, Date_Of_Birth = @BirthDate WHERE User_Id = @Id";
        _connection.Execute(sql, user);
    }

    public static void Delete(UserModel user)
    {
        string sql = $"DELETE FROM {_table} WHERE User_Id = @Id";
        _connection.Execute(sql, user);
    }
}
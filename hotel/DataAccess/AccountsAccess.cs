using Microsoft.Data.Sqlite;
using Dapper;

public static class AccountsAccess
{
    private static readonly SqliteConnection _connection = new($"Data Source=DataSources/project.db");
    private const string _table = "Accounts";

    public static void Write(AccountModel account)
    {
        string sql = $"INSERT INTO {_table} (email, password, fullname) VALUES (@EmailAddress, @Password, @FullName)";
        _connection.Execute(sql, account);
    }

    public static AccountModel? GetById(int id)
    {
        string sql = $"SELECT * FROM {_table} WHERE id = @Id";
        return _connection.QueryFirstOrDefault<AccountModel>
            (sql, new { Id = id });
    }

    public static AccountModel? GetByEmail(string email)
    {
        string sql = $"SELECT * FROM {_table} WHERE email = @Email";
        return _connection.QueryFirstOrDefault<AccountModel>
            (sql, new { Email = email });
    }

    public static void Update(AccountModel account)
    {
        string sql = $"UPDATE {_table} SET email = @EmailAddress, password = @Password, fullname = @FullName WHERE id = @Id";
        _connection.Execute(sql, account);
    }

    public static void Delete(AccountModel account)
    {
        string sql = $"DELETE FROM {_table} WHERE id = @Id";
        _connection.Execute(sql, new { account.Id });
    }
}
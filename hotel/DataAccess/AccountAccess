using Microsoft.Data.Sqlite;
using Dapper;

public static class AccountAccess
{
    private static readonly SqliteConnection _connection = new($"Data Source=DataSources/el_dorado.db");
    private const string _table = "Accounts";

    public static AccountModel? GetById(int id)
    {
        string sql = $"SELECT * FROM {_table} WHERE Account_Id = @Id";
        return _connection.QueryFirstOrDefault<AccountModel>(sql, new { Id = id });
    }
    public static void Create(AccountModel account)
    {
        string sql = $"INSERT INTO {_table} (User_Id, Password, Role) VALUES (@UserId, @Password, @Role)";
        _connection.Execute(sql, account);
    }

    public static void Delete(AccountModel account)
    {
        string sql = $"DELETE FROM {_table} WHERE User_Id = @Id";
        _connection.Execute(sql, new { account.Id });
    }

    public static void Update(AccountModel account)
    {
        string sql = $"UPDATE {_table} SET User_Id = @UserId, Password = @Password, Role = @Role WHERE User_Id = @Id";
        _connection.Execute(sql, account);
    }

}

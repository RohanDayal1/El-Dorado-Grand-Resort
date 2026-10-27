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

    // methods van oude AccountsAccess
    public static UserModel? GetUserByAccountId(int accountId)
    {
        using var connection = DatabaseInitializer.Connect();
        const string sql = @"
            SELECT u.User_Id AS UserId, u.First_Name AS FirstName, u.Last_Name AS LastName,
                   u.Email, u.Phone, u.Date_Of_Birth AS DateOfBirth
            FROM User u
            INNER JOIN Account a ON a.User_Id = u.User_Id
            WHERE a.Account_Id = @AccountId;";
        return connection.QueryFirstOrDefault<UserModel>(sql, new { AccountId = accountId });
    }

    public static AccountModel? GetByEmail(string email)
    {
        using var connection = DatabaseInitializer.Connect();

        return connection.QueryFirstOrDefault<AccountModel>(@"
            SELECT
                a.Account_Id AS Id,
                a.User_Id AS UserId,
                a.Password,
                a.Role,
                u.First_Name AS FirstName,
                u.Last_Name AS LastName,
                u.Email AS Email
            FROM Account a
            INNER JOIN User u ON u.User_Id = a.User_Id
            WHERE u.Email = @Email COLLATE NOCASE;",
            new { Email = email });
    }

}

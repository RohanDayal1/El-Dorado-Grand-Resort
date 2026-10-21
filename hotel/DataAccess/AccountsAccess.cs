using Dapper;

public static class AccountsAccess
{
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

using Dapper;

public static class AccountsAccess
{
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
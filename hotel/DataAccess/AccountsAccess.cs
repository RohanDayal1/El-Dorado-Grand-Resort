using Dapper;

public static class AccountsAccess
{
    public static AccountModel? GetByEmail(string email)
    {
        using var connection = DatabaseInitializer.Connect();

        return connection.QueryFirstOrDefault<AccountModel>(@"
            SELECT
                a.Account_Id AS Id,
                u.Email AS Email,
                a.Password,
                u.First_Name || ' ' || u.Last_Name AS FullName
            FROM Account a
            INNER JOIN User u ON u.User_Id = a.User_Id
            WHERE u.Email = @Email COLLATE NOCASE;",
            new { Email = email });
    }
}
public class AccountModel
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
 
    public AccountModel(int id, int userId, string password, string role)
    {
        Id = id;
        UserId = userId;
        Password = password;
        Role = role;
    }
    public AccountModel(int userId, string password, string role)
    {
        UserId = userId;
        Password = password;
        Role = role;
    } 
    public AccountModel() { }

}
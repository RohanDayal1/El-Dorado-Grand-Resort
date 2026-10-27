public class UserModel
{
    public int UserId { get; set; } 
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public DateTime? DateOfBirth { get; set; }

    public UserModel(int id, string name, string lastName, string email, string phoneNumber, DateTime birthDate)
    {
        UserId = id;
        FirstName = name;
        LastName = lastName;
        Email = email;
        Phone = phoneNumber;
        DateOfBirth = birthDate;
    }

    public UserModel(string name, string lastName, string email, string phoneNumber, DateTime birthDate)
    {
        FirstName = name;
        LastName = lastName;
        Email = email;
        Phone = phoneNumber;
        DateOfBirth = birthDate;
    }

    public UserModel() { }
    
}

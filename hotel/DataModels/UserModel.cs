public class UserModel
{
    public int Id { get; set; } 
    public string Name { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public DateTime BirthDate { get; set; }

    public UserModel(int id, string name, string lastName, string email, string phoneNumber, DateTime birthDate)
    {
        Id = id;
        Name = name;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
        BirthDate = birthDate;
    }

    public UserModel(string name, string lastName, string email, string phoneNumber, DateTime birthDate)
    {
        Name = name;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
        BirthDate = birthDate;
    }

    public UserModel() { }
    
}
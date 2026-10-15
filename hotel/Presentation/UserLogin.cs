public class UserLogin
{
    private static UserLogic _userlogic = new();
    public static void Register()
    {
        Console.WriteLine("Name:");
        string name = Console.ReadLine();

        Console.WriteLine("\nLast Name:");
        string lastName = Console.ReadLine();

        Console.WriteLine("\nBirth Date:");
        string birthDate = Console.ReadLine();

        Console.WriteLine("\nEmail:");
        string mail = Console.ReadLine();

        Console.WriteLine("\nPhone Number:");
        string phoneNumber = Console.ReadLine();

        RegisterEnum result = _userlogic.Register(name, lastName, birthDate, mail, phoneNumber);

        switch (result)
        {
            case RegisterEnum.Success:
                Console.WriteLine("\nInformation saved");
                break;

            case RegisterEnum.InvalidName:
                Console.WriteLine("\nName is invalid");
                break;
            
            case RegisterEnum.InvalidLastName:
                Console.WriteLine("\nLast name is invalid");
                break;

            case RegisterEnum.InvalidEmail:
                Console.WriteLine("\nE-mail is invalid");
                break;

            case RegisterEnum.EmailAlreadyExists:
                Console.WriteLine("\nThis email is already registered");
                break;

            case RegisterEnum.InvalidBirthDate:
                Console.WriteLine("\nInvalid Birthdate");
                break;

            case RegisterEnum.InvalidPhoneNumber:
                Console.WriteLine("\nInvalid phone number");
                break;
        }
    }
}
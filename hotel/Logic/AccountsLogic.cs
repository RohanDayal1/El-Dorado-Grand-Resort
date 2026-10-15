//This class is not static so later on we can use inheritance and interfaces
public class AccountsLogic
{

    //Static properties are shared across all instances of the class
    //This can be used to get the current logged in account from anywhere in the program
    //private set, so this can only be set by the class itself
    public static AccountModel? CurrentAccount { get; private set; }

    public AccountsLogic()
    {

    }

    public AccountModel? CheckLogin(string email, string password)
    {
        AccountModel? account = AccountsAccess.GetByEmail(email);
        if (account != null && account.Password == password)
        {
            CurrentAccount = account;
            return account;
        }

        CurrentAccount = null;
        return null;
    }
}
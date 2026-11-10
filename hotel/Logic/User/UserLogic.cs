public static class UserLogic
{
    public static UserModel GetById(int id)
    {
        return UserAccess.GetById(id);
    }

    public static UserModel GetByEmail(string email)
    {
        if (UserValidator.ValidateEmail(email))
        {
            return UserAccess.GetByEmail(email);
        }
        return null;
    }
}
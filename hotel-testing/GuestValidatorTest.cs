namespace HotelTesting;

[TestClass]
public class GuestValidatorTest
{
    private readonly DateTime today = DateTime.Today;

    // All guest details are valid.
    [TestMethod]
    [DataRow("John", "Smith", "john@gmail.com", "0612345678", 25)]
    [DataRow("Emma", "Brown", "emma@hotmail.com", "0687654321", 30)]
    [DataRow("David", "Jones", "david@outlook.com", "0611223344", 40)]
    [DataRow("Sarah", "Wilson", "sarah@gmail.com", "0698765432", 18)]
    public void ValidateGuestDetails_ValidDetails_NoErrors(string firstName, string lastName, string email, string phone, int age)
    {
        UserModel user = new UserModel(firstName, lastName, email, phone, DateTime.Today.AddYears(-age));

        var errors = GuestValidator.ValidateGuestDetails(user, DateTime.Today);

        Assert.IsEmpty(errors);
    }

    // First name is required.
    [TestMethod]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow("   ")]
    public void ValidateGuestDetails_EmptyFirstName_FirstNameRequired(string firstName)
    {
        UserModel user = new UserModel(firstName, "Smith", "john@gmail.com", "0612345678", DateTime.Today.AddYears(-25));

        var errors = GuestValidator.ValidateGuestDetails(user, DateTime.Today);

        Assert.AreEqual(GuestDetailsErrors.FirstNameRequired, errors[0]);
        Assert.AreEqual(1, errors.Count);
    }

    // First name must contain only letters and be between 2 and 40 characters.
    [TestMethod]
    [DataRow()]
    public void ValidateGuestDetails_InvalidFirstName_FirstNameInvalid(string firstName)
    {
        
    }

}
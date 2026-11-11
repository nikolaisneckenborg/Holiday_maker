public class Customer
{
    public int Id { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public string Mail { get; }
    public string PhoneNumber { get; }

    public Customer(int id, string firstName, string lastName, string mail, string phoneNumber)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Mail = mail;
        PhoneNumber = phoneNumber;
    }
}

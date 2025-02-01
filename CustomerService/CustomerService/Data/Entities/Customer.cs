using System.ComponentModel.DataAnnotations;

namespace CustomerService.Data.Entities;

public class Customer
{
    [Key]
    public int CustomerId { get;}
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Phone{ get; set; }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Skyworx.Repository.Entity;

[Table("user")]
public class UserAccount
{
    [Key, Column("userid")]
    public Guid Id { get; set; }
    [Column("username")]
    public string Username { get; set; }
    [Column("password")]
    public string Password { get; set; }
    [Column("role")]
    public string Role { get; set; }
}
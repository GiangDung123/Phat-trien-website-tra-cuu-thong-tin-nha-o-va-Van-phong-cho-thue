using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PhatTrienWebTraCuuvaChoThue.Models;

[Index("Email", Name = "UQ__Users__A9D1053422ED0877", IsUnique = true)]
public partial class User
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    public string FullName { get; set; } = null!;

    [StringLength(100)]
    public string Email { get; set; } = null!;

    [StringLength(255)]
    public string PasswordHash { get; set; } = null!;

    [StringLength(20)]
    public string? Role { get; set; }

    [InverseProperty("Owner")]
    public virtual ICollection<Listing> Listings { get; set; } = new List<Listing>();
}

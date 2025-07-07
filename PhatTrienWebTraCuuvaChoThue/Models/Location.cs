using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PhatTrienWebTraCuuvaChoThue.Models;

public partial class Location
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    public string Province { get; set; } = null!;

    [StringLength(100)]
    public string District { get; set; } = null!;

    [StringLength(100)]
    public string? Ward { get; set; }

    [StringLength(150)]
    public string? Street { get; set; }

    [InverseProperty("Location")]
    public virtual ICollection<Listing> Listings { get; set; } = new List<Listing>();
}

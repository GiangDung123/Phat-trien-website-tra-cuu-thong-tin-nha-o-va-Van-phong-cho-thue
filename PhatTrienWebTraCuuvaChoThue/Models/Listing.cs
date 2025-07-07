using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PhatTrienWebTraCuuvaChoThue.Models;

public partial class Listing
{
    [Key]
    public int Id { get; set; }

    [StringLength(200)]
    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Price { get; set; }

    public double? Area { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    public int CategoryId { get; set; }

    public int LocationId { get; set; }

    public int OwnerId { get; set; }

    [ForeignKey("CategoryId")]
    [InverseProperty("Listings")]
    public virtual Category? Category { get; set; } 

    [InverseProperty("Listing")]
    public virtual ICollection<ListingImage> ListingImages { get; set; } = new List<ListingImage>();

    [ForeignKey("LocationId")]
    [InverseProperty("Listings")]
    public virtual Location? Location { get; set; }

    [ForeignKey("OwnerId")]
    [InverseProperty("Listings")]
    public virtual User? Owner { get; set; }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PhatTrienWebTraCuuvaChoThue.Models;

public partial class ListingImage
{
    [Key]
    public int Id { get; set; }

    [StringLength(500)]
    public string ImageUrl { get; set; } = null!;

    public int ListingId { get; set; }

    [ForeignKey("ListingId")]
    [InverseProperty("ListingImages")]
    public virtual Listing Listing { get; set; } = null!;
}

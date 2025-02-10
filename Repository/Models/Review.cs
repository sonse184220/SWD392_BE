using System;
using System.Collections.Generic;

namespace Repository.Models;

public partial class Review
{
    public int ReviewId { get; set; }

    public int? RatingStar { get; set; }

    public string? Comment { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? UserId { get; set; }

    public virtual Account? User { get; set; }
}

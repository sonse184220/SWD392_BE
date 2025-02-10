using System;
using System.Collections.Generic;

namespace Repository.Models;

public partial class Blog
{
    public int BlogId { get; set; }

    public string Content { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public int? DestinationId { get; set; }

    public int? UserId { get; set; }

    public virtual Destination? Destination { get; set; }

    public virtual Account? User { get; set; }
}

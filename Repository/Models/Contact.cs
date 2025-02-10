using System;
using System.Collections.Generic;

namespace Repository.Models;

public partial class Contact
{
    public int ContactId { get; set; }

    public string? Email { get; set; }

    public string? Link { get; set; }

    public string? Phone { get; set; }

    public int? DestinationId { get; set; }

    public virtual Destination? Destination { get; set; }
}

using System;
using System.Collections.Generic;

namespace Repository.Models;

public partial class Destination
{
    public int DestinationId { get; set; }

    public string DestinationName { get; set; } = null!;

    public string? Address { get; set; }

    public string? Description { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<Blog> Blogs { get; set; } = new List<Blog>();

    public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();

    public virtual ICollection<OpeningHour> OpeningHours { get; set; } = new List<OpeningHour>();
}

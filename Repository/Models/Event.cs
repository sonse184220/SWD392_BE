using System;
using System.Collections.Generic;

namespace Repository.Models;

public partial class Event
{
    public int EventId { get; set; }

    public string EventName { get; set; } = null!;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public string? Status { get; set; }

    public int? DestinationId { get; set; }

    public virtual Destination? Destination { get; set; }
}

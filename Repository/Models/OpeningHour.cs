using System;
using System.Collections.Generic;

namespace Repository.Models;

public partial class OpeningHour
{
    public int OpeningId { get; set; }

    public int? DestinationId { get; set; }

    public string DayOfWeek { get; set; } = null!;

    public TimeOnly? OpenTime { get; set; }

    public TimeOnly? CloseTime { get; set; }

    public bool? IsClosed { get; set; }

    public virtual Destination? Destination { get; set; }
}

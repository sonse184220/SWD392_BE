﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Repository.Models;

public partial class OpeningHour
{
    public int DestinationId { get; set; }

    public string DayOfWeek { get; set; }

    public TimeSpan? OpenTime { get; set; }

    public TimeSpan? CloseTime { get; set; }

    public bool? IsClosed { get; set; }

    public virtual Destination Destination { get; set; }
}
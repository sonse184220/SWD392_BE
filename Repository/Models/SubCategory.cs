﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Repository.Models;

public partial class SubCategory
{
    public int SubCategoryId { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public int? CategoryId { get; set; }

    public virtual Category Category { get; set; }
}
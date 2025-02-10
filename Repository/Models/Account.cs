using System;
using System.Collections.Generic;

namespace Repository.Models;

public partial class Account
{
    public int UserId { get; set; }

    public string UserName { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public string? Address { get; set; }

    public string Password { get; set; } = null!;

    public string Email { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? RoleId { get; set; }

    public virtual ICollection<Blog> Blogs { get; set; } = new List<Blog>();

    public virtual ICollection<ChatHistory> ChatHistoryReceivers { get; set; } = new List<ChatHistory>();

    public virtual ICollection<ChatHistory> ChatHistorySenders { get; set; } = new List<ChatHistory>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual Role? Role { get; set; }
}

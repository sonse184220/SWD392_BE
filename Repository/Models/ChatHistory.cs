using System;
using System.Collections.Generic;

namespace Repository.Models;

public partial class ChatHistory
{
    public int ChatId { get; set; }

    public int? SenderId { get; set; }

    public int? ReceiverId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? Content { get; set; }

    public virtual Account? Receiver { get; set; }

    public virtual Account? Sender { get; set; }
}

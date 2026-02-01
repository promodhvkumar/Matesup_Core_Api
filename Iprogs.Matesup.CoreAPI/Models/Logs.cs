using System;
using System.Collections.Generic;

namespace Iprogs.Matesup.CoreAPI.Models;

public partial class Logs
{
    public long Id { get; set; }

    public string Category { get; set; } = null!;

    public string? Message { get; set; }

    public DateTime CreatedOn { get; set; }
}

using System;
using System.Collections.Generic;

namespace Iprogs.Matesup.CoreAPI.Models;

public partial class ErrorLogs
{
    public long Id { get; set; }

    public string Category { get; set; } = null!;

    public string? ErrorMessage { get; set; }

    public string? StackTrace { get; set; }

    public DateTime CreatedOn { get; set; }
}

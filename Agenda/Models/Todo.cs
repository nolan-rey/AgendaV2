using System;
using System.Collections.Generic;

namespace Agenda.Models;

public partial class Todo
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public bool? IsCompleted { get; set; }

    public DateTime? DueDate { get; set; }

    public int? ContactId { get; set; }

    public int? CategoryId { get; set; }

    public virtual Category? Category { get; set; }

    public virtual Contact? Contact { get; set; }
}

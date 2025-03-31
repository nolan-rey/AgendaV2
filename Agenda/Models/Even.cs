using System;
using System.Collections.Generic;

namespace Agenda.Models;

public partial class Even
{
    public int Id { get; set; }

    public int? ContactId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string? Location { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public int? CategoryId { get; set; }

    public virtual Category? Category { get; set; }

    public virtual Contact? Contact { get; set; }
}

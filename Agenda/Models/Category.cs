using System;
using System.Collections.Generic;

namespace Agenda.Models;

public partial class Category
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Even> Evens { get; set; } = new List<Even>();

    public virtual ICollection<Todo> Todos { get; set; } = new List<Todo>();
}

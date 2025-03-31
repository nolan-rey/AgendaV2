using System;
using System.Collections.Generic;

namespace Agenda.Models;

public partial class Contact
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? Address { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public DateOnly? BirthDate { get; set; }

    public virtual ICollection<Even> Evens { get; set; } = new List<Even>();

    public virtual ICollection<Socialnetwork> Socialnetworks { get; set; } = new List<Socialnetwork>();

    public virtual ICollection<Todo> Todos { get; set; } = new List<Todo>();
}

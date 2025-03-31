using System;
using System.Collections.Generic;

namespace Agenda.Models;

public partial class Socialnetwork
{
    public int Id { get; set; }

    public int ContactId { get; set; }

    public string NetworkName { get; set; } = null!;

    public string? ProfileUrl { get; set; }

    public string? LogoUrl { get; set; }

    public virtual Contact Contact { get; set; } = null!;
}

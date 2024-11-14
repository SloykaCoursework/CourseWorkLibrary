using System.Collections.Generic;

namespace CourseWorkLibrary;

public class Command
{
    public ushort Code { get; set; }

    public Dictionary<string, object?> Arguments { get; set; } = new Dictionary<string, object?>();
}


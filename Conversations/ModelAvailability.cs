using System;
using System.Collections.Generic;
using System.Linq;

namespace NewMod.Conversations;

public enum RuntimeAvailability { Unknown, Checking, Available, Unavailable }

// A saved preference is never evidence that a runtime or model is installed.
public sealed class ModelAvailability
{
    public RuntimeAvailability State { get; private set; }
    public IReadOnlyList<string> Models { get; private set; } = Array.Empty<string>();
    public bool Contains(string name) => State == RuntimeAvailability.Available && Models.Contains(name);

    public void Invalidate(RuntimeAvailability state = RuntimeAvailability.Unknown)
    {
        State = state;
        Models = Array.Empty<string>();
    }

    public void Complete(IEnumerable<string> models)
    {
        Models = Array.AsReadOnly(models.ToArray());
        State = RuntimeAvailability.Available;
    }
}

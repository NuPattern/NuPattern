using System;
using System.Collections.Generic;
using System.Text;

namespace NuPattern.Extensibility.Serialization.Json
{
  /// <summary>
  /// Specifies missing member handling options for the <see cref="JsonSerializer"/>.
  /// </summary>
  public enum MissingMemberHandling
  {
    /// <summary>
    /// Ignore a missing member and do not attempt to deserialize it.
    /// </summary>
    Ignore = 0,
    /// <summary>
    /// Throw a <see cref="JsonSerializationException"/> when a missing member is encountered during deserialization.
    /// </summary>
    Error = 1
  }
}

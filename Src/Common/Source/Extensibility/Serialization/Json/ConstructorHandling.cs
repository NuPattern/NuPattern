using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Patterning.Extensibility.Serialization.Json
{
  /// <summary>
  /// Specifies how constructors are used when initializing objects during deserialization by the <see cref="JsonSerializer"/>.
  /// </summary>
  public enum ConstructorHandling
  {
    /// <summary>
    /// First attempt to use the public default constructor then fall back to single paramatized constructor.
    /// </summary>
    Default = 0,
    /// <summary>
    /// Allow Json.NET to use a non-public default constructor.
    /// </summary>
    AllowNonPublicDefaultConstructor = 1
  }
}
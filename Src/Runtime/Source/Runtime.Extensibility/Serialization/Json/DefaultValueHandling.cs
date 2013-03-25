using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NuPattern.Extensibility.Serialization.Json
{
  /// <summary>
  /// Specifies default value handling options for the <see cref="JsonSerializer"/>.
  /// </summary>
  [Flags]
  public enum DefaultValueHandling
  {
    /// <summary>
    /// Include members where the member value is the same as the member's default value when serializing objects.
    /// Included members are written to JSON. Has no effect when deserializing.
    /// </summary>
    Include = 0,
    /// <summary>
    /// Ignore members where the member value is the same as the member's default value when serializing objects
    /// so that is is not written to JSON, and ignores setting members when the JSON value equals the member's default value.
    /// </summary>
    Ignore = 1,
    /// <summary>
    /// Members with a default value but no JSON will be set to their default value when deserializing.
    /// </summary>
    Populate = 2,
    /// <summary>
    /// Ignore members where the member value is the same as the member's default value when serializing objects
    /// and sets members to their default value when deserializing.
    /// </summary>
    IgnoreAndPopulate = Ignore | Populate
  }
}
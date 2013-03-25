using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Globalization;

namespace NuPattern.Extensibility.Serialization.Json
{
  /// <summary>
  /// A collection of <see cref="JsonProperty"/> objects.
  /// </summary>
  public class JsonPropertyCollection : KeyedCollection<string, JsonProperty>
  {
    private readonly Type _type;

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonPropertyCollection"/> class.
    /// </summary>
    /// <param name="type">The type.</param>
    public JsonPropertyCollection(Type type) : base(StringComparer.Ordinal)
    {
      Guard.NotNull(()=> type, type);
      _type = type;
    }

    /// <summary>
    /// When implemented in a derived class, extracts the key from the specified element.
    /// </summary>
    /// <param name="item">The element from which to extract the key.</param>
    /// <returns>The key for the specified element.</returns>
    protected override string GetKeyForItem(JsonProperty item)
    {
      return item.PropertyName;
    }

    /// <summary>
    /// Adds a <see cref="JsonProperty"/> object.
    /// </summary>
    /// <param name="property">The property to add to the collection.</param>
    public void AddProperty(JsonProperty property)
    {
      if (Contains(property.PropertyName))
      {
        // don't overwrite existing property with ignored property
        if (property.Ignored)
          return;

        JsonProperty existingProperty = this[property.PropertyName];

        if (existingProperty.Ignored)
        {
          // remove ignored property so it can be replaced in collection
          Remove(existingProperty);
          return;
        }

        if (property.DeclaringType != null && existingProperty.DeclaringType != null)
        {
          if (property.DeclaringType.IsSubclassOf(existingProperty.DeclaringType))
          {
            // current property is on a derived class and hides the existing
            Remove(existingProperty);
            return;
          }
          if (existingProperty.DeclaringType.IsSubclassOf(property.DeclaringType))
          {
            // current property is hidden by the existing so don't add it
            return;
          }
        }

        throw new JsonSerializationException(
          "A member with the name '{0}' already exists on '{1}'. Use the JsonPropertyAttribute to specify another name.".FormatWith(CultureInfo.InvariantCulture, property.PropertyName, _type));
      }

      Add(property);
    }

    /// <summary>
    /// Gets the closest matching <see cref="JsonProperty"/> object.
    /// First attempts to get an exact case match of propertyName and then
    /// a case insensitive match.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns>A matching property if found.</returns>
    public JsonProperty GetClosestMatchProperty(string propertyName)
    {
      JsonProperty property = GetProperty(propertyName, StringComparison.Ordinal);
      if (property == null)
        property = GetProperty(propertyName, StringComparison.OrdinalIgnoreCase);

      return property;
    }

    private bool TryGetValue(string key, out JsonProperty item)
    {
      if (Dictionary == null)
      {
        item = default(JsonProperty);
        return false;
      }

      return Dictionary.TryGetValue(key, out item);
    }


    /// <summary>
    /// Gets a property by property name.
    /// </summary>
    /// <param name="propertyName">The name of the property to get.</param>
    /// <param name="comparisonType">Type property name string comparison.</param>
    /// <returns>A matching property if found.</returns>
    public JsonProperty GetProperty(string propertyName, StringComparison comparisonType)
    {
      // KeyedCollection has an ordinal comparer
      if (comparisonType == StringComparison.Ordinal)
      {
        JsonProperty property;
        if (TryGetValue(propertyName, out property))
          return property;

        return null;
      }

      foreach (JsonProperty property in this)
      {
        if (string.Equals(propertyName, property.PropertyName, comparisonType))
        {
          return property;
        }
      }

      return null;
    }
  }
}
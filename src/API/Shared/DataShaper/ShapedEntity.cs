using System.Collections;
using System.Dynamic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DfE.GetInformationAboutSchools.Prototyping.API.Shared.DataShaper;

/// <summary>
/// Represents a dynamically shaped object whose properties are determined at runtime.
/// Fully JSON-serializable, supports dynamic access, typed getters, optional immutability,
/// nested shaped entities, deep cloning, equality comparison, schema validation,
/// and JSON conversion helpers.
/// </summary>
public class ShapedEntity : DynamicObject, IDictionary<string, object?>
{
    private bool _isReadOnly;

    /// <summary>
    /// Stores dynamic values and participates in JSON serialization.
    /// </summary>
    [JsonExtensionData]
    public IDictionary<string, object?> Values { get; set; } = new ExpandoObject();

    /// <summary>
    /// Initializes a new instance of the <see cref="ShapedEntity"/> class.
    /// </summary>
    public ShapedEntity(bool isReadOnly = false)
    {
        _isReadOnly = isReadOnly;
    }

    private static readonly JsonSerializerOptions _defaultJsonSerializerOptions =
        new(){ WriteIndented = false };

    private static readonly JsonSerializerOptions _indentedJsonSerializerOptions =
        new(){ WriteIndented = true };

    public override bool TryGetMember(GetMemberBinder binder, out object? result)
        => Values.TryGetValue(binder.Name, out result);

    public override bool TrySetMember(SetMemberBinder binder, object? value)
    {
        EnsureWritable();
        Values[binder.Name] = Normalize(value);
        return true;
    }

    /// <summary>
    /// Retrieves a value by key and converts it to the specified type.
    /// Returns default(T) if the key does not exist or cannot be converted.
    /// </summary>
    public TShapedEntity? Get<TShapedEntity>(string key)
    {
        if (!Values.TryGetValue(key, out var value))
            return default;

        try
        {
            if (value is TShapedEntity shapedEntity)
                return shapedEntity;

            if (value is JsonElement json)
                return JsonSerializer.Deserialize<TShapedEntity>(json.GetRawText());

            if (typeof(TShapedEntity).IsEnum && value is string str)
                return (TShapedEntity)Enum.Parse(typeof(TShapedEntity), str, ignoreCase: true);

            return (TShapedEntity?)Convert.ChangeType(value, typeof(TShapedEntity));
        }
        catch
        {
            return default;
        }
    }

    public void MakeReadOnly() => _isReadOnly = true;

    private void EnsureWritable()
    {
        if (_isReadOnly)
        {
            throw new InvalidOperationException(
                "This ShapedEntity is read-only.");
        }
    }

    private static object? Normalize(object? value)
    {
        if (value is null)
            return null;

        if (value is ShapedEntity)
            return value;

        if (value is IDictionary<string, object?> dict)
        {
            var nested = new ShapedEntity();
            foreach (var kvp in dict)
                nested[kvp.Key] = Normalize(kvp.Value);

            return nested;
        }

        if (value is JsonElement)
            return value;

        if (value is IEnumerable<object> list)
            return list.Select(Normalize).ToList();

        var type = value.GetType();

        if (!type.IsPrimitive && type != typeof(string) && type != typeof(decimal))
        {
            ShapedEntity nested = [];

            foreach (var prop in type.GetProperties())
            {
                nested[prop.Name] =
                    Normalize(prop.GetValue(value));
            }

            return nested;
        }

        return value;
    }

    /// <summary>
    /// Creates a deep clone of this <see cref="ShapedEntity"/>, including nested entities.
    /// </summary>
    public ShapedEntity Clone()
    {
        string json = ToJson();
        return FromJson(json);
    }

    public override bool Equals(object? obj)
    {
        if (obj is not ShapedEntity other)
        { 
            return false;
        }

        return JsonSerializer.Serialize(Values) == JsonSerializer.Serialize(other.Values);
    }

    public override int GetHashCode()
        => JsonSerializer.Serialize(Values).GetHashCode();

    /// <summary>
    /// Validates that the entity contains the required fields.
    /// </summary>
    public bool ValidateSchema(
        IEnumerable<string> requiredFields,
        out List<string> missingFields)
    {
        missingFields =
            [
                .. requiredFields.Where(field =>
                    !Values.ContainsKey(field))
            ];

        return missingFields.Count == 0;
    }

    /// <summary>
    /// Serializes this entity to a JSON string.
    /// </summary>
    public string ToJson(bool indented = false)
        => JsonSerializer.Serialize(this,
            indented ? _indentedJsonSerializerOptions : _defaultJsonSerializerOptions);

    /// <summary>
    /// Creates a <see cref="ShapedEntity"/> from a JSON string.
    /// </summary>
    public static ShapedEntity FromJson(string json)
        => JsonSerializer.Deserialize<ShapedEntity>(json) ?? [];

    public void Add(string key, object? value)
    {
        EnsureWritable();
        Values.Add(key, Normalize(value));
    }

    public bool ContainsKey(string key) => Values.ContainsKey(key);

    public bool Remove(string key)
    {
        EnsureWritable();
        return Values.Remove(key);
    }

    public bool TryGetValue(string key, out object? value)
        => Values.TryGetValue(key, out value);

    public object? this[string key]
    {
        get => Values[key];
        set
        {
            EnsureWritable();
            Values[key] = Normalize(value);
        }
    }

    public ICollection<string> Keys => Values.Keys;

    public ICollection<object?> ValuesCollection => Values.Values;

    public void Add(KeyValuePair<string, object?> item)
    {
        EnsureWritable();
        Values.Add(item.Key, Normalize(item.Value));
    }

    public void Clear()
    {
        EnsureWritable();
        Values.Clear();
    }

    public bool Contains(KeyValuePair<string, object?> item)
        => Values.Contains(item);

    public void CopyTo(KeyValuePair<string, object?>[] array, int arrayIndex)
        => Values.CopyTo(array, arrayIndex);

    public int Count => Values.Count;

    public bool IsReadOnly => _isReadOnly;

    ICollection<object?> IDictionary<string, object?>
        .Values => throw new NotImplementedException();

    public bool Remove(KeyValuePair<string, object?> item)
    {
        EnsureWritable();
        return Values.Remove(item);
    }

    public IEnumerator<KeyValuePair<string, object?>> GetEnumerator()
        => Values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}

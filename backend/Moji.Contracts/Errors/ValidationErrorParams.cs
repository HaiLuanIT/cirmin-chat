namespace Moji.Contracts.Errors;

public static class ValidationErrorParams
{
    public static IReadOnlyDictionary<string, object?> Create(
        params (string Key, object? Value)[] values)
    {
        return values.ToDictionary(
            item => item.Key,
            item => item.Value);
    }
}
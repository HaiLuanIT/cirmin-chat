using FluentValidation.Results;
using CirMin.Contracts.Errors;

namespace CirMin.BusinessLogic.Exceptions;

public class CirMinApplicationException : Exception
{
    public CirMinApplicationException(string code, IReadOnlyDictionary<string, object?>? parameters = null,
        Exception? innerException = null) : base(code, innerException)
    {
        Code = code;
        Params = parameters ?? new Dictionary<string, object?>();
    }

    public string Code { get; }

    public IReadOnlyDictionary<string, object?> Params { get; }
}

public class CirMinNotFoundException : CirMinApplicationException
{
    public CirMinNotFoundException(string code, IReadOnlyDictionary<string, object?>? paramters = null) : base(code,
        paramters)
    {
    }
}

public class CirMinUnauthorizedException : CirMinApplicationException
{
    public CirMinUnauthorizedException(string code, IReadOnlyDictionary<string, object?>? paramters = null) : base(code,
        paramters)
    {
    }
}

public class CirMinForbiddenException : CirMinApplicationException
{
    public CirMinForbiddenException(string code, IReadOnlyDictionary<string, object?>? paramters = null) : base(code,
        paramters)
    {
    }
}

public class CirMinConflictException : CirMinApplicationException
{
    public CirMinConflictException(string code, IReadOnlyDictionary<string, object?>? paramters = null) : base(code,
        paramters)
    {
    }
}

public class CirMinBadRequestException : CirMinApplicationException
{
    public CirMinBadRequestException(string code, IReadOnlyDictionary<string, object?>? parameters = null) : base(code,
        parameters)
    {
    }
}

public class CirMinValidationException : Exception
{
    public CirMinValidationException(IEnumerable<ValidationFailure> failures) : base("Validation failed")
    {
        Errors = failures
            .GroupBy(failure => ToCamelCase(failure.PropertyName))
            .ToDictionary(failureGroup => failureGroup.Key,
                failureGroup => failureGroup.Select(ToValidationError).ToArray());
    }

    public IReadOnlyDictionary<string, CirMinValidationError[]> Errors { get; }

    public static CirMinValidationError ToValidationError(ValidationFailure failure)
    {
        var parameters = failure.CustomState as IReadOnlyDictionary<string, object?> ??
                         new Dictionary<string, object?>();

        var code = string.IsNullOrWhiteSpace(failure.ErrorCode) ? ErrorCodes.Validation.Invalid : failure.ErrorCode;
        return new CirMinValidationError(code, parameters);
    }

    private static string ToCamelCase(string str)
    {
        if (string.IsNullOrEmpty(str) || !char.IsUpper(str[0])) return str;
        return char.ToLower(str[0]) + str.Substring(1);
    }
}

public class MediaStorageException : CirMinApplicationException
{
    public MediaStorageException(string code) : base(code)
    {
    }

    public MediaStorageException(string code, Exception? innerException = null) : base(code, null, innerException)
    {
    }
}

public sealed record CirMinValidationError(
    string Code,
    IReadOnlyDictionary<string, object?> Params
);
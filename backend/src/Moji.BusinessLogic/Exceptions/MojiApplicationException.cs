using FluentValidation.Results;
using Moji.Contracts.Errors;

namespace Moji.BusinessLogic.Exceptions;

public class MojiApplicationException : Exception
{
    public MojiApplicationException(string code, IReadOnlyDictionary<string, object?>? parameters = null,
        Exception? innerException = null) : base(code, innerException)
    {
        Code = code;
        Params = parameters ?? new Dictionary<string, object?>();
    }

    public string Code { get; }

    public IReadOnlyDictionary<string, object?> Params { get; }
}

public class MojiNotFoundException : MojiApplicationException
{
    public MojiNotFoundException(string code, IReadOnlyDictionary<string, object?>? paramters = null) : base(code,
        paramters)
    {
    }
}

public class MojiUnauthorizedException : MojiApplicationException
{
    public MojiUnauthorizedException(string code, IReadOnlyDictionary<string, object?>? paramters = null) : base(code,
        paramters)
    {
    }
}

public class MojiForbiddenException : MojiApplicationException
{
    public MojiForbiddenException(string code, IReadOnlyDictionary<string, object?>? paramters = null) : base(code,
        paramters)
    {
    }
}

public class MojiConflictException : MojiApplicationException
{
    public MojiConflictException(string code, IReadOnlyDictionary<string, object?>? paramters = null) : base(code,
        paramters)
    {
    }
}

public class MojiBadRequestException : MojiApplicationException
{
    public MojiBadRequestException(string code, IReadOnlyDictionary<string, object?>? parameters = null) : base(code,
        parameters)
    {
    }
}

public class MojiValidationException : Exception
{
    public MojiValidationException(IEnumerable<ValidationFailure> failures) : base("Validation failed")
    {
        Errors = failures
            .GroupBy(failure => ToCamelCase(failure.PropertyName))
            .ToDictionary(failureGroup => failureGroup.Key,
                failureGroup => failureGroup.Select(ToValidationError).ToArray());
    }

    public IReadOnlyDictionary<string, MojiValidationError[]> Errors { get; }

    public static MojiValidationError ToValidationError(ValidationFailure failure)
    {
        var parameters = failure.CustomState as IReadOnlyDictionary<string, object?> ??
                         new Dictionary<string, object?>();

        var code = string.IsNullOrWhiteSpace(failure.ErrorCode) ? ErrorCodes.Validation.Invalid : failure.ErrorCode;
        return new MojiValidationError(code, parameters);
    }

    private static string ToCamelCase(string str)
    {
        if (string.IsNullOrEmpty(str) || !char.IsUpper(str[0])) return str;
        return char.ToLower(str[0]) + str.Substring(1);
    }
}

public class MediaStorageException : MojiApplicationException
{
    public MediaStorageException(string code) : base(code)
    {
    }

    public MediaStorageException(string code, Exception? innerException = null) : base(code, null, innerException)
    {
    }
}

public sealed record MojiValidationError(
    string Code,
    IReadOnlyDictionary<string, object?> Params
);
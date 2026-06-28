using FluentValidation.Results;

namespace Moji.BusinessLogic.Exceptions;

public class CustomException
{
}

public class MojiNotFoundException : Exception
{
    public MojiNotFoundException(string message) : base(message)
    {
    }
}

public class MojiUnauthorizedException : Exception
{
    public MojiUnauthorizedException(string message) : base(message)
    {
    }
}

public class MojiForbiddenException : Exception
{
    public MojiForbiddenException(string message) : base(message)
    {
    }
}

public class MojiConflictException : Exception
{
    public MojiConflictException(string message) : base(message)
    {
    }
}

public class MojiBadRequestException : Exception
{
    public MojiBadRequestException(string message) : base(message)
    {
    }
    
}

public class MojiValidationException : Exception
{
    public IDictionary<string, string[]> Errors { get; }
    public MojiValidationException() : base("Đã xảy ra một hoặc nhiều lỗi xác thực dữ liệu.")
    {
        Errors = new Dictionary<string, string[]>();
    }

    public MojiValidationException(IEnumerable<ValidationFailure> failures) : this()
    {
        Errors = failures
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(failureGroup => ToCamelCase(failureGroup.Key), failureGroup => failureGroup.ToArray());
    }
    
    private static string ToCamelCase(string str)
    {
        if (string.IsNullOrEmpty(str) || !char.IsUpper(str[0])) return str;
        return char.ToLower(str[0]) + str.Substring(1);
    }
    
}
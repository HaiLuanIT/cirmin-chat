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
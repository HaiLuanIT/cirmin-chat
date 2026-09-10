namespace CirMin.DataAccess.Commons.Exceptions;

public class DataConcurrencyException : Exception
{
    public DataConcurrencyException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
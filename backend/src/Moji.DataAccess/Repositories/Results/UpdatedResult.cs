namespace Moji.DataAccess.Models;

public enum UpdatedResult
{
    Updated,
    ConcurrencyConflict,
    DuplicatedEmail
}
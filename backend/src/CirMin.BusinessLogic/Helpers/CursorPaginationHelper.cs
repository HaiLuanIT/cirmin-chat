using System.Text;

namespace CirMin.BusinessLogic.Helpers;

public static class CursorPaginationHelper
{
    public static string Encode(long? lastId, DateTimeOffset? createdAt)
    {
        if (lastId == null || createdAt == null) return null;

        var cursorRaw = $"{lastId}_{createdAt.Value.Ticks}";
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(cursorRaw));
    }

    public static (long?, DateTimeOffset?) Decode(string? cursor)
    {
        if (cursor == null) return (null, null);
        DateTimeOffset? cursorDateTime = null;
        long? cursorMessageId = null;
        var decodingString = Encoding.UTF8.GetString(Convert.FromBase64String(cursor));
        var parts = decodingString.Split("_");
        if (parts.Length == 2 && long.TryParse(parts[1], out var ticks))
        {
            cursorMessageId = long.Parse(parts[0]);
            cursorDateTime = new DateTimeOffset(ticks, TimeSpan.Zero);
        }
        
        return (cursorMessageId, cursorDateTime);
    }
}
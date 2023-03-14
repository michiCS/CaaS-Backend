using System.Data;

namespace Dal.Common;

public static class DataRecordExtensions
{
    public static T? GetValueOrDefault<T>(this IDataRecord row, string name) where T : struct
    {
        int col = row.GetOrdinal(name);
        if(!row.IsDBNull(col))
        {
            return (T)row.GetValue(col);
        }
        return null;
    }
}

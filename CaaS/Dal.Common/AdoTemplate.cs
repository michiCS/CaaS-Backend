using System.Data;
using System.Data.Common;

namespace Dal.Common;

public delegate T RowMapper<T>(IDataRecord row);

public class AdoTemplate
{
    private readonly IConnectionFactory connectionFactory;

    public AdoTemplate(IConnectionFactory connectionFactory)
    {
        this.connectionFactory = connectionFactory;
    }

    private void AddParameters(DbCommand command, QueryParameter[] parameters)
    {
        foreach (var p in parameters)
        {
            DbParameter dbParam = command.CreateParameter();
            dbParam.ParameterName = p.Name;
            dbParam.Value = p.Value;
            command.Parameters.Add(dbParam);
        }
    }

    public async Task<IEnumerable<T>> QueryAsync<T>(string sql, RowMapper<T> rowMapper, params QueryParameter[] parameters)
    {
        await using DbConnection connection = await connectionFactory.CreateConnectionAsync();

        await using DbCommand command = connection.CreateCommand();
        command.CommandText = sql;
        AddParameters(command, parameters);

        var items = new List<T>();
        await using (DbDataReader reader = await command.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                items.Add(rowMapper(reader));
            }
        }

        return items;
    }

    public async Task<T?> QuerySingleAsync<T>(string sql, RowMapper<T> rowMapper, params QueryParameter[] parameters)
    {
        return (await QueryAsync(sql, rowMapper, parameters)).SingleOrDefault();
    }

    public async Task<int> ExecuteAsync(string sql, params QueryParameter[] parameters)
    {
        await using DbConnection connection = await connectionFactory.CreateConnectionAsync();
        await using DbCommand command = connection.CreateCommand();
        command.CommandText = sql;
        AddParameters(command, parameters);
        return await command.ExecuteNonQueryAsync();
    }

    public async Task<object?> ExecuteScalarAsync(string sql, params QueryParameter[] parameters)
    {
        await using DbConnection connection = await connectionFactory.CreateConnectionAsync();
        await using DbCommand command = connection.CreateCommand();
        command.CommandText = sql;
        AddParameters(command, parameters);
        return await command.ExecuteScalarAsync();
    }
}

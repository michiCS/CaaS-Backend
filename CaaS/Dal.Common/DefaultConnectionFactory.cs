namespace Dal.Common;

using System.Data.Common;
using System.Threading.Tasks;
using Dal.Common;
using Microsoft.Extensions.Configuration;

public class DefaultConnectionFactory : IConnectionFactory
{
    private readonly DbProviderFactory dbProviderFactory;

    public static IConnectionFactory FromConfiguration(IConfiguration config, string connectionStringConfigName)
    {
        var connectionConfig = config.GetSection("ConnectionStrings").GetSection(connectionStringConfigName);
        string connectionString = connectionConfig["ConnectionString"];
        string providerName = connectionConfig["ProviderName"];

        return new DefaultConnectionFactory(connectionString, providerName);
    }

    public DefaultConnectionFactory(string connectionString, string providerName)
    {
        this.ConnectionString = connectionString;
        this.ProviderName = providerName;

        DbUtil.RegisterAdoProviders();
        this.dbProviderFactory = DbProviderFactories.GetFactory(providerName);
    }

    public string ConnectionString { get; }

    public string ProviderName { get; }

    public async Task<DbConnection> CreateConnectionAsync()
    {
        var connection = dbProviderFactory.CreateConnection();
        if(connection is null)
        {
            throw new InvalidOperationException("DbProviderFactory.CreateConnection() returned null");
        }

        connection.ConnectionString = this.ConnectionString;
        await connection.OpenAsync();

        return connection;
    }
}

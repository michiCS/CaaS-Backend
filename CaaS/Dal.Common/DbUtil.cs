namespace Dal.Common;

using System.Data.Common;

public static class DbUtil
{
  public static void RegisterAdoProviders()
  {
    DbProviderFactories.RegisterFactory("MySql.Data.MySqlClient", MySql.Data.MySqlClient.MySqlClientFactory.Instance);
  }
}

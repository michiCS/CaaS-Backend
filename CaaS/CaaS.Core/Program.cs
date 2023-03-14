using CaaS.Core.Dal.Ado;
using CaaS.Core.Dal.Interface;
using CaaS.Core.Logic.Statistics;
using Dal.Common;
using Microsoft.Extensions.Configuration;

namespace CaaS.Core;

public class Program
{

    public static async void PopulateDb()
    {
        var dbPopulator = new DbPopulator(new AdoDaoProvider("ProdDbConnection"));
        await dbPopulator.DeleteAll();
        await dbPopulator.PopulateAll();
    }

    public static void Main()
    {
        PopulateDb();
    }
}
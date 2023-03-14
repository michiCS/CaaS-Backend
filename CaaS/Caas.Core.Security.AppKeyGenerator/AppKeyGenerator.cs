namespace CaaS.Core.Security;

public class AppKeyGenerator : IAppKeyGenerator
{
    public string GenerateAppKey()
    {
        return Guid.NewGuid().ToString();
    }
}

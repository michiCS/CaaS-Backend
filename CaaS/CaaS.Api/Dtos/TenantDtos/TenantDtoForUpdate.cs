namespace CaaS.Api.Dtos.TenantDtos;

public class TenantDtoForUpdate
{
    public string Name { get; set; } = null!;
    public bool RequestNewAppKey { get; set; }
}

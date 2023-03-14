namespace CaaS.Api.Dtos.TenantDtos;

public class TenantDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string AppKey { get; set; } = null!;
}

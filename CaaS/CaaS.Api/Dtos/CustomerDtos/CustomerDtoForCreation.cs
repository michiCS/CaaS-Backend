namespace CaaS.Api.Dtos.CustomerDtos;

public class CustomerDtoForCreation
{
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public int TenantId { get; set; }
}

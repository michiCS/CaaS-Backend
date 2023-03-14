namespace CaaS.Api.Dtos.CustomerDtos;

public class CustomerDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public int TenantId { get; set; }
}

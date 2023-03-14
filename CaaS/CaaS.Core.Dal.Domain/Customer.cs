using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CaaS.Core.Dal.Domain;

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public int TenantId { get; set; }

    public Customer(int id, string name, string email, int tenantId)
    {
        Id = id;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        TenantId = tenantId;
    }

    public override string ToString()
    {
        return this.GetType().Name + JsonSerializer.Serialize(this);
    }
}

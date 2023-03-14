using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;


namespace CaaS.Core.Dal.Domain;

public class Cart
{
    public int Id { get; set; }
    public DateTime CreatedOn { get; set; }
    public int TenantId { get; set; }

    public Cart(int id, DateTime createdOn, int tenantId)
    {
        Id = id;
        CreatedOn = createdOn;
        TenantId = tenantId;
    }

    public override string ToString()
    {
        return this.GetType().Name + JsonSerializer.Serialize(this);
    }
}

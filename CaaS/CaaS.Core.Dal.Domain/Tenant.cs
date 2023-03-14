using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CaaS.Core.Dal.Domain;

public class Tenant
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string AppKey { get; set; }

    public Tenant(int id, string name,  string appKey)
    {
        Id = id;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        AppKey = appKey ?? throw new ArgumentNullException(nameof(appKey));
    }

    public override string ToString()
    {
        return this.GetType().Name + JsonSerializer.Serialize(this);
    }
}

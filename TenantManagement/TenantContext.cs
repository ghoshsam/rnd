using System;

namespace Tenant
{
    public class TenantContext:IDisposable
    {
        public TenantContext(string tenantCode)
        {
            TenantCode = tenantCode;
        }
        public string TenantCode { get; set; }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}

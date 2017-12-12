using Microsoft.AspNetCore.Rewrite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tenant
{
    public class TenantRewriteRule : IRule
    {

        public void ApplyRule(RewriteContext context)
        {
            var httpContext = context.HttpContext;
            var subDomain = httpContext.Request.Host.Host.Split(".");
            var post = httpContext.Request.Host.Port;
            var path = httpContext.Request.Path.ToUriComponent();

            if (subDomain.Length > 2)
                context.HttpContext.Items["TenantContext"] = new TenantContext(subDomain[0]);
            
                      

        }
    }
}

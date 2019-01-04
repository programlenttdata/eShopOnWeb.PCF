using System.Security.Principal;

namespace Microsoft.eShopWeb.Web.Interfaces
{
    public interface IIdentityParser<T>
    {
        T Parse(IPrincipal principal);
    }
}


using Microsoft.EntityFrameworkCore;

namespace Microsoft.eShopOnContainers.Services.Empty
{
    public class EmptyContext: DbContext
    {
        public EmptyContext(DbContextOptions<EmptyContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
           
        }


    }
}

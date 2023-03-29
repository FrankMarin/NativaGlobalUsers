using NativaGlobalUsers.Models;
using System.Linq.Expressions;

namespace NativaGlobalUsers.Repository
{
    public class UserRepository : Repository<User>
    {
        private readonly ApplicationDbContext context;

        public UserRepository(ApplicationDbContext context) :base(context)
        {
            this.context = context;
        }
    }
}

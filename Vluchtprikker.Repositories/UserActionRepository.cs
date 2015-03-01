using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vluchtprikker.DB;
using Vluchtprikker.DB.Repositories;
using Vluchtprikker.Shared;

namespace Vluchtprikker.Repositories
{
    public class UserActionRepository : DbRepository
    {
        public UserActionRepository(IDbContext dbContext)
            : base(dbContext)
        {
        }

        public void Post(string action, string user)
        {
            var dbUserAction = DbContext.UserActions.Create();
            DbContext.UserActions.Add(dbUserAction);

            dbUserAction.CreatedUtc = DateTime.UtcNow;
            dbUserAction.Action = action;
            dbUserAction.User = user;

            DbContext.SaveChanges();
        }
    }
}

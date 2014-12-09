using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgendaAssistant.DB;
using AgendaAssistant.DB.Repositories;
using AgendaAssistant.Shared;

namespace AgendaAssistant.Repositories
{
    public class ErrorRepository : DbRepository
    {
        public ErrorRepository(IDbContext dbContext)
            : base(dbContext)
        {
        }

        public void Post(string source, string message, string stackTrace, string requestUrl, string ipAddress)
        {
            var dbError = CreateDbError(source);
            dbError.Message = message;
            dbError.StackTrace = stackTrace;
            dbError.RequestUrl = requestUrl;
            dbError.IPAddress = ipAddress;

            DbContext.SaveChanges();
        }

        public void PostServerError(Exception ex)
        {
            var dbError = CreateDbError("Server");
            dbError.Message = ex.Message;
            if (ex.InnerException != null)
                dbError.Message += ". " + ex.InnerException.Message;
            dbError.StackTrace = ex.StackTrace;

            DbContext.SaveChanges();
        }

        private Error CreateDbError(string source)
        {
            var dbError = DbContext.Errors.Create();
            DbContext.Errors.Add(dbError);

            dbError.ID = Guid.NewGuid();
            dbError.CreatedUtc = DateTime.UtcNow;
            dbError.Source = source;
            return dbError;
        }
    }
}

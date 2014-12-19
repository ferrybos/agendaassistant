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

        public void PostServerError(Exception ex, string requestUrl, string requestDump, string ipAddress)
        {
            var msg = new StringBuilder();
            msg.AppendLine(ex.Message);

            var innerException = ex.InnerException;
            while (innerException != null)
            {
                msg.AppendLine(innerException.Message);
                innerException = innerException.InnerException;
            }

            if (!string.IsNullOrWhiteSpace(requestDump))
            {
                msg.AppendLine("Request:");
                msg.AppendLine(requestDump);
            }

            var dbError = CreateDbError("Server");
            dbError.Message = msg.ToString();
            dbError.StackTrace = ex.StackTrace;
            dbError.RequestUrl = requestDump;
            dbError.IPAddress = ipAddress;

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

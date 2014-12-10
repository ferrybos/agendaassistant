using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Vluchtprikker.DB.Repositories;
using Vluchtprikker.Repositories;

namespace Vluchtprikker.Web.api
{
    public abstract class ApiBaseController : ApiController
    {
        public void HandleServerError(Exception ex)
        {
            try
            {
                new ErrorRepository(new VluchtprikkerDbContext()).PostServerError(ex);
            }
            catch (Exception e)
            {
                Trace.WriteLine(string.Format("Failed to HandleServerError: {0}. Original exception: {1}", e.Message, ex.Message));
            }
        }
    }
}

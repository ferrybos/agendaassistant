using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
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
                var ipAddress = HttpContext.Current.Request.UserHostAddress;
                string requestUrl = string.Format("{0} ({1})", Request.RequestUri.AbsolutePath, Request.Method.Method);
                string requestDump = ""; // RequestDump(Request);

                new ErrorRepository(new VluchtprikkerDbContext()).PostServerError(ex, requestUrl, requestDump, ipAddress);
            }
            catch (Exception e)
            {
                Trace.WriteLine(string.Format("Failed to HandleServerError: {0}. Original exception: {1}", e.Message, ex.Message));
            }
        }

        //private string RequestDump(HttpRequestMessage request)
        //{
        //    var sb = new StringBuilder();

        //    //sb.AppendLine("Headers:");
        //    //sb.AppendLine(request.Headers.ToString());

        //    if (request.Content != null)
        //    {
        //        sb.AppendLine("Content:");
        //        var content = request.Content.ReadAsStringAsync().Result;

        //        sb.AppendLine(content);
        //    }

        //    return sb.ToString();
        //}
    }
}

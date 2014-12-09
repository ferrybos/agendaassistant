using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vluchtprikker.Shared
{
    public interface IDbContext
    {
        DbContext Current { get; }
    }
}

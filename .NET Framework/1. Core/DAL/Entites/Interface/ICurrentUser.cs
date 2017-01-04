using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entites
{
    public interface ICurrentUser
    {
        long UserId { get; set; }
        bool IsSystemUser { get; set; }
    }
}

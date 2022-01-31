using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domains
{
    public class RefreshToken
    {
        public string UserId { get; set; }
        public DateTime ExpireDate { get; set; }
    }
}

using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Entities
{
    public class UserApp:IdentityUser
    {
        //identity user içerisinde özellikleri kullanır birde bu özelliği ekledim içerisine
        public string CityPlateCode { get; set; }
    }
}

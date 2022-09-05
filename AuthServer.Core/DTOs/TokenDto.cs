using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Dto
{
    public class TokenDto
    {
        public string AccessToken { get; set; }

        //accesstoken kullanım süresi
        public DateTime AccessTokenExpiration { get; set; }

        public string RefreshToken { get; set; }

        //refreshtoken kullanım süresi
        public DateTime RefreshTokenExpiration { get; set; }


    }
}

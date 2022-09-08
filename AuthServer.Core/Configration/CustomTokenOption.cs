using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Configration
{
    public class CustomTokenOption
    {

        //Bu token hangi apilere istek yapabilir. Audiences ile tutulur
        public List<string> Audience { get; set; }

        //Bu tokenı kim dağıtmışsa onu işiyoruz buraya
        public string Issuer { get; set; }

        //Access Token ömrü
        public int AccessTokenExpiration { get; set; }

        //Refreshy token ömrü
        public int RefreshTokenExpiration { get; set; }

        //Sign servisine gönderip oradan key alacağız
        public string SecurityKey { get; set; }


        //Bunların karşılığı  appjsonsettings içerisinde olacak
    }
}

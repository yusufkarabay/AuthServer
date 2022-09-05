using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Configration
{
    public class Client
    {
        //her client tipi için oluşturulmuş bir sınıf örnek olarak ios için id=1 ios için clientsevret=asfda gibi
        //her tip için birer örnek oluşturacağız
        public string Id { get; set; }
        public string Secret { get; set; }

        //client tiplerinin hangi apilere ulaşabileceğini burada belirteceğiz.
        //Mesela webden gelen x'e androidden gelen y ile haberleşebilir gibi
        public List<string> Audiences { get; set; }
    }
}

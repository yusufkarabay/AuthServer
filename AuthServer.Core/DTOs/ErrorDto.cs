using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Dto
{
    public class ErrorDto
    {
        public List<string> Errors { get; private set; }

        //oluşan hatalarda ıshow true ise client hataları görür. false ise sadece yazılım tarafı görür.
        public bool IsSHow { get; set; }
        public ErrorDto()
        {
            Errors = new List<string>();
        }

        public ErrorDto(string error,bool isShow)
        {
            Errors.Add(error);
            isShow=true;

        }
        public ErrorDto(List<string> errors,bool isShow)
        {
            Errors=errors;
            isShow=true;
        }

    }
}

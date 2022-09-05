using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AuthServer.Core.Dto
{
    public class CustomResponseDto<T> where T : class
    {
        public T Data { get; private set; }
        public int StatusCode { get; private set; }
        public ErrorDto Error { get;private set; }

        //olayın başarılı olup olmadığını status code yerine  buradan kontrol edeceğiz
        [JsonIgnore]
        public bool IsSuccessful { get; private  set; }  

        //başarılı durumda ilgili class ve durum kodu döner
        public static CustomResponseDto<T> Success(T data, int statusCode)
        {
            return new CustomResponseDto<T> { Data = data, StatusCode = statusCode,IsSuccessful=true };
        }

        //başarılı durumdae durum kodu döner
        public static CustomResponseDto<T> Success(int statusCode)
        {
            return new CustomResponseDto<T> { Data=default, StatusCode = statusCode, IsSuccessful=true };
        }

        //birden fazla hata ve durum kodu döner
        public static CustomResponseDto<T> Fail(ErrorDto errorDto, int statusCode)
        {
            return new CustomResponseDto<T> { Error=errorDto, StatusCode = statusCode, IsSuccessful=false };
        }

        //hata mesajını ve durum kodunu döner. Gösterilme durumuna göre döner
        public static CustomResponseDto<T> Fail(string errorMessage, int statusCode, bool isShow)
        {
            var errorDto = new ErrorDto(errorMessage, isShow);

            return new CustomResponseDto<T>
            {
                Error=errorDto,
                StatusCode = statusCode,
                IsSuccessful=false
            };
        }
    }
}

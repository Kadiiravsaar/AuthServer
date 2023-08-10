using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SharedLibrary.Dtos
{
    public class Response<T> where T : class
    {
        public T Data { get; private set; }
        public int StatusCode { get; private set; }

        [JsonIgnore]// serilaze edilmesin, json dataya dönüştürülürken bu yok sayılsın
        public bool IsSuccessful { get; private set; } // clientlere açmayacağım bir propr
        // kendi apilerimde kullanmak için yapıyorum
        // kısa yoldan olayın başarılı mı başarısız mı olduğunu burdan kontrol edelim (statusden değil) 
        public ErrorDto Error { get; private set; }


        public static Response<T> Success(T data, int statusCode)
        {
            return new Response<T>
            {
                Data = data,
                StatusCode = statusCode,
                IsSuccessful = true
            };
        }

        public static Response<T> Success(int statusCode) // hiç data göndermek istemiyorsam
        {
            return new Response<T>
            {
                Data = default, // bir entityde güncelleme veya silme yaptığımda data dönmeye gerek yoktur
                StatusCode = statusCode,
                IsSuccessful = true
            };
        }

        public static Response<T> Fail(ErrorDto errorDto, int statusCode) // parametre de errorDto verdiğimiz için isShow vermedik
        {
            return new Response<T>
            {
                Error = errorDto,
                StatusCode = statusCode,
                IsSuccessful = false
            };
        }


        public static Response<T> Fail(string errorMessage, int statusCode, bool isShow) // tek bir hata dönmek için
        {
            var errorDto = new ErrorDto(errorMessage, isShow); // error dto nesnesi oluşturalım
            return new Response<T>
            {
                Error = errorDto,
                StatusCode = statusCode,
                IsSuccessful = false
            };
        }
    }

}

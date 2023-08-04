using SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Dtos
{
    public class ErrorDto
    {

        public List<string> Errors { get; private set; } // hataların listesi
        public bool IsShow { get; private set; } // hataların kullanıcıya gösterilip gösterilmeyeceği

        // *** private sete çektik çünkü başka biri propu set etmek isterse ctoru mutlaka kullansın

        public ErrorDto()
        {
            Errors = new List<string>(); // nesne örneği aldık ki kolayca doldurabilelim
        }
        public ErrorDto(string error, bool isShow) // bir tane hata meydana gelirse diye yaptık
        {
            Errors.Add(error); // hata dizinine tek gelen hatayı ekledik
            isShow = true;
        }
        public ErrorDto(List<string> errors, bool isShow) // birden fazla hata meydana gelirse
        {
            Errors = errors;
            isShow = true;
        }

    }
}

























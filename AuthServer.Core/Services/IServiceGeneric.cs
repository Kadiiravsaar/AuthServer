using SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Services
{
    public interface IServiceGeneric<TEntity, TDto> where TEntity : class where TDto : class // dto transferiin burda yapacağım için TDto verdik
    {
        // (TDto) => Service katmanına data katmanından gelen entity dtoya çevirip direk api'ye sunacağım
        Task<Response<TDto>> GetByIdAsync(int id);

        Task<Response<IEnumerable<TDto>>> GetAllAsync(); 

        Task<Response<IEnumerable<TDto>>> Where(Expression<Func<TDto, bool>> predicate); // direk ham datayı çekeceğiz

        Task<Response<TDto>> Add(TDto entity);
        Task<Response<NoDataDto>> Remove(TEntity entity); // cilentlara boş data dönücem başka bir şey dönmeme gerek yok
        Task<Response<NoDataDto>> Update(TEntity entity); // _context.Entry(entity).state = entityState.modified

    }
}

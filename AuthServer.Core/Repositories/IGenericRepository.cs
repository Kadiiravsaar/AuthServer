using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Repositories
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        Task<TEntity> GetByIdAsync(int id);

        Task<IEnumerable<TEntity>> GetAllAsync();   

        IQueryable<TEntity> Where(Expression<Func<TEntity,bool>> predicate);

        // where(x=>x.id>50)
        // x => TEntity
        // x.ıd>50 = bool

        // product = prodrepo.where(x=>x.ıd>5) bu daha dbye yansımadı hala where sorgusu yazabilirsin
        // product.tolist dersen dbye yansır

        Task Add(TEntity entity);
        void Remove(TEntity entity);
        TEntity Update(TEntity entity); // _context.Entry(entity).state = entityState.modified

    }
}

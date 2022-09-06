using AuthServer.Core.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Services
{
    public interface IGenericService<T, TDto> where T : class where TDto : class
    {
        Task<CustomResponseDto<TDto>> GetByIdAsync(Guid id);
        Task<CustomResponseDto<IQueryable<TDto>>> GetAll();
        Task<CustomResponseDto<IEnumerable<TDto>>> Where(Expression<Func<T, bool>> expression);
        Task<CustomResponseDto<TDto>> AddAsync(TDto entity);
        Task<CustomResponseDto<NoDataDto>> Update(TDto entity);
        Task<CustomResponseDto<NoDataDto>> DeleteAsync(TDto entity);
    }
}

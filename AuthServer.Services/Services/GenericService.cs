using AuthServer.Core;
using AuthServer.Core.Dto;
using AuthServer.Core.Repositories;
using AuthServer.Core.Services;
using AuthServer.Services.Mapping;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Services.Services
{
    public class GenericService<T, TDto> : IGenericService<T, TDto> where T : class where TDto : class
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<T> _genericRepository;
        private readonly IMapper _mapper;

        public GenericService(IUnitOfWork unitOfWork, IGenericRepository<T> genericRepository, IMapper mapper)
        {
            _unitOfWork=unitOfWork;
            _genericRepository=genericRepository;
            _mapper=mapper;
        }

        public async Task<CustomResponseDto<TDto>> AddAsync(TDto entity)
        {
            //Dto entitye dönüştürüldü
            var newEntity = _mapper.Map<T>(entity);
            //entity eklendi
            await _genericRepository.AddAsync(newEntity);
            //veritabanına kaydedildi
            await _unitOfWork.SaveChangesAsync();

            //sonuç döndermek için entity tekrardan dtoya dönüşütrüldü
            var newDto = _mapper.Map<TDto>(newEntity);
            //işlem sonucu ile birlikte dto dönderildi
            return CustomResponseDto<TDto>.Success(newDto, 200);
        }

        public Task<CustomResponseDto<NoDataDto>> DeleteAsync(TDto entity)
        {
            throw new NotImplementedException();
        }

        public async Task<CustomResponseDto<IQueryable<TDto>>> GetAll()
        {
            throw new NotImplementedException();
        }

        public async Task<CustomResponseDto<TDto>> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<CustomResponseDto<NoDataDto>> Update(TDto entity)
        {
            throw new NotImplementedException();
        }

        public Task<CustomResponseDto<IEnumerable<TDto>>> Where(Expression<Func<T, bool>> expression)
        {
            throw new NotImplementedException();
        }
    }
}

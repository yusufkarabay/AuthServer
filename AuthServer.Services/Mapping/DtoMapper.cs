using AuthServer.Core.DTOs;
using AuthServer.Core.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Services.Mapping
{
    public class DtoMapper : Profile
    {
        public DtoMapper()
        {
            CreateMap<UserAppDto,UserApp>().ReverseMap();
        }
    }
}

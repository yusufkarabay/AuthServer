﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.DTOs
{
    public class UserAppDto
    {
        public string UserId { get; set; }
      //  public string UserName { get; set; }
        public string Email { get; set; }
        public string CityPlateCode { get; set; }
    }
}

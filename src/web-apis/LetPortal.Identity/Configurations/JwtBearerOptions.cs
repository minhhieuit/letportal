﻿using System;
using System.Collections.Generic;
using System.Text;

namespace LetPortal.Identity.Configurations
{
    public class JwtBearerOptions
    {
        public string Secret { get; set; }

        public string Issuer { get; set; }

        public string Audience { get; set; }

        public int TokenExpiration { get; set; }

        public int RefreshTokenExpiration { get; set; }
    }
}

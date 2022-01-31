using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace VitalityApp.Authentication
{
    // TODO: make into a microservice
    public class VitalityAppAuthAttribute : OpenApiSecurityAttribute
    {
        public VitalityAppAuthAttribute() : base("VitalityAppAuth", SecuritySchemeType.Http)
        {
            Description = "JWT for authorization";
            In = OpenApiSecurityLocationType.Header;
            Scheme = OpenApiSecuritySchemeType.Bearer;
            BearerFormat = "JWT";
        }
    }

    public class UnauthorizedRequestAttribute : OpenApiResponseWithBodyAttribute
    {
        public UnauthorizedRequestAttribute() : base(HttpStatusCode.Unauthorized, "text/plain", typeof(string))
        {
            Description = "User login is invalid";
        }
    }

    public class ForbiddenRequestAttribute : OpenApiResponseWithBodyAttribute
    {
        public ForbiddenRequestAttribute() : base(HttpStatusCode.Forbidden, "text/plain", typeof(string))
        {
            Description = "User login does not permit acces to this function";
        }
    }
}

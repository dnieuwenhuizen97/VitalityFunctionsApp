using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Api.Models
{
    public class FileModel
    {
        //The 'Microsoft.AspNetCore.Http.IFormFile' type can read the stream of file data from the client upload files.
            public IFormFile ImageFile { get; set; }
    }
}

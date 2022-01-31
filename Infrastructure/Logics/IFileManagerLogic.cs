using Infrastructure.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Logics
{
    public interface IFileManagerLogic
    {
        Task Upload(FileModel model);
    }
}

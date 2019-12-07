using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Imagegram.API.Application.Repositories
{
    public interface IAccountRepository : IDisposable
    {
        Task<string> CreateAccount(string name, string UUID);
        Task<string> DeleteAccount(string UUID);
    }
}

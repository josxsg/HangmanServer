using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace HangmanServer.DataAccess
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Users> Users { get; }
        IRepository<Matches> Matches { get; }
        IRepository<Words> Words { get; }
        IRepository<Categories> Categories { get; }

        Task<int> CompleteAsync();
    }
}
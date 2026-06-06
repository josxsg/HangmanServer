using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace HangmanServer.DataAccess
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly HangmanDBEntities _context;

        public IRepository<Users> Users { get; private set; }
        public IRepository<Matches> Matches { get; private set; }
        public IRepository<Words> Words { get; private set; }
        public IRepository<Categories> Categories { get; private set; }

        public UnitOfWork(HangmanDBEntities context)
        {
            _context = context;
            Users = new Repository<Users>(_context);
            Matches = new Repository<Matches>(_context);
            Words = new Repository<Words>(_context);
            Categories = new Repository<Categories>(_context);
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
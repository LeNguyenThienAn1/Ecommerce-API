using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Queries
{
    public interface IUserQueries
    {
        public Task<UserEntity> GetUserAsync();
    }
}

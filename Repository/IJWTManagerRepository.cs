using Crud11API.Models;
using Crud11API.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crud11API.Repository
{
    public interface IJWTManagerRepository
    {
        Tokens Authenticate(User users);
        Tokens VerifyToken(string token);
    }
}

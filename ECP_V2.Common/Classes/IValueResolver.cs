using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECP_V2.Common.Classes
{
    interface IValueResolver
    {
        ResolutionResult Resolve(ResolutionResult source);
    }
}

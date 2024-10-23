using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECP_V2.Business.UnitOfWork
{
    public interface IUnitOfWork
    {
        void Save();
        void RollBack();
    }
}

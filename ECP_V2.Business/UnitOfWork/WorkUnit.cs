using System;
using ECP_V2.DataAccess;

namespace ECP_V2.Business.UnitOfWork
{
    public class WorkUnit : IUnitOfWork, IDisposable
    {
        public ECP_V2Entities Context { get; private set; }

        public WorkUnit()
        {
            Context = new ECP_V2Entities();
        }

        public void Save()
        {
            if (Context != null)
            {
                Context.SaveChanges();
            }
        }

        public void RollBack()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            if (Context != null)
            {
                Context.Dispose();
                Context = null;
            }
        }
    }
}

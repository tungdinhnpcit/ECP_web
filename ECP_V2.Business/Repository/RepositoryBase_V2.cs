using ECP_V2.Business.UnitOfWork;
using ECP_V2.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECP_V2.Business.Repository
{
    public abstract class RepositoryBase_V2 : IDisposable
    {
        protected WorkUnit unit = null;
        private bool isWorkUnitGranular = true;
        public RepositoryBase_V2()
        {
            unit = new WorkUnit();
        }

        public RepositoryBase_V2(WorkUnit unit)
        {
            this.unit = unit;
            isWorkUnitGranular = false;
        }

        public void Dispose()
        {
            if (isWorkUnitGranular)
            {
                unit.Context.SaveChanges();
            }
            unit.Dispose();
        }

        public ECP_V2Entities Context { get { return unit.Context; } }
    }
}

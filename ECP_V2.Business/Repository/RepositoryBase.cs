using System;
using System.Collections.Generic;
using ECP_V2.DataAccess;
using ECP_V2.Business.UnitOfWork;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Entity;

namespace ECP_V2.Business.Repository
{
    public abstract class RepositoryBase<T> : IDisposable
    {
        protected WorkUnit unit = null;
        private bool isWorkUnitGranular = true;        
        public RepositoryBase()
        {
            unit = new WorkUnit();
        }

        public RepositoryBase(WorkUnit unit)
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
        public abstract object Create(T entity, ref string strError);
        //public abstract object ImportList(List<T> entity, ref string strError);
        public abstract object Update(T entity, ref string strError);
        public abstract string Delete(object entityId, ref string strError);

        public abstract T GetById(object entityId);

        public abstract List<T> List();
        public abstract List<T> ListByQuery(string strQuery);

        public abstract List<T> GetTableListById(object entityId);        
    }

}

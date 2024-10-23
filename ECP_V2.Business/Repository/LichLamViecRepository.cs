using ECP_V2.Business.UnitOfWork;
using ECP_V2.DataAccess;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ECP_V2.Business.Repository
{
    public class LichLamViecRepository : RepositoryBase<tblLichLamViec>
    {
        public LichLamViecRepository()
           : base()
        {
        }

        public LichLamViecRepository(WorkUnit unit)
            : base(unit)
        {
        }


        public override object Create(tblLichLamViec entity, ref string strError)
        {
            throw new NotImplementedException();
        }

        public override string Delete(object entityId, ref string strError)
        {
            throw new NotImplementedException();
        }

        public override tblLichLamViec GetById(object entityId)
        {
            try
            {
                var id = Convert.ToInt16(entityId);
                var donvi = Context.tblLichLamViecs.Where(p => p.Id == id).Single();
                return donvi;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public tblLichLamViec GetByCreatedDateOnTop()
        {
            try
            {
                var donvi = Context.tblLichLamViecs.OrderByDescending(x => x.CreateDate).First();

                return donvi;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

   

        public override List<tblLichLamViec> GetTableListById(object entityId)
        {
            throw new NotImplementedException();
        }

        public override List<tblLichLamViec> List()
        {
            throw new NotImplementedException();
        }

        public override List<tblLichLamViec> ListByQuery(string strQuery)
        {
            throw new NotImplementedException();
        }      

        public override object Update(tblLichLamViec entity, ref string strError)
        {
            throw new NotImplementedException();
        }
    }
}

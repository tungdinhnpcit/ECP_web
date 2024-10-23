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
    public class AspNetRoleReponsitory : RepositoryBase<AspNetRole>
    {
        public AspNetRoleReponsitory()
           : base()
        {
        }

        public AspNetRoleReponsitory(WorkUnit unit)
            : base(unit)
        {
        }

        public override object Create(AspNetRole entity, ref string strError)
        {
            throw new NotImplementedException();
        }

        public override string Delete(object entityId, ref string strError)
        {
            throw new NotImplementedException();
        }

        public override AspNetRole GetById(object entityId)
        {
            try
            {
                string id = entityId.ToString();
                var entity = Context.AspNetRoles.Where(p => p.Id == id).FirstOrDefault();
                return entity;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public AspNetRole GetRoleSytemById(int typeOfRole)
        {
            try
            {                
                var entity = Context.AspNetRoles.Where(p => p.TypeOfRole == typeOfRole).FirstOrDefault();
                return entity;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public override List<AspNetRole> GetTableListById(object entityId)
        {
            throw new NotImplementedException();
        }

        public override List<AspNetRole> List()
        {
            throw new NotImplementedException();
        }

        public override List<AspNetRole> ListByQuery(string strQuery)
        {
            throw new NotImplementedException();
        }

        public override object Update(AspNetRole entity, ref string strError)
        {
            throw new NotImplementedException();
        }
    }
}

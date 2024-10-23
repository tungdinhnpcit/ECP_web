using Dapper;
using ECP_V2.Business.UnitOfWork;
using ECP_V2.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECP_V2.Business.Repository
{
    public class sc_KienNghiMienTruRepository : RepositoryBase<sc_KienNghiMienTru>
    {
        public string Connectstr { get; set; }
        public sc_KienNghiMienTruRepository(string connect)
            : base()
        {
            this.Connectstr = connect;
            try
            {
                var connection = new SqlConnection(Context.Database.Connection.ConnectionString);
                Connectstr = connection.ConnectionString;
            }
            catch (Exception ex)
            { }
        }

        public sc_KienNghiMienTruRepository()
            : base()
        {
        }

        public sc_KienNghiMienTruRepository(WorkUnit unit)
            : base(unit)
        {
        }

        public override string Delete(object entityId, ref string strError)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                var entity = Context.sc_KienNghiMienTru.SingleOrDefault(o => o.Id == id);
                Context.sc_KienNghiMienTru.Remove(entity);
                Context.SaveChanges();
                strError = "";
                return "success";
            }
            catch (Exception ex) { strError = ex.Message; return "error"; }
        }

        public override object Create(sc_KienNghiMienTru entity, ref string strError)
        {
            try
            {
                Context.Entry(entity).State = EntityState.Added;
                Context.SaveChanges();
                return entity.Id;
            }
            catch (Exception ex)
            {
                strError = ex.Message;
                return 0;
            }
        }

        public override sc_KienNghiMienTru GetById(object entityId)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                return Context.sc_KienNghiMienTru.SingleOrDefault(p => p.Id == id);
            }
            catch { return null; }
        }

        public override List<sc_KienNghiMienTru> List()
        {
            try
            {
                return Context.sc_KienNghiMienTru.ToList();
            }
            catch (Exception ex) { return null; }
        }

        public override List<sc_KienNghiMienTru> ListByQuery(string strQuery)
        {
            throw new NotImplementedException();
        }

        public override object Update(sc_KienNghiMienTru entity, ref string strError)
        {
            try
            {
                Context.Entry(entity).State = EntityState.Modified;
                Context.SaveChanges();
                return entity.Id;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public override List<sc_KienNghiMienTru> GetTableListById(object entityId)
        {
            throw new NotImplementedException();
        }

       
    }
}

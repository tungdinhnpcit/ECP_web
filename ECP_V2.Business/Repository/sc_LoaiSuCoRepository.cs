using Dapper;
using ECP_V2.Business.UnitOfWork;
using ECP_V2.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECP_V2.Business.Repository
{
    public class sc_LoaiSuCoRepository : RepositoryBase<sc_LoaiSuCo>
    {



        public override object Create(sc_LoaiSuCo entity, ref string strError)
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

        public override string Delete(object entityId, ref string strError)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                var entity = Context.sc_LoaiSuCo.SingleOrDefault(o => o.Id == id);
                Context.sc_LoaiSuCo.Remove(entity);
                Context.SaveChanges();
                strError = "";
                return "success";
            }
            catch (Exception ex) { strError = ex.Message; return "error"; }
        }

        public override sc_LoaiSuCo GetById(object entityId)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                return Context.sc_LoaiSuCo.SingleOrDefault(p => p.Id == id);
            }
            catch { return null; }
        }



        public override List<sc_LoaiSuCo> GetTableListById(object entityId)
        {
            throw new NotImplementedException();
        }

        public List<sc_LoaiSuCo> GetByType(object entityId)
        {
            try
            {
                string id = entityId.ToString();
                return Context.sc_LoaiSuCo.Where(p => p.TypeOfLSC == id).ToList();
            }
            catch (Exception ex) { return null; }
        }

        public List<sc_LoaiSuCo> GetListNguyenNhanByTinhChat(int tc)
        {
            try
            {
                return Context.sc_LoaiSuCo.Where(p => p.CapCha == tc).ToList();
            }
            catch (Exception ex) { return null; }
        }

        public override List<sc_LoaiSuCo> List()
        {
            try
            {
                return Context.sc_LoaiSuCo.ToList();
            }
            catch (Exception ex) { return null; }
        }

        public override List<sc_LoaiSuCo> ListByQuery(string strQuery)
        {
            throw new NotImplementedException();
        }

        public override object Update(sc_LoaiSuCo entity, ref string strError)
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
    }
}

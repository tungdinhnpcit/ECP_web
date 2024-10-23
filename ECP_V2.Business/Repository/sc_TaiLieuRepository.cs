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
    public class sc_TaiLieuRepository : RepositoryBase<sc_TaiLieu>
    {   


        public override object Create(sc_TaiLieu entity, ref string strError)
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
                var entity = Context.sc_TaiLieu.SingleOrDefault(o => o.Id == id);
                Context.sc_TaiLieu.Remove(entity);
                Context.SaveChanges();
                strError = "";
                return "success";
            }
            catch (Exception ex) { strError = ex.Message; return "error"; }
        }

        public string DeleteTaiLieuBySuCoId(object entityId, ref string strError)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                var entity = Context.sc_TaiLieu.Where(o => o.SuCoId == id && o.TypeObj == 1);
                //foreach (var item in entity)
                //{
                    
                //}
                Context.sc_TaiLieu.RemoveRange(entity);
                Context.SaveChanges();
                strError = "";
                return "success";
            }
            catch (Exception ex) { strError = ex.Message; return "error"; }
        }

        public string DeleteHinhAnhBySuCoId(object entityId, ref string strError)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                var entity = Context.sc_TaiLieu.Where(o => o.SuCoId == id && o.TypeObj == 2);           
                Context.sc_TaiLieu.RemoveRange(entity);
                Context.SaveChanges();
                strError = "";
                return "success";
            }
            catch (Exception ex) { strError = ex.Message; return "error"; }
        }

        public override sc_TaiLieu GetById(object entityId)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                return Context.sc_TaiLieu.SingleOrDefault(p => p.Id == id);
            }
            catch { return null; }
        }

        public override List<sc_TaiLieu> GetTableListById(object entityId)
        {
            throw new NotImplementedException();
        }

        public override List<sc_TaiLieu> List()
        {
            try
            {
                return Context.sc_TaiLieu.ToList();
            }
            catch (Exception ex) { return null; }
        }

        public sc_TaiLieu GetTaiLieuBySuCoId(object entityId)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                return Context.sc_TaiLieu.FirstOrDefault(p => p.SuCoId == id & p.TypeObj == 1);
            }
            catch(Exception ex) { return null; }
        }

        public sc_TaiLieu GetHinhAnhBySuCoId(object entityId)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                return Context.sc_TaiLieu.FirstOrDefault(p => p.SuCoId == id & p.TypeObj == 2);
            }
            catch { return null; }
        }

        public override List<sc_TaiLieu> ListByQuery(string strQuery)
        {
            try
            {
                int suCoId = Convert.ToInt32(strQuery);
                return Context.sc_TaiLieu.Where(p => p.SuCoId == suCoId).ToList();
            }
            catch { return null; }
        }

        public override object Update(sc_TaiLieu entity, ref string strError)
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

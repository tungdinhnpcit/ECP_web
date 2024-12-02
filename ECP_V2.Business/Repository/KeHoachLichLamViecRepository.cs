using Dapper;
using ECP_V2.Business.UnitOfWork;
using ECP_V2.DataAccess;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECP_V2.Business.Repository
{
    public class KeHoachLichLamViecRepository : RepositoryBase<plv_KeHoachLichLamViec>
    {
        public KeHoachLichLamViecRepository()
            : base()
        {
        }

        public KeHoachLichLamViecRepository(WorkUnit unit)
            : base(unit)
        {
        }

        public int AddNew(plv_KeHoachLichLamViec input)
        {
            int kt = 0;
            try
            {
                //    Context.plv_KeHoachLichLamViec.Add(input);
                //    Context.SaveChanges();
                kt = 1;
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var eve in ex.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;

                kt = -1;
            }
        
            return kt;
        }

        public override string Delete(object entityId, ref string strError)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                var entity = Context.tblTrams.SingleOrDefault(o => o.ID == id);
                Context.tblTrams.Remove(entity);
                Context.SaveChanges();
                strError = "";
                return "success";
            }
            catch (Exception ex) { strError = ex.Message; return "error"; }
        }

       

        public tblTram GetByName(string name)
        {
            try
            {
                var phongBan = Context.tblTrams.Where(p => p.SoPhieu.ToLower().Equals(name.Trim().ToLower())).Single();
                return phongBan;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

     

        public List<TramViewModel> ListPaging(int page, int pageSize, string filter,
        string PhongBanId, string Connectstr)
        {

            List<TramViewModel> lstData = new List<TramViewModel>();
            try
            {

                using (SqlConnection db = new SqlConnection(Connectstr))
                {
                    string query =
                        "select * from ( " +
                        "select ROW_NUMBER() OVER (ORDER BY t.[Id]) AS RowNum " +
                        ",* " +
                        ",TenPB=(select TenPhongBan from tblPhongBan where Id=PhongBanId) " +
                        ",TenDV=(select dv.TenDonVi from tblDonVi dv join tblPhongBan pb on pb.MaDVi=dv.Id where pb.Id=t.PhongBanId) " +
                        "from tblTram t " +
                        "where " +
                        "((t.Ten like REPLACE(N'%'+(N'" + filter + "')+N'%',' ',N'%') or N'" + filter + "'=N'')) ";
                        ;

                    if (!string.IsNullOrEmpty(PhongBanId))
                    {
                        query = query + "and t.PhongBanId = @PhongBanId ";
                    }

                    query = query +
                        ") as kq " +
                        "where RowNum BETWEEN ((@page-1)*@pageSize)+1 and @page*@pageSize"
                        ;

                    using (var multipleresult = db.QueryMultiple(query,
                        new
                        {
                            page = page,
                            pageSize = pageSize,
                            PhongBanId = PhongBanId,
                        }))
                    {
                        var q = multipleresult.Read<TramViewModel>();
                        lstData = q.ToList();
                    }
                }
            }
            catch (Exception ex) { }
            return lstData;
        }

        public int CountListPaging(string filter,
             string PhongBanId, string Connectstr)
        {

            int count = 0;
            try
            {

                using (SqlConnection db = new SqlConnection(Connectstr))
                {
                    string query = "select count(Id) " +
                        "from tblTram t " +
                        "where " +
                        "((t.Ten like REPLACE(N'%'+(N'" + filter + "')+N'%',' ',N'%') or N'" + filter + "'=N'')) ";
                        ;
                    if (!string.IsNullOrEmpty(PhongBanId))
                    {
                        query = query + "and t.PhongBanId = @PhongBanId ";
                    }


                    using (var multipleresult = db.QueryMultiple(query,
                        new
                        {
                            PhongBanId = PhongBanId,
                        }))
                    {
                        try
                        {
                            var q = multipleresult.Read<int>();
                            count = q.FirstOrDefault();
                        }
                        catch (Exception ex)
                        {
                            count = 0;
                        }


                    }
                }
            }
            catch (Exception ex) { }
            return count;
        }

        public object ImportList(List<tblTram> entity, ref string strError, out List<tblTram> lstFail)
        {
            lstFail = new List<tblTram>();
            try
            {
                Context.Configuration.AutoDetectChangesEnabled = false;
                Context.Configuration.ValidateOnSaveEnabled = false;
                entity = entity.Where(x => !Context.tblTrams.Any(y => y.ID == x.ID)).ToList();
                int addedCount = 0;
                foreach (var item in entity)
                {
                    Context.Entry(item).State = EntityState.Added;
                    try
                    {

                        Context.SaveChanges();
                        addedCount++;
                    }
                    catch (Exception ex)
                    {
                        lstFail.Add(item);
                        continue;
                    }
                }

                strError = "";
                return "Số bản ghi thêm được: " + addedCount;
            }
            catch (Exception ex)
            {
                strError = ex.Message;
                return 0;
            }
        }

        public override object Create(plv_KeHoachLichLamViec entity, ref string strError)
        {
            throw new NotImplementedException();
        }

        public override object Update(plv_KeHoachLichLamViec entity, ref string strError)
        {
            throw new NotImplementedException();
        }

        public override plv_KeHoachLichLamViec GetById(object entityId)
        {
            throw new NotImplementedException();
        }

        public override List<plv_KeHoachLichLamViec> List()
        {
            throw new NotImplementedException();
        }

        public override List<plv_KeHoachLichLamViec> ListByQuery(string strQuery)
        {
            throw new NotImplementedException();
        }

        public override List<plv_KeHoachLichLamViec> GetTableListById(object entityId)
        {
            throw new NotImplementedException();
        }
    }

  

}

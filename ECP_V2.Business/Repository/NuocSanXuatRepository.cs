using Dapper;
using ECP_V2.Business.UnitOfWork;
using ECP_V2.Business.ViewModels.PagingModel;
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
    public class NuocSanXuatRepository : RepositoryBase<NuocSanXuat>
    {
        public string Connectstr { get; set; }
        static string DBName { get; set; }

        public NuocSanXuatRepository(string connect)
            : base()
        {
            this.Connectstr = connect;
            try
            {
                var connection = new SqlConnection(Context.Database.Connection.ConnectionString);
                DBName = connection.Database;
            }
            catch (Exception ex)
            { }
        }

        public NuocSanXuatRepository()
            : base()
        {
        }

        public NuocSanXuatRepository(WorkUnit unit)
            : base(unit)
        {
        }

        public override string Delete(object entityId, ref string strError)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                var entity = Context.NuocSanXuats.SingleOrDefault(o => o.ID == id);
                Context.NuocSanXuats.Remove(entity);
                Context.SaveChanges();
                strError = "";
                return "success";
            }
            catch (Exception ex) { strError = ex.Message; return "error"; }
        }

        public NuocSanXuat Add(NuocSanXuat news)
        {
            Context.NuocSanXuats.Add(news);
            Context.SaveChanges();


            return news;
        }

        public override object Create(NuocSanXuat entity, ref string strError)
        {
            try
            {
                Context.Entry(entity).State = EntityState.Added;
                Context.SaveChanges();
                return entity.ID;
            }
            catch (Exception ex)
            {
                strError = ex.Message;
                return 0;
            }
        }

        public override NuocSanXuat GetById(object entityId)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                return Context.NuocSanXuats.SingleOrDefault(p => p.ID == id);
            }
            catch { return null; }
        }

        public NuocSanXuatViewModel GetInfoById(int entityId)
        {
            try
            {
                var entity = Context.NuocSanXuats.SingleOrDefault(p => p.ID == entityId);
                var model = new NuocSanXuatViewModel()
                {
                    ID = entity.ID,
                    Name = entity.Name
                };
                return model;
            }
            catch { return null; }
        }

        public override List<NuocSanXuat> List()
        {
            try
            {
                return Context.NuocSanXuats.ToList();
            }
            catch (Exception ex) { return null; }
        }

        public override List<NuocSanXuat> ListByQuery(string strQuery)
        {
            throw new NotImplementedException();
        }

        public override object Update(NuocSanXuat entity, ref string strError)
        {
            var model = Context.NuocSanXuats.SingleOrDefault(x => x.ID == entity.ID);
            if (model != null)
            {
                model.Name = entity.Name;
                Context.SaveChanges();
            }

            return model;
        }

        public override List<NuocSanXuat> GetTableListById(object entityId)
        {
            throw new NotImplementedException();
        }

        public int GetIdByName(string Name)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(Connectstr))
                {
                    string query = "select ID from NuocSanXuat where UPPER(Name) like N'%" + Name.ToUpper() + "%'";
                    var q = db.Query<int>(query);
                    return q.FirstOrDefault();
                }
            }
            catch (Exception ex) { return new int(); }
        }

        public bool CheckExistName(string name)
        {
            return Context.NuocSanXuats.FirstOrDefault(x => x.Name.Equals(name)) != null;
        }

        public PagedResult<NuocSanXuatViewModel> GetAllPaging(string keyword, int page, int pageSize)
        {
            var query = Context.NuocSanXuats.AsNoTracking().AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(x => x.Name.Contains(keyword));


            int totalRow = query.Count();

            query = query.OrderBy(x => x.Name).Skip((page - 1) * pageSize).Take(pageSize);

            var data = query.Select(x => new NuocSanXuatViewModel()
            {
                ID = x.ID,
                Name = x.Name
            }).ToList();

            var paginationSet = new PagedResult<NuocSanXuatViewModel>()
            {
                Results = data,
                CurrentPage = page,
                RowCount = totalRow,
                PageSize = pageSize
            };
            return paginationSet;
        }

    }

    public class NuocSanXuatViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }

    }
}

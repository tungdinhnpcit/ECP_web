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
    public class HangSanXuatRepository : RepositoryBase<HangSanXuat>
    {
        public string Connectstr { get; set; }
        static string DBName { get; set; }
        public HangSanXuatRepository(string connect)
            : base()
        {
            this.Connectstr = connect;
        }

        public HangSanXuatRepository()
            : base()
        {
            try
            {
                var connection = new SqlConnection(Context.Database.Connection.ConnectionString);
                DBName = connection.Database;
            }
            catch (Exception ex)
            { }
        }

        public HangSanXuatRepository(WorkUnit unit)
            : base(unit)
        {
        }

        public override string Delete(object entityId, ref string strError)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                var entity = Context.HangSanXuats.SingleOrDefault(o => o.ID == id);
                Context.HangSanXuats.Remove(entity);
                Context.SaveChanges();
                strError = "";
                return "success";
            }
            catch (Exception ex) { strError = ex.Message; return "error"; }
        }

        public HangSanXuat Add(HangSanXuat news)
        {
            Context.HangSanXuats.Add(news);
            Context.SaveChanges();


            return news;
        }

        public override object Create(HangSanXuat entity, ref string strError)
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

        public override HangSanXuat GetById(object entityId)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                return Context.HangSanXuats.SingleOrDefault(p => p.ID == id);
            }
            catch { return null; }
        }

        public HangSanXuatViewModel GetInfoById(int entityId)
        {
            try
            {
                var entity = Context.HangSanXuats.SingleOrDefault(p => p.ID == entityId);
                var model = new HangSanXuatViewModel()
                {
                    ID = entity.ID,
                    Name = entity.Name
                };
                return model;
            }
            catch { return null; }
        }

        public override List<HangSanXuat> List()
        {
            try
            {
                return Context.HangSanXuats.ToList();
            }
            catch (Exception ex) { return null; }
        }

        public override List<HangSanXuat> ListByQuery(string strQuery)
        {
            throw new NotImplementedException();
        }

        public override object Update(HangSanXuat entity, ref string strError)
        {
            var model = Context.HangSanXuats.SingleOrDefault(x => x.ID == entity.ID);
            if (model != null)
            {
                model.Name = entity.Name;
                Context.SaveChanges();
            }

            return model;
        }

        public override List<HangSanXuat> GetTableListById(object entityId)
        {
            throw new NotImplementedException();
        }

        public int GetIdByName(string Name)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(Connectstr))
                {
                    string query = "select ID from HangSanXuat where UPPER(Name) like N'%" + Name.ToUpper() + "%'";
                    var q = db.Query<int>(query);
                    return q.FirstOrDefault();
                }
            }
            catch (Exception ex) { return new int(); }
        }

        public bool CheckExistName(string name)
        {
            return Context.HangSanXuats.FirstOrDefault(x => x.Name.Equals(name)) != null;
        }

        public PagedResult<HangSanXuatViewModel> GetAllPaging(string keyword, int page, int pageSize)
        {
            var query = Context.HangSanXuats.AsNoTracking().AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(x => x.Name.Contains(keyword));


            int totalRow = query.Count();

            query = query.OrderBy(x => x.Name).Skip((page - 1) * pageSize).Take(pageSize);

            var data = query.Select(x => new HangSanXuatViewModel()
            {
                ID = x.ID,
                Name = x.Name
            }).ToList();

            var paginationSet = new PagedResult<HangSanXuatViewModel>()
            {
                Results = data,
                CurrentPage = page,
                RowCount = totalRow,
                PageSize = pageSize
            };
            return paginationSet;
        }
    }

    public class HangSanXuatViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }
}

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
    public class bcbs_ChiTieuRepository : RepositoryBase_V2
    {
        public string Connectstr { get; set; }
        public bcbs_ChiTieuRepository(string strConn)
        : base()
        {
            this.Connectstr = strConn;
        }

        public bcbs_ChiTieuRepository(WorkUnit unit)
            : base(unit)
        {
        }

        public bcbs_ChiTieu Add(bcbs_ChiTieu news)
        {
            Context.bcbs_ChiTieu.Add(news);
            Context.SaveChanges();

            return news;
        }

        public bcbs_ChiTieu Delete(int entityId, string user)
        {
            try
            {
                var entity = Context.bcbs_ChiTieu.SingleOrDefault(o => o.Id == entityId);
                entity.IsDelete = true;
                entity.NguoiXoa = user;
                entity.NgayXoa = DateTime.Now;

                Context.SaveChanges();
                return entity;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bcbs_ChiTieu GetById(int id)
        {
            using (IDbConnection db = new SqlConnection(Connectstr))
            {
                string query = "select * from bcbs_ChiTieu " +
                 "where Id = @Id";
                return db.Query<bcbs_ChiTieu>(query, new { Id = id }).FirstOrDefault();
            }

            //return Context.bcbs_ChiTieu.AsNoTracking().SingleOrDefault(x => x.Id == id);
        }

        public List<bcbs_ChiTieu> GetAll()
        {
            return Context.bcbs_ChiTieu.AsNoTracking().ToList();
        }

        public List<bcbs_ChiTieu> GetAllActive()
        {
            return Context.bcbs_ChiTieu.Where(x => x.IsDelete != true).AsNoTracking().ToList();
        }

    }

    public class bcbs_ChiTieuViewModel
    {
        public int Id { get; set; }
        public string TenChiTieu { get; set; }
        public string DonViTinh { get; set; }
        public string KieuDuLieu { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public string NguoiXoa { get; set; }
        public Nullable<System.DateTime> NgayXoa { get; set; }
    }
}

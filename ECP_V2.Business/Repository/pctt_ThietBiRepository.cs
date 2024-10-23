using Dapper;
using ECP_V2.Business.UnitOfWork;
using ECP_V2.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECP_V2.Business.Repository
{
    public class pctt_ThietBiRepository : RepositoryBase_V2
    {
        public string Connectstr { get; set; }
        public pctt_ThietBiRepository(string strConn)
         : base()
        {
            this.Connectstr = strConn;
        }

        public pctt_ThietBiRepository(WorkUnit unit)
            : base(unit)
        {
        }


        public pctt_ThietBi Add(pctt_ThietBi model)
        {
            Context.pctt_ThietBi.Add(model);
            Context.SaveChanges();

            return model;
        }

        public List<pctt_ThietBi> GetByOption(int nam, int type, string keyword, string donviId)
        {
            var query = Context.pctt_ThietBi.Where(x => x.Nam == nam && x.Type == type && x.IsDelete != true);

            if (!string.IsNullOrEmpty(donviId))
                query = query.Where(x => x.DonViId.Equals(donviId));

            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(x => x.Ten.Contains(keyword));

            return query.ToList();
        }

        public pctt_ThietBi Delete(int entityId, string user)
        {
            try
            {
                var entity = Context.pctt_ThietBi.SingleOrDefault(o => o.Id == entityId);
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

        public pctt_ThietBiViewModel GetById(int id)
        {
            using (IDbConnection db = new SqlConnection(Connectstr))
            {
                string query = "select * from pctt_ThietBi " +
                 "where Id = @Id";
                return db.Query<pctt_ThietBiViewModel>(query, new { Id = id }).FirstOrDefault();
            }

            //return Context.bcbs_NoiDung.AsNoTracking().SingleOrDefault(x => x.Id == id);
        }

        public List<pctt_ThietBi> GetAll()
        {
            return Context.pctt_ThietBi.AsNoTracking().ToList();
        }

        public void Update(pctt_ThietBi moduleVm)
        {

            var module = Context.pctt_ThietBi.SingleOrDefault(x => x.Id == moduleVm.Id);
            if (module != null)
            {
                module.DonViTinh = moduleVm.DonViTinh;
                module.GhiChu = moduleVm.GhiChu;
                module.NoiDe = moduleVm.NoiDe;
                module.SoLuong = moduleVm.SoLuong;
                module.Ten = moduleVm.Ten;
                module.ThuTu = moduleVm.ThuTu;

                Context.SaveChanges();
            }

        }

        public bool Update_ChuyenNPC(string donviId, int year, string user)
        {

            var model = Context.pctt_ThietBi.Where(x => x.Nam == year && x.DonViId == donviId && x.IsDelete != true && x.IsChuyenNPC != true).ToList();
            if (model != null && model.Count > 0)
            {
                foreach (var item in model)
                {
                    if (item.IsChuyenNPC != true)
                    {
                        item.IsChuyenNPC = true;
                        item.NgayChuyenNPC = DateTime.Now;
                        item.NguoiChuyenNPC = user;
                        item.TrangThai = 4;
                    }
                }

                Context.SaveChanges();

                return true;
            }
            return false;
        }
    }

    public class pctt_ThietBiViewModel
    {
        public int Id { get; set; }
        public string Ten { get; set; }
        public string DonViTinh { get; set; }
        public Nullable<double> SoLuong { get; set; }
        public string NoiDe { get; set; }
        public string GhiChu { get; set; }
        public Nullable<int> ThuTu { get; set; }
        public int Type { get; set; }
        public Nullable<bool> IsChuyenNPC { get; set; }
        public string NguoiChuyenNPC { get; set; }
        public Nullable<System.DateTime> NgayChuyenNPC { get; set; }
        public System.DateTime NgayTao { get; set; }
        public string NguoiTao { get; set; }
        public Nullable<System.DateTime> NgaySua { get; set; }
        public string NguoiSua { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public Nullable<System.DateTime> NgayXoa { get; set; }
        public string NguoiXoa { get; set; }
        public int TrangThai { get; set; }
        public int Nam { get; set; }
        public string DonViId { get; set; }
    }
}

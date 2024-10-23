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
    public class pctt_DoiXungKichRepository : RepositoryBase_V2
    {
        public string Connectstr { get; set; }
        public pctt_DoiXungKichRepository(string strConn)
         : base()
        {
            this.Connectstr = strConn;
        }

        public pctt_DoiXungKichRepository(WorkUnit unit)
            : base(unit)
        {
        }


        public pctt_DoiXungKich Add(pctt_DoiXungKich model)
        {
            Context.pctt_DoiXungKich.Add(model);
            Context.SaveChanges();

            return model;
        }

        public List<pctt_DoiXungKich> GetByOption(int nam, string keyword, string donviId)
        {
             var query = Context.pctt_DoiXungKich.Where(x => x.Nam == nam && x.IsDelete != true);

            if (!string.IsNullOrEmpty(donviId))
                query = query.Where(x => x.DonViId.Equals(donviId));

            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(x => x.HoTen.Contains(keyword));

            return query.ToList();
        }

        public pctt_DoiXungKich Delete(int entityId, string user)
        {
            try
            {
                var entity = Context.pctt_DoiXungKich.SingleOrDefault(o => o.Id == entityId);
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

        public pctt_DoiXungKichViewModel GetById(int id)
        {
            using (IDbConnection db = new SqlConnection(Connectstr))
            {
                string query = "select * from pctt_DoiXungKich " +
                 "where Id = @Id";
                return db.Query<pctt_DoiXungKichViewModel>(query, new { Id = id }).FirstOrDefault();
            }

            //return Context.bcbs_NoiDung.AsNoTracking().SingleOrDefault(x => x.Id == id);
        }

        public List<pctt_DoiXungKich> GetAll()
        {
            return Context.pctt_DoiXungKich.AsNoTracking().ToList();
        }

        public void Update(pctt_DoiXungKich moduleVm)
        {

            var module = Context.pctt_DoiXungKich.SingleOrDefault(x => x.Id == moduleVm.Id);
            if (module != null)
            {
                module.ChucDanh = moduleVm.ChucDanh;
                module.DonViId = moduleVm.DonViId;
                module.TenDonVi = moduleVm.TenDonVi;
                module.NgaySua = moduleVm.NgaySua;
                module.NguoiSua = moduleVm.NguoiSua;
                module.HoTen = moduleVm.HoTen;
                module.Nam = moduleVm.Nam;
                module.SoDienThoai = moduleVm.SoDienThoai;

                Context.SaveChanges();
            }

        }

        public bool Update_ChuyenNPC(int year, string donviId, string user)
        {
            var model = Context.pctt_DoiXungKich.Where(x => x.Nam == year && x.IsDelete != true);

            if (!string.IsNullOrEmpty(donviId))
                model = model.Where(x => x.DonViId.Equals(donviId));

            var data = model.ToList();

            if (data != null && data.Count > 0)
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

    public class pctt_DoiXungKichViewModel
    {
        public int Id { get; set; }
        public string DonViId { get; set; }
        public string TenDonVi { get; set; }
        public int Nam { get; set; }
        public string HoTen { get; set; }
        public string ChucDanh { get; set; }
        public string SoDienThoai { get; set; }
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
    }
}

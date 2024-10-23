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

    public class hlat_TaiNanRepository : RepositoryBase_V2
    {
        public string Connectstr { get; set; }
        public hlat_TaiNanRepository(string strConn)
         : base()
        {
            this.Connectstr = strConn;
        }

        public hlat_TaiNanRepository(WorkUnit unit)
            : base(unit)
        {
        }


        public hlat_TaiNan Add(hlat_TaiNan model)
        {
            Context.hlat_TaiNan.Add(model);
            Context.SaveChanges();

            return model;
        }


        public hlat_TaiNan Delete(int entityId, string user)
        {
            try
            {
                var entity = Context.hlat_TaiNan.SingleOrDefault(o => o.Id == entityId);
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

        public List<hlat_TaiNanViewModel> GetByOption(List<string> donvi, int quy, int nam)
        {
            var model = new List<hlat_TaiNanViewModel>();

            foreach (var item in donvi)
            {
                var temp = Context.hlat_TaiNan.Where(x => x.DonViId.Equals(item) && x.Quy == quy && x.Nam == nam && x.IsDelete != true);
                List<hlat_TaiNanViewModel> query = new List<hlat_TaiNanViewModel>();

                query = (from a in temp
                         join b in Context.hlat_CapDienAp.Where(x => x.TrangThai == true) on a.CapDienApId equals b.Id
                         join c in Context.tblDonVis on a.DonViId equals c.Id
                         select new hlat_TaiNanViewModel()
                         {
                             CapDienApId = a.CapDienApId,
                             Id = a.Id,
                             DonViId = a.DonViId,
                             TenDonVi = c.TenDonVi,
                             GhiChu = a.GhiChu,
                             HoTenNN = a.HoTenNN,
                             IsChuyenNPC = a.IsChuyenNPC,
                             IsDelete = a.IsDelete,
                             Nam = a.Nam,
                             NgayChuyenNPC = a.NgayChuyenNPC,
                             NgaySua = a.NgaySua,
                             NgayTao = a.NgayTao,
                             NgayXoa = a.NgayXoa,
                             NgayXayRa = a.NgayXayRa,
                             NgheNghiepNN = a.NgheNghiepNN,
                             NguoiChuyenNPC = a.NguoiChuyenNPC,
                             NguoiSua = a.NguoiSua,
                             NguoiTao = a.NguoiTao,
                             NguoiXoa = a.NguoiXoa,
                             NguyenNhan_DienBien = a.NguyenNhan_DienBien,
                             PhanLoai_NguyenNhan = a.PhanLoai_NguyenNhan,
                             Quy = a.Quy,
                             SoLuongVu = a.SoLuongVu,
                             TenCapDienAp = b.CapDienAp,
                             TinhTrang = a.TinhTrang,
                             TrangThai = a.TrangThai,
                             TuoiNN = a.TuoiNN,
                             DiaChi = a.DiaChi
                         }).OrderBy(x => x.DonViId).ThenByDescending(x => x.NgayXayRa).ThenBy(x => x.CapDienApId).ThenByDescending(x => x.NgayTao).ToList();


                model.AddRange(query);
            }



            return model;
        }

        public hlat_TaiNan GetById(int id)
        {
            using (IDbConnection db = new SqlConnection(Connectstr))
            {
                string query = "select * from hlat_TaiNan " +
                 "where Id = @Id";
                return db.Query<hlat_TaiNan>(query, new { Id = id }).FirstOrDefault();
            }

            //return Context.hlat_TaiNan.AsNoTracking().SingleOrDefault(x => x.Id == id);
        }

        public hlat_TaiNanViewModel GetInfoById(int id)
        {
            using (IDbConnection db = new SqlConnection(Connectstr))
            {
                string query = "select * from hlat_TaiNan " +
                 "where Id = @Id";
                return db.Query<hlat_TaiNanViewModel>(query, new { Id = id }).FirstOrDefault();
            }
        }

        public List<hlat_TaiNan> GetAll()
        {
            return Context.hlat_TaiNan.AsNoTracking().ToList();
        }

        public void Update(hlat_TaiNan moduleVm)
        {

            var module = Context.hlat_TaiNan.SingleOrDefault(x => x.Id == moduleVm.Id);
            if (module != null)
            {
                module.CapDienApId = moduleVm.CapDienApId;
                module.DiaChi = moduleVm.DiaChi;
                module.GhiChu = moduleVm.GhiChu;
                module.HoTenNN = moduleVm.HoTenNN;
                module.NgaySua = moduleVm.NgaySua;
                module.NgayXayRa = moduleVm.NgayXayRa;
                module.NgheNghiepNN = moduleVm.NgheNghiepNN;
                module.NguoiSua = moduleVm.NguoiSua;
                module.NguyenNhan_DienBien = moduleVm.NguyenNhan_DienBien;
                module.PhanLoai_NguyenNhan = moduleVm.PhanLoai_NguyenNhan;
                module.SoLuongVu = moduleVm.SoLuongVu;
                module.TinhTrang = moduleVm.TinhTrang;
                module.TuoiNN = moduleVm.TuoiNN;

                Context.SaveChanges();
            }

        }

        public bool Update_ChuyenNPC(int quarter, int year, List<string> listP, string user)
        {

            var model = Context.hlat_TaiNan.Where(x => x.Quy == quarter && x.Nam == year && x.IsDelete != true && listP.Contains(x.DonViId)).ToList();
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

    public class hlat_TaiNanViewModel
    {
        public int Id { get; set; }
        public string DonViId { get; set; }
        public string TenDonVi { get; set; }
        public string DiaChi { get; set; }
        public int Quy { get; set; }
        public int Nam { get; set; }
        public int CapDienApId { get; set; }
        public int TenCapDienAp { get; set; }
        public Nullable<int> SoLuongVu { get; set; }
        public string HoTenNN { get; set; }
        public Nullable<int> TuoiNN { get; set; }
        public string NgheNghiepNN { get; set; }
        public Nullable<System.DateTime> NgayXayRa { get; set; }
        public Nullable<int> PhanLoai_NguyenNhan { get; set; }
        public string NguyenNhan_DienBien { get; set; }
        public Nullable<int> TinhTrang { get; set; }
        public string GhiChu { get; set; }
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

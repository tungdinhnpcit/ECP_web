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
    public class cthl_KetQuaThiRepository : RepositoryBase_V2
    {
        public string Connectstr { get; set; }
        public cthl_KetQuaThiRepository(string strConn)
         : base()
        {
            this.Connectstr = strConn;
        }

        public cthl_KetQuaThiRepository(WorkUnit unit)
            : base(unit)
        {
        }


        public cthl_KetQuaThi Add(cthl_KetQuaThi model)
        {
            Context.cthl_KetQuaThi.Add(model);
            Context.SaveChanges();

            return model;
        }

        public cthl_AnhThe AddAnhThe(cthl_AnhThe model)
        {
            Context.cthl_AnhThe.Add(model);
            Context.SaveChanges();

            return model;
        }


        public cthl_KetQuaThi Delete(int entityId, string user)
        {
            try
            {
                var entity = Context.cthl_KetQuaThi.SingleOrDefault(o => o.Id == entityId);
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

        public cthl_AnhThe DeleteAnhThe(int entityId, string user)
        {
            try
            {
                var entity = Context.cthl_AnhThe.SingleOrDefault(o => o.Id == entityId);
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

        public List<cthl_KetQuaThiViewModel> GetByOption(int nam, int loaiKT)
        {
            var test = Context.cthl_KetQuaThi.Where(x => x.Nam == nam && x.LoaiKyThiId == loaiKT).ToList();
            var query = from a in Context.cthl_KetQuaThi.Where(x => x.Nam == nam && x.LoaiKyThiId == loaiKT)
                        join b in Context.tblNhanViens on a.NhanVienId equals b.Id
                        join c in Context.tblDonVis on b.DonViId equals c.Id
                        select new cthl_KetQuaThiViewModel()
                        {
                            BacAnToan = a.BacAnToan,
                            CapGCN = a.CapGCN,
                            ChucVu = b.ChucVu,
                            DonViHL = a.DonViHL,
                            DonViId = b.DonViId,
                            GhiChu = a.GhiChu,
                            HoTenNV = b.TenNhanVien,
                            Id = a.Id,
                            IsChuyenNPC = a.IsChuyenNPC,
                            IsDelete = a.IsDelete,
                            KetQuaSH = a.KetQuaSH,
                            KySHTiepTheo = a.KySHTiepTheo,
                            LoaiKyThiId = a.LoaiKyThiId,
                            Nam = a.Nam,
                            NgayChuyenNPC = a.NgayChuyenNPC,
                            NgayHL_BD = a.NgayHL_BD,
                            NgayHL_KT = a.NgayHL_KT,
                            NgaySH = a.NgaySH,
                            NgaySinh = b.NgaySinh,
                            NgaySua = a.NgaySua,
                            NgayTao = a.NgayTao,
                            NgayXoa = a.NgayXoa,
                            NguoiChuyenNPC = a.NguoiChuyenNPC,
                            NguoiSua = a.NguoiSua,
                            NguoiTao = a.NguoiTao,
                            NguoiXoa = a.NguoiXoa,
                            NhanVienId = a.NhanVienId,
                            NhomHL = a.NhomHL,
                            TenDonVi = c.TenDonVi
                        };

            return query.ToList();
        }

        public cthl_KetQuaThi GetById(int id)
        {
            using (IDbConnection db = new SqlConnection(Connectstr))
            {
                string query = "select * from cthl_KetQuaThi " +
                 "where Id = @Id";
                return db.Query<cthl_KetQuaThi>(query, new { Id = id }).FirstOrDefault();
            }

            //return Context.cthl_KetQuaThi.AsNoTracking().SingleOrDefault(x => x.Id == id);
        }

        public cthl_LoaiKyThi GetLoaiKTById(int id)
        {
            return Context.cthl_LoaiKyThi.Where(x => x.Id == id).FirstOrDefault();
        }

        public cthl_KetQuaThiViewModel GetInfoById(int id)
        {
            using (IDbConnection db = new SqlConnection(Connectstr))
            {
                string query = "select * from cthl_KetQuaThi " +
                 "where Id = @Id";
                return db.Query<cthl_KetQuaThiViewModel>(query, new { Id = id }).FirstOrDefault();
            }
        }

        public List<cthl_AnhThe> GetAllAnhTheById(int id)
        {
            return Context.cthl_AnhThe.Where(x => x.KetQuaThiId == id && x.IsDelete != true).ToList();
        }

        public List<cthl_LoaiKyThi> GetAllLoaiKyThi()
        {
            return Context.cthl_LoaiKyThi.ToList();
        }

        public List<NhanVienTemp> GetAllNhanVien()
        {
            return Context.tblNhanViens.Select(a => new NhanVienTemp
            {
                DonViId = a.DonViId,
                Id = a.Id,
                PhongBanId = a.PhongBanId,
                SoDT = a.SoDT,
                TenNhanVien = a.TenNhanVien,
                Username = a.Username
            }).ToList();
        }

        public List<cthl_KetQuaThi> GetAll()
        {
            return Context.cthl_KetQuaThi.AsNoTracking().ToList();
        }

        public void Update(cthl_KetQuaThi moduleVm)
        {

            var module = Context.cthl_KetQuaThi.SingleOrDefault(x => x.Id == moduleVm.Id);
            if (module != null)
            {
                module.CapGCN = moduleVm.CapGCN;
                module.DonViHL = moduleVm.DonViHL;
                module.GhiChu = moduleVm.GhiChu;
                module.KySHTiepTheo = moduleVm.KySHTiepTheo;
                module.NgayHL_BD = moduleVm.NgayHL_BD;
                module.NgayHL_KT = moduleVm.NgayHL_KT;
                module.NgaySH = moduleVm.NgaySH;
                module.NgaySua = moduleVm.NgaySua;
                module.NguoiSua = moduleVm.NguoiSua;
                module.NhomHL = moduleVm.NhomHL;
                module.NhanVienId = moduleVm.NhanVienId;
                module.BacAnToan = moduleVm.BacAnToan;

                Context.SaveChanges();
            }

        }

        public bool Update_ChuyenNPC(int year, int loaiKT, string user)
        {

            var model = Context.cthl_KetQuaThi.Where(x => x.Nam == year && x.LoaiKyThiId == loaiKT).ToList();
            if (model != null && model.Count > 0)
            {
                foreach (var item in model)
                {
                    if (item.IsChuyenNPC != true)
                    {
                        item.IsChuyenNPC = true;
                        item.NgayChuyenNPC = DateTime.Now;
                        item.NguoiChuyenNPC = user;

                        if (item.KetQuaSH)
                        {
                            var nv = Context.tblNhanViens.Where(x => x.Id.Equals(item.NhanVienId)).FirstOrDefault();
                            if (nv != null)
                            {
                                nv.BacAnToan = item.BacAnToan;
                            }
                        }
                        
                    }
                }

                Context.SaveChanges();

                return true;
            }
            return false;
        }
    }

    public class cthl_KetQuaThiViewModel
    {
        public int Id { get; set; }
        public int Nam { get; set; }
        public int LoaiKyThiId { get; set; }
        public string NhanVienId { get; set; }
        public string HoTenNV { get; set; }
        public DateTime NgaySinh { get; set; }
        public string SoCMT { get; set; }
        public string TrinhDo { get; set; }
        public string ChucVu { get; set; }
        public string DonViId { get; set; }
        public string TenDonVi { get; set; }
        public Nullable<int> NhomHL { get; set; }
        public string BacAnToan { get; set; }
        public Nullable<System.DateTime> NgayHL_BD { get; set; }
        public Nullable<System.DateTime> NgayHL_KT { get; set; }
        public Nullable<System.DateTime> NgaySH { get; set; }
        public bool KetQuaSH { get; set; }
        public string CapGCN { get; set; }
        public string DonViHL { get; set; }
        public string KySHTiepTheo { get; set; }
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
    }

    public class NhanVienTemp
    {
        public string Id { get; set; }
        public string TenNhanVien { get; set; }
        public string Username { get; set; }
        public string SoDT { get; set; }
        public Nullable<int> PhongBanId { get; set; }
        public string DonViId { get; set; }
    }
}

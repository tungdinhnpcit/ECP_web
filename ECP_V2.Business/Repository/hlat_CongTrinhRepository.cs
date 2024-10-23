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

    public class hlat_CongTrinhRepository : RepositoryBase_V2
    {
        public string Connectstr { get; set; }
        public hlat_CongTrinhRepository(string strConn)
         : base()
        {
            this.Connectstr = strConn;
        }

        public hlat_CongTrinhRepository(WorkUnit unit)
            : base(unit)
        {
        }


        public hlat_CongTrinh Add(hlat_CongTrinh model)
        {
            Context.hlat_CongTrinh.Add(model);
            Context.SaveChanges();

            return model;
        }


        public hlat_CongTrinh Delete(int entityId, string user)
        {
            try
            {
                var entity = Context.hlat_CongTrinh.SingleOrDefault(o => o.Id == entityId);
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

        public List<hlat_CongTrinhViewModel> GetByOption(List<string> donvi, int month, int year)
        {
            var model = new List<hlat_CongTrinhViewModel>();

            foreach (var item in donvi)
            {
                var temp = Context.hlat_CongTrinh.Where(x => x.DonViId.Equals(item) && x.Thang == month && x.Nam == year && x.IsDelete != true);
                List<hlat_CongTrinhViewModel> query = new List<hlat_CongTrinhViewModel>();

                query = (from a in (Context.hlat_CapDienAp.Where(x => x.TrangThai == true).ToList())
                         join b in temp on a.Id equals b.CapDienApId into ps
                         from p in ps.DefaultIfEmpty()
                         select new hlat_CongTrinhViewModel()
                         {
                             CapDienApId = a.Id,
                             CapNgam = p != null ? p.CapNgam : null,
                             Id = p != null ? p.Id : 0,
                             DanhGiaDoNguyHiemHRN = p != null ? p.DanhGiaDoNguyHiemHRN : null,
                             DonViId = p != null ? p.DonViId : item,
                             GhiChu = p != null ? p.GhiChu : null,
                             IsChuyenNPC = p != null ? p.IsChuyenNPC : null,
                             IsDelete = p != null ? p.IsDelete : null,
                             Nam = p != null ? p.Nam : year,
                             NgayChuyenNPC = p != null ? p.NgayChuyenNPC : null,
                             NgaySua = p != null ? p.NgaySua : null,
                             NgayTao = p != null ? p.NgayTao : DateTime.Now,
                             NgayXoa = p != null ? p.NgayXoa : null,
                             NguoiChuyenNPC = p != null ? p.NguoiChuyenNPC : null,
                             NguoiSua = p != null ? p.NguoiSua : null,
                             NguoiTao = p != null ? p.NguoiTao : null,
                             NguoiXoa = p != null ? p.NguoiXoa : null,
                             PhanLoai_Khoan1 = p != null ? p.PhanLoai_Khoan1 : null,
                             PhanLoai_Khoan2 = p != null ? p.PhanLoai_Khoan2 : null,
                             PhanLoai_Khoan3 = p != null ? p.PhanLoai_Khoan3 : null,
                             PhanLoai_Khoan5 = p != null ? p.PhanLoai_Khoan5 : null,
                             TangGiam_GiamDoCaiTao = p != null ? p.TangGiam_GiamDoCaiTao : null,
                             TangGiam_GiamDoPhoiHopDiaPhuong = p != null ? p.TangGiam_GiamDoPhoiHopDiaPhuong : null,
                             TangGiam_PhatSinhMoi = p != null ? p.TangGiam_PhatSinhMoi : null,
                             TenCapDienAp = a.CapDienAp,
                             Thang = p != null ? p.Thang : month,
                             Tong_DauNam = p != null ? p.Tong_DauNam : null,
                             Tong_LuyKe = p != null ? p.Tong_LuyKe : null,
                             TrangThai = p != null ? p.TrangThai : 1


                         }).OrderBy(x => x.DonViId).ThenBy(x => x.CapDienApId).ToList();


                model.AddRange(query);
            }



            return model;
        }


        public hlat_CongTrinh GetById(int id)
        {
            using (IDbConnection db = new SqlConnection(Connectstr))
            {
                string query = "select * from hlat_CongTrinh " +
                 "where Id = @Id";
                return db.Query<hlat_CongTrinh>(query, new { Id = id }).FirstOrDefault();
            }

            //return Context.hlat_CongTrinh.AsNoTracking().SingleOrDefault(x => x.Id == id);
        }

        public hlat_CongTrinhViewModel GetInfoById(int id)
        {
            using (IDbConnection db = new SqlConnection(Connectstr))
            {
                string query = "select * from hlat_CongTrinh " +
                 "where Id = @Id";
                return db.Query<hlat_CongTrinhViewModel>(query, new { Id = id }).FirstOrDefault();
            }
        }

        public List<hlat_CongTrinh> GetAll()
        {
            return Context.hlat_CongTrinh.AsNoTracking().ToList();
        }

        public void Update(hlat_CongTrinh moduleVm)
        {

            var module = Context.hlat_CongTrinh.SingleOrDefault(x => x.Id == moduleVm.Id);
            if (module != null)
            {
                module.CapNgam = moduleVm.CapNgam;
                module.DanhGiaDoNguyHiemHRN = moduleVm.DanhGiaDoNguyHiemHRN;
                module.GhiChu = moduleVm.GhiChu;
                module.NgaySua = moduleVm.NgaySua;
                module.NguoiSua = moduleVm.NguoiSua;
                module.PhanLoai_Khoan1 = moduleVm.PhanLoai_Khoan1;
                module.PhanLoai_Khoan2 = moduleVm.PhanLoai_Khoan2;
                module.PhanLoai_Khoan3 = moduleVm.PhanLoai_Khoan3;
                module.PhanLoai_Khoan5 = moduleVm.PhanLoai_Khoan5;
                module.TangGiam_GiamDoCaiTao = moduleVm.TangGiam_GiamDoCaiTao;
                module.TangGiam_GiamDoPhoiHopDiaPhuong = moduleVm.TangGiam_GiamDoPhoiHopDiaPhuong;
                module.TangGiam_PhatSinhMoi = moduleVm.TangGiam_PhatSinhMoi;
                module.Tong_DauNam = moduleVm.Tong_DauNam;
                module.Tong_LuyKe = moduleVm.Tong_LuyKe;

                Context.SaveChanges();
            }

        }

        public bool Update_ChuyenNPC(int month, int year, List<string> listP, string user)
        {

            var model = Context.hlat_CongTrinh.Where(x => x.Thang == month && x.Nam == year && x.IsDelete != true && listP.Contains(x.DonViId)).ToList();
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

        //public bool Update_Duyet(int id, string user)
        //{

        //    var model = Context.hlat_CongTrinh.SingleOrDefault(x => x.Id == id);
        //    if (model != null && (model.TrangThai == 1 || model.TrangThai == 3))
        //    {
        //        model.TrangThai = 2;

        //        Context.SaveChanges();

        //        return true;
        //    }
        //    return false;
        //}

        //public bool Update_ChuyenHoan(int id, string user)
        //{

        //    var model = Context.hlat_CongTrinh.SingleOrDefault(x => x.Id == id);
        //    if (model != null && model.TrangThai == 2)
        //    {
        //        model.TrangThai = 3;

        //        Context.SaveChanges();

        //        return true;
        //    }
        //    return false;
        //}
    }

    public class hlat_CongTrinhViewModel
    {
        public int Id { get; set; }
        public string DonViId { get; set; }
        public int Thang { get; set; }
        public int Nam { get; set; }
        public int CapDienApId { get; set; }
        public int TenCapDienAp { get; set; }
        public Nullable<int> Tong_DauNam { get; set; }
        public Nullable<int> Tong_LuyKe { get; set; }
        public Nullable<int> TangGiam_PhatSinhMoi { get; set; }
        public Nullable<int> TangGiam_GiamDoCaiTao { get; set; }
        public Nullable<int> TangGiam_GiamDoPhoiHopDiaPhuong { get; set; }
        public Nullable<double> PhanLoai_Khoan1 { get; set; }
        public Nullable<double> PhanLoai_Khoan2 { get; set; }
        public Nullable<double> PhanLoai_Khoan3 { get; set; }
        public Nullable<double> PhanLoai_Khoan5 { get; set; }
        public Nullable<double> CapNgam { get; set; }
        public Nullable<double> DanhGiaDoNguyHiemHRN { get; set; }
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

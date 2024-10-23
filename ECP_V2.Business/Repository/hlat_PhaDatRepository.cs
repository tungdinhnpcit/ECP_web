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
    public class hlat_PhaDatRepository : RepositoryBase_V2
    {
        public string Connectstr { get; set; }
        public hlat_PhaDatRepository(string strConn)
         : base()
        {
            this.Connectstr = strConn;
        }

        public hlat_PhaDatRepository(WorkUnit unit)
            : base(unit)
        {
        }


        public hlat_PhaDat Add(hlat_PhaDat model)
        {
            Context.hlat_PhaDat.Add(model);
            Context.SaveChanges();

            return model;
        }


        public hlat_PhaDat Delete(int entityId, string user)
        {
            try
            {
                var entity = Context.hlat_PhaDat.SingleOrDefault(o => o.Id == entityId);
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

        public List<hlat_PhaDatViewModel> GetByOption(List<string> donvi, int month, int year)
        {
            var model = new List<hlat_PhaDatViewModel>();

            foreach (var item in donvi)
            {
                var temp = Context.hlat_PhaDat.Where(x => x.DonViId.Equals(item) && x.Thang == month && x.Nam == year && x.IsDelete != true).ToList();
                List<hlat_PhaDatViewModel> query = new List<hlat_PhaDatViewModel>();

                query = (from a in (Context.hlat_CapDienAp.Where(x => x.TrangThai == true).ToList())
                         join b in temp on a.Id equals b.CapDienApId into ps
                         from p in ps.DefaultIfEmpty()
                         select new hlat_PhaDatViewModel()
                         {
                             CapDienApId = a.Id,
                             TenCapDienAp = a.CapDienAp,
                             CayCoi_DiemHRN = p != null ? p.CayCoi_DiemHRN : null,
                             CayCoi_NgoaiHL = p != null ? p.CayCoi_NgoaiHL : null,
                             CayCoi_TrongHL = p != null ? p.CayCoi_TrongHL : null,
                             DanhGiaDoNguyHiem = p != null ? p.DanhGiaDoNguyHiem : null,
                             DonViId = p != null ? p.DonViId : item,
                             DoVong = p != null ? p.DoVong : null,
                             GhiChu = p != null ? p.GhiChu : null,
                             Id = p != null ? p.Id : 0,
                             IsDelete = p != null ? p.IsDelete : null,
                             Nam = p != null ? p.Nam : year,
                             NgaySua = p != null ? p.NgaySua : null,
                             NgayTao = p != null ? p.NgayTao : DateTime.Now,
                             NgayXoa = p != null ? p.NgayXoa : null,
                             NguoiSua = p != null ? p.NguoiSua : null,
                             NguoiTao = p != null ? p.NguoiTao : null,
                             NguoiXoa = p != null ? p.NguoiXoa : null,
                             TangGiam_GiamDoCaiTao = p != null ? p.TangGiam_GiamDoCaiTao : null,
                             TangGiam_GiamDoPhoiHopDiaPhuong = p != null ? p.TangGiam_GiamDoPhoiHopDiaPhuong : null,
                             TangGiam_PhatSinhMoi = p != null ? p.TangGiam_PhatSinhMoi : null,
                             Thang = p != null ? p.Thang : month,
                             Tong_DauNam = p != null ? p.Tong_DauNam : null,
                             Tong_LuyKe = p != null ? p.Tong_LuyKe : null,
                             TrangThai = p != null ? p.TrangThai : 1,
                             IsChuyenNPC = p != null ? p.IsChuyenNPC : null,
                             NgayChuyenNPC = p != null ? p.NgayChuyenNPC : null,
                             NguoiChuyenNPC = p != null ? p.NguoiChuyenNPC : null
                            
                         }).OrderBy(x => x.DonViId).ThenBy(x => x.CapDienApId).ToList();


                model.AddRange(query);
            }



            return model;
        }

       
        public hlat_PhaDat GetById(int id)
        {
            using (IDbConnection db = new SqlConnection(Connectstr))
            {
                string query = "select * from hlat_PhaDat " +
                 "where Id = @Id";
                return db.Query<hlat_PhaDat>(query, new { Id = id }).FirstOrDefault();
            }

            //return Context.hlat_PhaDat.AsNoTracking().SingleOrDefault(x => x.Id == id);
        }

        public hlat_PhaDatViewModel GetInfoById(int id)
        {
            using (IDbConnection db = new SqlConnection(Connectstr))
            {
                string query = "select * from hlat_PhaDat " +
                 "where Id = @Id";
                return db.Query<hlat_PhaDatViewModel>(query, new { Id = id }).FirstOrDefault();
            }
        }

        public List<hlat_PhaDat> GetAll()
        {
            return Context.hlat_PhaDat.AsNoTracking().ToList();
        }

        public void Update(hlat_PhaDat moduleVm)
        {

            var module = Context.hlat_PhaDat.SingleOrDefault(x => x.Id == moduleVm.Id);
            if (module != null)
            {
                module.CayCoi_DiemHRN = moduleVm.CayCoi_DiemHRN;
                module.CayCoi_NgoaiHL = moduleVm.CayCoi_NgoaiHL;
                module.CayCoi_TrongHL = moduleVm.CayCoi_TrongHL;
                module.DanhGiaDoNguyHiem = moduleVm.DanhGiaDoNguyHiem;
                module.DoVong = moduleVm.DoVong;
                module.GhiChu = moduleVm.GhiChu;
                module.NgaySua = moduleVm.NgaySua;
                module.NguoiSua = moduleVm.NguoiSua;
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

            var model = Context.hlat_PhaDat.Where(x => x.Thang == month && x.Nam == year && x.IsDelete != true && listP.Contains(x.DonViId)).ToList();
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

        //    var model = Context.hlat_PhaDat.SingleOrDefault(x => x.Id == id);
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

        //    var model = Context.hlat_PhaDat.SingleOrDefault(x => x.Id == id);
        //    if (model != null && model.TrangThai == 2)
        //    {
        //        model.TrangThai = 3;

        //        Context.SaveChanges();

        //        return true;
        //    }
        //    return false;
        //}
    }

    public class hlat_PhaDatViewModel
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
        public string DoVong { get; set; }
        public string DanhGiaDoNguyHiem { get; set; }
        public Nullable<double> CayCoi_TrongHL { get; set; }
        public Nullable<double> CayCoi_NgoaiHL { get; set; }
        public Nullable<double> CayCoi_DiemHRN { get; set; }
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

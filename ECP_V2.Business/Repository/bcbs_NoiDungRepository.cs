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

    public class bcbs_NoiDungRepository : RepositoryBase_V2
    {
        public string Connectstr { get; set; }
        public bcbs_NoiDungRepository(string strConn)
         : base()
        {
            this.Connectstr = strConn;
        }

        public bcbs_NoiDungRepository(WorkUnit unit)
            : base(unit)
        {
        }


        public bcbs_NoiDung Add(bcbs_NoiDung model)
        {
            Context.bcbs_NoiDung.Add(model);
            Context.SaveChanges();

            var history = new bcbs_LichSu()
            {
                KhoiLuongVTTB = model.KhoiLuongVTTB,
                NgayHoanThanh = model.NgayHoanThanh,
                NgaySua = model.NgayTao,
                NguoiSua = model.NguoiTao,
                NoiDungId = model.Id,
                NoiDung = model.NoiDung,
                PhamVi = model.PhamVi,
                TongGiaTri = model.TongGiaTri,
                LyDo = null
            };

            Context.bcbs_LichSu.Add(history);
            Context.SaveChanges();

            return model;
        }


        public bcbs_NoiDung Delete(int entityId, string user)
        {
            try
            {
                var entity = Context.bcbs_NoiDung.SingleOrDefault(o => o.Id == entityId);
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

        public List<bcbs_NoiDungViewModel> GetAllByOption(string donviId, DateTime from, DateTime to)
        {
            using (IDbConnection db = new SqlConnection(Connectstr))
            {
                string query = $"select * from bcbs_NoiDung where (IsDelete = 'false' or IsDelete is null ) and DonViId='{donviId}' AND CONVERT(date, NgayHoanThanh) BETWEEN CONVERT(date, '{from.ToString("yyyy-MM-dd")}') AND CONVERT(date, '{to.ToString("yyyy-MM-dd")}') order by ThuTu,NgayTao";
                return (List<bcbs_NoiDungViewModel>)db.Query<bcbs_NoiDungViewModel>(query);
            }
        }

        public List<bcbs_NoiDungViewModel> GetAllByOption_V2(List<string> list, DateTime from, DateTime to, int status)
        {
            using (IDbConnection db = new SqlConnection(Connectstr))
            {
                var checkStatus = "";
                if (status == 1)
                    checkStatus = " AND TrangThai = 4";
                else if(status == 2)
                    checkStatus = " AND TrangThai != 4";

                var temp = list.Select(x => "'" + x + "'");
                string query = $"select * from bcbs_NoiDung where (IsDelete = 'false' or IsDelete is null ) and DonViId in ({String.Join(",", temp)}) AND CONVERT(date, NgayHoanThanh) BETWEEN CONVERT(date, '{from.ToString("yyyy-MM-dd")}') AND CONVERT(date, '{to.ToString("yyyy-MM-dd")}')" + checkStatus + " order by ThuTu,NgayTao";
                return (List<bcbs_NoiDungViewModel>)db.Query<bcbs_NoiDungViewModel>(query); 
            }
        }

        public PagedResult<bcbs_NoiDung> GetAllPaging(string keyword, int page, int pageSize)
        {
            var query = Context.bcbs_NoiDung.AsNoTracking().AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(x => x.NoiDung.Contains(keyword));


            int totalRow = query.Count();

            query = query.OrderByDescending(x => x.NgayTao)
                .Skip((page - 1) * pageSize).Take(pageSize);

            var data = query.ToList();

            var paginationSet = new PagedResult<bcbs_NoiDung>()
            {
                Results = data,
                CurrentPage = page,
                RowCount = totalRow,
                PageSize = pageSize
            };
            return paginationSet;
        }

        public bcbs_NoiDung GetById(int id)
        {
            using (IDbConnection db = new SqlConnection(Connectstr))
            {
                string query = "select * from bcbs_NoiDung " +
                 "where Id = @Id";
                return db.Query<bcbs_NoiDung>(query, new { Id = id }).FirstOrDefault();
            }

            //return Context.bcbs_NoiDung.AsNoTracking().SingleOrDefault(x => x.Id == id);
        }

        public bcbs_NoiDungViewModel GetInfoById(int id)
        {
            using (IDbConnection db = new SqlConnection(Connectstr))
            {
                string query = "select * from bcbs_NoiDung " +
                 "where Id = @Id";
                return db.Query<bcbs_NoiDungViewModel>(query, new { Id = id }).FirstOrDefault();
            }

            //var a = Context.bcbs_NoiDung.AsNoTracking().SingleOrDefault(x => x.Id == id);

            //var config = new NoiDungVeSinhViewModel()
            //{
            //    Ten = a.Ten,
            //    NgaySua = a.NgaySua,
            //    NgayTao = a.NgayTao,
            //    NguoiSua = a.NguoiSua,
            //    NguoiTao = a.NguoiTao,
            //    Id = a.Id,
            //    DonViTinh = a.DonViTinh,
            //    GhiChu = a.GhiChu,
            //    GiaTri = a.GiaTri,
            //    MaLoai = a.MaLoai,
            //    TrangThai = a.TrangThai,
            //    ThuTu = a.ThuTu,
            //    MaKyBaoCao = a.MaKyBaoCao
            //};

            //return config;
        }

        public List<bcbs_NoiDung> GetAll()
        {
            return Context.bcbs_NoiDung.AsNoTracking().ToList();
        }

        public void Update(bcbs_NoiDung moduleVm)
        {

            var module = Context.bcbs_NoiDung.SingleOrDefault(x => x.Id == moduleVm.Id);
            if (module != null)
            {
                module.KhoiLuongVTTB = moduleVm.KhoiLuongVTTB;
                module.NgayHoanThanh = moduleVm.NgayHoanThanh;
                module.NgaySua = moduleVm.NgaySua;
                module.NguoiSua = moduleVm.NguoiSua;
                module.NoiDung = moduleVm.NoiDung;
                module.PhamVi = moduleVm.PhamVi;
                module.ThuTu = moduleVm.ThuTu;
                module.TongGiaTri = moduleVm.TongGiaTri;

                Context.SaveChanges();
            }

        }

        public bcbs_NoiDung Update_V2(bcbs_NoiDung moduleVm, string lydo, ref decimal d)
        {

            var module = Context.bcbs_NoiDung.SingleOrDefault(x => x.Id == moduleVm.Id);
            if (module != null)
            {
                d = (moduleVm.TongGiaTri - module.TongGiaTri);

                module.KhoiLuongVTTB = moduleVm.KhoiLuongVTTB;
                module.NgayHoanThanh = moduleVm.NgayHoanThanh;
                module.NgaySua = moduleVm.NgaySua;
                module.NguoiSua = moduleVm.NguoiSua;
                module.NoiDung = moduleVm.NoiDung;
                module.PhamVi = moduleVm.PhamVi;
                module.ThuTu = moduleVm.ThuTu;
                module.TongGiaTri = moduleVm.TongGiaTri;

                Context.SaveChanges();

                var history = new bcbs_LichSu()
                {
                    KhoiLuongVTTB = module.KhoiLuongVTTB,
                    NgayHoanThanh = module.NgayHoanThanh,
                    NgaySua = module.NgaySua,
                    NguoiSua = module.NguoiSua,
                    NoiDungId = module.Id,
                    NoiDung = module.NoiDung,
                    PhamVi = module.PhamVi,
                    TongGiaTri = module.TongGiaTri,
                    LyDo = lydo
                };

                Context.bcbs_LichSu.Add(history);
                Context.SaveChanges();

                return module;
            }
            return null;
        }

        public bool Update_ChuyenNPC(int id, string user)
        {

            var model = Context.bcbs_NoiDung.SingleOrDefault(x => x.Id == id);
            if (model != null && model.IsChuyenNPC != true && model.TrangThai == 2)
            {
                model.IsChuyenNPC = true;
                model.NgayChuyenNPC = DateTime.Now;
                model.NguoiChuyenNPC = user;
                model.TrangThai = 4;

                Context.SaveChanges();

                return true;
            }
            return false;
        }

        public bool Update_Duyet(int id, string user)
        {

            var model = Context.bcbs_NoiDung.SingleOrDefault(x => x.Id == id);
            if (model != null && (model.TrangThai == 1 || model.TrangThai == 3))
            {
                model.TrangThai = 2;

                Context.SaveChanges();

                return true;
            }
            return false;
        }

        public bool Update_ChuyenHoan(int id, string user)
        {

            var model = Context.bcbs_NoiDung.SingleOrDefault(x => x.Id == id);
            if (model != null && model.TrangThai == 2)
            {
                model.TrangThai = 3;

                Context.SaveChanges();

                return true;
            }
            return false;
        }
    }

    public class bcbs_NoiDungViewModel
    {
        public int Id { get; set; }
        public string DonViId { get; set; }
        public int Tuan { get; set; }
        public int Thang { get; set; }
        public int Nam { get; set; }
        public int ThuTu { get; set; }
        public string NoiDung { get; set; }
        public string PhamVi { get; set; }
        public string KhoiLuongVTTB { get; set; }
        public Nullable<decimal> TongGiaTri { get; set; }
        public Nullable<System.DateTime> NgayHoanThanh { get; set; }
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
        public string LyDo { get; set; }
    }
}

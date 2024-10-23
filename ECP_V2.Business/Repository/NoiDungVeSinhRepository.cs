using Dapper;
using ECP_V2.Business.UnitOfWork;
using ECP_V2.Business.ViewModels.PagingModel;
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
    public class NoiDungVeSinhRepository : RepositoryBase_V2
    {
        public string Connectstr { get; set; }
        public NoiDungVeSinhRepository(string strConn)
         : base()
        {
            this.Connectstr = strConn;
        }

        public NoiDungVeSinhRepository(WorkUnit unit)
            : base(unit)
        {
        }


        public vs_NoiDungVeSinh Add(vs_NoiDungVeSinh news)
        {
            Context.vs_NoiDungVeSinh.Add(news);
            Context.SaveChanges();


            return news;
        }


        public vs_NoiDungVeSinh Delete(int entityId, string user)
        {
            try
            {
                var entity = Context.vs_NoiDungVeSinh.SingleOrDefault(o => o.Id == entityId);
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

        public List<vs_NoiDungVeSinh> GetAllByOption(string donviId, int thang, int nam)
        {
            using (IDbConnection db = new SqlConnection(Connectstr))
            {
                string query = $"select * from vs_NoiDungVeSinh where (IsDelete = 'false' or IsDelete is null ) and DonViId='{donviId}' AND Thang='{thang}' AND Nam='{nam}' order by ThuTu,NgayTao";
                return (List<vs_NoiDungVeSinh>)db.Query<vs_NoiDungVeSinh>(query);
            }
        }

        

        public vs_NoiDungVeSinh GetById(int id)
        {
            using (IDbConnection db = new SqlConnection(Connectstr))
            {
                string query = "select * from vs_NoiDungVeSinh " +
                 "where Id = @Id";
                return db.Query<vs_NoiDungVeSinh>(query, new { Id = id }).FirstOrDefault();
            }

            //return Context.vs_NoiDungVeSinh.AsNoTracking().SingleOrDefault(x => x.Id == id);
        }

        public vs_NoiDungVeSinh GetInfoById(int id)
        {
            using (IDbConnection db = new SqlConnection(Connectstr))
            {
                string query = "select * from vs_NoiDungVeSinh " +
                 "where Id = @Id";
                return db.Query<vs_NoiDungVeSinh>(query, new { Id = id }).FirstOrDefault();
            }

            //var a = Context.vs_NoiDungVeSinh.AsNoTracking().SingleOrDefault(x => x.Id == id);

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

        public List<vs_NoiDungVeSinh> GetAll()
        {
            return Context.vs_NoiDungVeSinh.AsNoTracking().ToList();
        }


        public double Update(vs_NoiDungVeSinh moduleVm)
        {

            var module = Context.vs_NoiDungVeSinh.SingleOrDefault(x => x.Id == moduleVm.Id);
            if (module != null)
            {
                var temp = (moduleVm.GiaTri - module.GiaTri);

                module.Ten = moduleVm.Ten;
                module.NgaySua = moduleVm.NgaySua;
                module.NguoiSua = moduleVm.NguoiSua;
                module.DonViTinh = moduleVm.DonViTinh;
                module.GhiChu = moduleVm.GhiChu;
                module.GiaTri = moduleVm.GiaTri;
                module.ThuTu = moduleVm.ThuTu;
                module.TrangThai = moduleVm.TrangThai;

                Context.SaveChanges();

                return temp;
            }
            return 0;
        }

        public bool Update_ChuyenNPC(int id, string user)
        {

            var model = Context.vs_NoiDungVeSinh.SingleOrDefault(x => x.Id == id);
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

            var model = Context.vs_NoiDungVeSinh.SingleOrDefault(x => x.Id == id);
            if (model != null && (model.TrangThai == 1 || model.TrangThai == 3))
            {
                model.TrangThai = 2;

                Context.SaveChanges();

                return true;
            }
            return false;
        }

        public bool Update_ChuyenHoan(int id, string user, string lydo)
        {

            var model = Context.vs_NoiDungVeSinh.SingleOrDefault(x => x.Id == id);
            if (model != null && model.TrangThai == 2)
            {
                model.TrangThai = 3;
                model.LyDo = lydo;

                Context.SaveChanges();

                return true;
            }
            return false;
        }
    }

    public class NoiDungVeSinhViewModel
    {
        public int Id { get; set; }
        public int Thang { get; set; }
        public int Nam { get; set; }
        public string DonViId { get; set; }
        public string Ten { get; set; }
        public Nullable<int> ThuTu { get; set; }
        public int MaLoai { get; set; }
        public double GiaTri { get; set; }
        public string DonViTinh { get; set; }
        public string GhiChu { get; set; }
        public System.DateTime NgayTao { get; set; }
        public string NguoiTao { get; set; }
        public Nullable<System.DateTime> NgaySua { get; set; }
        public string NguoiSua { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public Nullable<System.DateTime> NgayXoa { get; set; }
        public string NguoiXoa { get; set; }
        public Nullable<bool> IsChuyenNPC { get; set; }
        public string NguoiChuyenNPC { get; set; }
        public Nullable<System.DateTime> NgayChuyenNPC { get; set; }
        public int TrangThai { get; set; }
        public string LyDo { get; set; }
    }
}

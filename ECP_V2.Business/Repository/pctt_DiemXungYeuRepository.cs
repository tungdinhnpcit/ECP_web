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
    public class pctt_DiemXungYeuRepository : RepositoryBase_V2
    {
        public string Connectstr { get; set; }
        public pctt_DiemXungYeuRepository(string strConn)
         : base()
        {
            this.Connectstr = strConn;
        }

        public pctt_DiemXungYeuRepository(WorkUnit unit)
            : base(unit)
        {
        }


        public pctt_DiemXungYeu Add(pctt_DiemXungYeu news)
        {
            Context.pctt_DiemXungYeu.Add(news);
            Context.SaveChanges();


            return news;
        }


        public pctt_DiemXungYeu Delete(int entityId, string user)
        {
            try
            {
                var entity = Context.pctt_DiemXungYeu.SingleOrDefault(o => o.Id == entityId);
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

        public List<pctt_DiemXungYeuViewModel> GetAllByOption(List<string> list, int nam)
        {
            using (IDbConnection db = new SqlConnection(Connectstr))
            {
                var temp = list.Select(x => "'" + x + "'");
                string query = $"select * from pctt_DiemXungYeu where (IsDelete = 'false' or IsDelete is null ) and DonViId in ({String.Join(",", temp)}) AND Nam = {nam} order by DonViId, NgayTao";
                return (List<pctt_DiemXungYeuViewModel>)db.Query<pctt_DiemXungYeuViewModel>(query);
            }
        }

        public pctt_DiemXungYeu GetById(int id)
        {
            using (IDbConnection db = new SqlConnection(Connectstr))
            {
                string query = "select * from pctt_DiemXungYeu " +
                 "where Id = @Id";
                return db.Query<pctt_DiemXungYeu>(query, new { Id = id }).FirstOrDefault();
            }

            //return Context.pctt_DiemXungYeu.AsNoTracking().SingleOrDefault(x => x.Id == id);
        }

        public NoiDungVeSinhViewModel GetInfoById(int id)
        {
            using (IDbConnection db = new SqlConnection(Connectstr))
            {
                string query = "select * from pctt_DiemXungYeu " +
                 "where Id = @Id";
                return db.Query<NoiDungVeSinhViewModel>(query, new { Id = id }).FirstOrDefault();
            }
        }

         public List<pctt_LoaiDuongDayViewModel> GetAllLoaiDuongDay()
        {
            using (IDbConnection db = new SqlConnection(Connectstr))
            {
                string query = "select * from pctt_LoaiDuongDay";
                return (List<pctt_LoaiDuongDayViewModel>)db.Query<pctt_LoaiDuongDayViewModel>(query);
            }
        }

        public List<pctt_DiemXungYeu> GetAll()
        {
            return Context.pctt_DiemXungYeu.AsNoTracking().ToList();
        }

        public void Update(pctt_DiemXungYeu moduleVm)
        {

            var module = Context.pctt_DiemXungYeu.SingleOrDefault(x => x.Id == moduleVm.Id);
            if (module != null)
            {
                module.GhiChu = moduleVm.GhiChu;
                module.NgaySua = moduleVm.NgaySua;
                module.NguoiSua = moduleVm.NguoiSua;
                module.KHXL_BD = moduleVm.KHXL_BD;
                module.KHXL_KT = moduleVm.KHXL_KT;
                module.MucDo = moduleVm.MucDo;
                module.TenDuongDay = moduleVm.TenDuongDay;
                module.TinhTrang = moduleVm.TinhTrang;

                Context.SaveChanges();
            }
        }

        public bool Update_ChuyenNPC(int id, string user)
        {

            var model = Context.pctt_DiemXungYeu.SingleOrDefault(x => x.Id == id);
            if (model != null && model.IsChuyenNPC != true)
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

    }

    public class pctt_DiemXungYeuViewModel
    {
        public int Id { get; set; }
        public string DonViId { get; set; }
        public int Nam { get; set; }
        public int LoaiDuongDayId { get; set; }
        public string TenDuongDay { get; set; }
        public string TinhTrang { get; set; }
        public int MucDo { get; set; }
        public Nullable<System.DateTime> KHXL_BD { get; set; }
        public Nullable<System.DateTime> KHXL_KT { get; set; }
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

    public class pctt_LoaiDuongDayViewModel
    {
        public int Id { get; set; }
        public string TenLoai { get; set; }
        public int ThuTu { get; set; }
        public int TrangThai { get; set; }
    }
}

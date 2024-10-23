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
    public class vsld_QuanTracRepository : RepositoryBase_V2
    {
        public string Connectstr { get; set; }
        public vsld_QuanTracRepository(string strConn)
         : base()
        {
            this.Connectstr = strConn;
        }

        public vsld_QuanTracRepository(WorkUnit unit)
            : base(unit)
        {
        }


        public vsld_QuanTrac Add(vsld_QuanTrac model)
        {
            Context.vsld_QuanTrac.Add(model);
            Context.SaveChanges();

            return model;
        }


        public vsld_QuanTrac Delete(int entityId, string user)
        {
            try
            {
                var entity = Context.vsld_QuanTrac.SingleOrDefault(o => o.Id == entityId);
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

        public List<vsld_QuanTrac> GetByOption(int year, string keyword, string donviId)
        {
            var query = Context.vsld_QuanTrac.Where(x => x.Nam == year && x.IsDelete != true);

            if (!string.IsNullOrEmpty(donviId))
                query = query.Where(x => x.DonViId.Equals(donviId));

            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(x => x.DonVi.Contains(keyword));

            return query.ToList();
        }


        public vsld_QuanTrac GetById(int id)
        {
            using (IDbConnection db = new SqlConnection(Connectstr))
            {
                string query = "select * from vsld_QuanTrac " +
                 "where Id = @Id";
                return db.Query<vsld_QuanTrac>(query, new { Id = id }).FirstOrDefault();
            }

            //return Context.vsld_QuanTrac.AsNoTracking().SingleOrDefault(x => x.Id == id);
        }

        public vsld_QuanTracViewModel GetInfoById(int id)
        {
            using (IDbConnection db = new SqlConnection(Connectstr))
            {
                string query = "select * from vsld_QuanTrac " +
                 "where Id = @Id";
                return db.Query<vsld_QuanTracViewModel>(query, new { Id = id }).FirstOrDefault();
            }
        }

        public List<vsld_QuanTrac> GetAll()
        {
            return Context.vsld_QuanTrac.AsNoTracking().ToList();
        }

        public void Update(vsld_QuanTrac moduleVm)
        {

            var module = Context.vsld_QuanTrac.SingleOrDefault(x => x.Id == moduleVm.Id);
            if (module != null)
            {
                module.DonVi = moduleVm.DonVi;
                module.DonViId = moduleVm.DonViId;
                module.GhiChu = moduleVm.GhiChu;
                module.KQDK_BD1 = moduleVm.KQDK_BD1;
                module.KQDK_BD2 = moduleVm.KQDK_BD2;
                module.KQDK_BD3 = moduleVm.KQDK_BD3;
                module.KQDK_BD4 = moduleVm.KQDK_BD4;
                module.KQDK_ChiPhiDK = moduleVm.KQDK_ChiPhiDK;
                module.KQDK_DonViThucHien = moduleVm.KQDK_DonViThucHien;
                module.KQDK_TongMau = moduleVm.KQDK_TongMau;
                module.KQDK_VuotMuc = moduleVm.KQDK_VuotMuc;
                module.NgaySua = moduleVm.NgaySua;
                module.NguoiSua = moduleVm.NguoiSua;
                module.PLSK_Loai1_Nam = moduleVm.PLSK_Loai1_Nam;
                module.PLSK_Loai1_Nu = moduleVm.PLSK_Loai1_Nu;
                module.PLSK_Loai2_Nam = moduleVm.PLSK_Loai2_Nam;
                module.PLSK_Loai2_Nu = moduleVm.PLSK_Loai2_Nu;
                module.PLSK_Loai3_Nam = moduleVm.PLSK_Loai3_Nam;
                module.PLSK_Loai3_Nu = moduleVm.PLSK_Loai3_Nu;
                module.PLSK_Loai4_Nam = moduleVm.PLSK_Loai4_Nam;
                module.PLSK_Loai4_Nu = moduleVm.PLSK_Loai4_Nu;
                module.PLSK_Loai5_Nam = moduleVm.PLSK_Loai5_Nam;
                module.PLSK_Loai5_Nu = moduleVm.PLSK_Loai5_Nu;
                module.PLSK_Tong_Nam = moduleVm.PLSK_Tong_Nam;
                module.PLSK_Tong_Nu = moduleVm.PLSK_Tong_Nu;

                Context.SaveChanges();
            }

        }

        public bool Update_ChuyenNPC(int id, string user)
        {

            var model = Context.vsld_QuanTrac.SingleOrDefault(x => x.Id == id);
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

    public class vsld_QuanTracViewModel
    {
        public int Id { get; set; }
        public string DonVi { get; set; }
        public string DonViId { get; set; }
        public int Nam { get; set; }
        public Nullable<int> PLSK_Tong_Nam { get; set; }
        public Nullable<int> PLSK_Tong_Nu { get; set; }
        public Nullable<int> PLSK_Loai1_Nam { get; set; }
        public Nullable<int> PLSK_Loai1_Nu { get; set; }
        public Nullable<int> PLSK_Loai2_Nam { get; set; }
        public Nullable<int> PLSK_Loai2_Nu { get; set; }
        public Nullable<int> PLSK_Loai3_Nam { get; set; }
        public Nullable<int> PLSK_Loai3_Nu { get; set; }
        public Nullable<int> PLSK_Loai4_Nam { get; set; }
        public Nullable<int> PLSK_Loai4_Nu { get; set; }
        public Nullable<int> PLSK_Loai5_Nam { get; set; }
        public Nullable<int> PLSK_Loai5_Nu { get; set; }
        public Nullable<int> KQDK_TongMau { get; set; }
        public Nullable<int> KQDK_VuotMuc { get; set; }
        public Nullable<int> KQDK_BD1 { get; set; }
        public Nullable<int> KQDK_BD2 { get; set; }
        public Nullable<int> KQDK_BD3 { get; set; }
        public Nullable<int> KQDK_BD4 { get; set; }
        public Nullable<decimal> KQDK_ChiPhiDK { get; set; }
        public string KQDK_DonViThucHien { get; set; }
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

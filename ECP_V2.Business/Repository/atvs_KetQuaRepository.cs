using ECP_V2.Business.UnitOfWork;
using ECP_V2.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECP_V2.Business.Repository
{
    public class atvs_KetQuaRepository : RepositoryBase_V2
    {
        public string Connectstr { get; set; }
        public atvs_KetQuaRepository(string strConn)
         : base()
        {
            this.Connectstr = strConn;
        }

        public atvs_KetQuaRepository(WorkUnit unit)
            : base(unit)
        {
        }


        public atvs_KetQua Add(atvs_KetQua news)
        {
            Context.atvs_KetQua.Add(news);
            Context.SaveChanges();


            return news;
        }


        public atvs_KetQua Delete(int entityId)
        {
            try
            {
                var entity = Context.atvs_KetQua.SingleOrDefault(o => o.Id == entityId);
                Context.atvs_KetQua.Remove(entity);

                Context.SaveChanges();
                return entity;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<atvs_KetQuaViewModel> GetAllByOption(int nam, string donviId)
        {
            var listDM = Context.atvs_DanhMuc.Where(x => x.IsDelete != true && x.Type == 2);
            var listKH = Context.atvs_KetQua.Where(x => x.Nam == nam && x.DonViId.Equals(donviId));

            var query = from a in listDM
                        join b in listKH on a.Id equals b.DanhMucId into ps
                        from p in ps.DefaultIfEmpty()
                        select new atvs_KetQuaViewModel()
                        {
                            DanhMucId = p == null ? a.Id : p.DanhMucId,
                            DonViId = donviId,
                            GhiChu = p == null ? null : p.GhiChu,
                            Id = p == null ? 0 : p.Id,
                            IsChuyenNPC = p == null ? null : p.IsChuyenNPC,
                            IsDelete = p == null ? null : p.IsDelete,
                            Nam = nam,
                            NgayChuyenNPC = p == null ? null : p.NgayChuyenNPC,
                            NgaySua = p == null ? null : p.NgaySua,
                            NgayTao = p == null ? DateTime.Now : p.NgayTao,
                            NgayXoa = p == null ? null : p.NgayXoa,
                            NguoiChuyenNPC = p == null ? null : p.NguoiChuyenNPC,
                            NguoiSua = p == null ? null : p.NguoiSua,
                            NguoiTao = p == null ? null : p.NguoiTao,
                            NguoiXoa = p == null ? null : p.NguoiXoa,
                            SoLuong = p == null ? null : p.SoLuong,
                            TrangThai = p == null ? 0 : p.TrangThai,
                            CapCha = a.CapCha,
                            DonViTinh = a.DonViTinh,
                            IsRequired = a.IsRequired,
                            TenDanhMuc = a.Ten,
                            ThuTu = a.ThuTu,
                            Type = a.Type
                        };

            return query.OrderBy(x => x.ThuTu).ToList();
        }


        public atvs_KetQua GetById(int id)
        {
            return Context.atvs_KetQua.Where(x => x.Id == id).SingleOrDefault();
        }

        //public atvs_KetQuaViewModel GetInfoById(int id)
        //{

        //}

        public List<atvs_KetQua> GetAll()
        {
            return Context.atvs_KetQua.AsNoTracking().ToList();
        }

        public void Update(atvs_KetQua moduleVm)
        {

            var module = Context.atvs_KetQua.SingleOrDefault(x => x.Id == moduleVm.Id);
            if (module != null)
            {
                module.GhiChu = moduleVm.GhiChu;
                module.SoLuong = moduleVm.SoLuong;

                Context.SaveChanges();
            }
        }

        public void Update_ChuyenNPC(string donviId, int nam, string user)
        {
            var model = Context.atvs_KetQua.Where(x => x.DonViId.Equals(donviId) && x.Nam == nam).ToList();

            foreach (var item in model)
            {
                item.IsChuyenNPC = true;
                item.NgayChuyenNPC = DateTime.Now;
                item.NguoiChuyenNPC = user;
            }

            Context.SaveChanges();
        }

    }

    public class atvs_KetQuaViewModel
    {
        public int Id { get; set; }
        public string DonViId { get; set; }
        public int DanhMucId { get; set; }
        public Nullable<double> SoLuong { get; set; }
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
        public int Nam { get; set; }
        public string TenDanhMuc { get; set; }
        public string DonViTinh { get; set; }
        public double ThuTu { get; set; }
        public Nullable<int> CapCha { get; set; }
        public bool IsRequired { get; set; }
        public int Type { get; set; }
    }
}

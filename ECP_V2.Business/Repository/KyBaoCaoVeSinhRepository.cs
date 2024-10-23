using ECP_V2.Business.UnitOfWork;
using ECP_V2.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECP_V2.Business.Repository
{
    public class KyBaoCaoVeSinhRepository : RepositoryBase_V2
    {
        public KyBaoCaoVeSinhRepository()
         : base()
        {
        }

        public KyBaoCaoVeSinhRepository(WorkUnit unit)
            : base(unit)
        {
        }


        public KyBaoCaoVeSinhViewModel Add(vs_KyBaoCao a)
        {
            Context.vs_KyBaoCao.Add(a);
            Context.SaveChanges();

            return new KyBaoCaoVeSinhViewModel()
            {
                DonViId = a.DonViId,
                Id = a.Id,
                TrangThaiChot = a.TrangThaiChot,
                Thang = a.Thang,
                IsChuyenNPC = a.IsChuyenNPC,
                Nam = a.Nam,
                NgayChot = a.NgayChot,
                NgayChuyenNPC = a.NgayChuyenNPC,
                NguoiChot = a.NguoiChot,
                NguoiChuyenNPC = a.NguoiChuyenNPC
            };
        }

        public bool CheckExist(string donviId, int month, int year)
        {
            return Context.vs_KyBaoCao.FirstOrDefault(x => x.DonViId.Equals(donviId) && x.Thang == month && x.Nam == year) != null;
        }

        public bool Delete(int entityId, string userId)
        {
            try
            {
                var entity = Context.vs_KyBaoCao.SingleOrDefault(o => o.Id == entityId);
                Context.vs_KyBaoCao.Remove(entity);

                Context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public KyBaoCaoVeSinhViewModel GetById(int id)
        {
            var a = Context.vs_KyBaoCao.AsNoTracking().SingleOrDefault(x => x.Id == id);
            return new KyBaoCaoVeSinhViewModel()
            {
                DonViId = a.DonViId,
                Id = a.Id,
                TrangThaiChot = a.TrangThaiChot,
                Thang = a.Thang,
                IsChuyenNPC = a.IsChuyenNPC,
                Nam = a.Nam,
                NgayChot = a.NgayChot,
                NgayChuyenNPC = a.NgayChuyenNPC,
                NguoiChot = a.NguoiChot,
                NguoiChuyenNPC = a.NguoiChuyenNPC
            };

        }

        public KyBaoCaoVeSinhViewModel GetByOption(string donviId, int month, int year)
        {
            var a = Context.vs_KyBaoCao.AsNoTracking().SingleOrDefault(x => x.DonViId.Equals(donviId) && x.Thang == month && x.Nam == year);

            if (a != null)
            {
                return new KyBaoCaoVeSinhViewModel()
                {
                    DonViId = a.DonViId,
                    Id = a.Id,
                    TrangThaiChot = a.TrangThaiChot,
                    Thang = a.Thang,
                    IsChuyenNPC = a.IsChuyenNPC,
                    Nam = a.Nam,
                    NgayChot = a.NgayChot,
                    NgayChuyenNPC = a.NgayChuyenNPC,
                    NguoiChot = a.NguoiChot,
                    NguoiChuyenNPC = a.NguoiChuyenNPC
                };
            }
            else
            {
                return null;
            }
        }

        public KyBaoCaoVeSinhViewModel GetInfoById(int id)
        {
            var x = Context.vs_KyBaoCao.AsNoTracking().SingleOrDefault(a => a.Id == id);

            var config = new KyBaoCaoVeSinhViewModel()
            {
                DonViId = x.DonViId,
                IsChuyenNPC = x.IsChuyenNPC,
                NguoiChot = x.NguoiChot,
                NgayChot = x.NgayChot,
                TrangThaiChot = x.TrangThaiChot,
                NguoiChuyenNPC = x.NguoiChuyenNPC,
                NgayChuyenNPC = x.NgayChuyenNPC,
                Nam = x.Nam,
                Id = x.Id,
                Thang = x.Thang
            };

            //var createdby = Context.tblNhanViens.AsNoTracking().SingleOrDefault(x => x.Id.Equals(config.CreatedBy));
            //if (createdby != null)
            //{
            //    config.CreatedByFullName = createdby.TenNhanVien;
            //    config.CreatedByUserName = createdby.Username;
            //}

            //if (config.ModifiedBy != null)
            //{
            //    var modifiedby = Context.tblNhanViens.AsNoTracking().SingleOrDefault(x => x.Id.Equals(config.ModifiedBy));
            //    if (modifiedby != null)
            //    {
            //        config.ModifiedByFullName = modifiedby.TenNhanVien;
            //        config.ModifiedByUserName = modifiedby.Username;
            //    }
            //}

            //if (config.DeletedBy != null)
            //{
            //    var deletedby = Context.tblNhanViens.AsNoTracking().SingleOrDefault(x => x.Id.Equals(config.DeletedBy));
            //    if (deletedby != null)
            //    {
            //        config.DeletedByFullName = deletedby.TenNhanVien;
            //        config.DeletedByUserName = deletedby.Username;
            //    }
            //}

            return config;
        }

        public List<KyBaoCaoVeSinhViewModel> GetAll()
        {
            return Context.vs_KyBaoCao.AsNoTracking().OrderBy(x => x.Nam).ThenBy(x => x.Thang).Select(x => new KyBaoCaoVeSinhViewModel()
            {
                DonViId = x.DonViId,
                IsChuyenNPC = x.IsChuyenNPC,
                NguoiChot = x.NguoiChot,
                NgayChot = x.NgayChot,
                TrangThaiChot = x.TrangThaiChot,
                NguoiChuyenNPC = x.NguoiChuyenNPC,
                NgayChuyenNPC = x.NgayChuyenNPC,
                Nam = x.Nam,
                Id = x.Id,
                Thang = x.Thang
            }).ToList();
        }

        public List<int> GetListIdByOption(int kbcId, List<string> listDV)
        {
            var kbc = Context.vs_KyBaoCao.SingleOrDefault(x => x.Id == kbcId);

            var model = Context.vs_KyBaoCao.Where(x => x.Nam == kbc.Nam && x.Thang == kbc.Thang && listDV.Contains(x.DonViId)).Select(x => x.Id).ToList();

            return model;
        }

        public bool Update_ChuyenNPC(int kybaocaoId, bool status, string username)
        {

            var module = Context.vs_KyBaoCao.SingleOrDefault(x => x.Id == kybaocaoId);
            if (module != null)
            {
                module.IsChuyenNPC = status;
                if (status)
                {
                    module.NgayChuyenNPC = DateTime.Now;
                    module.NguoiChuyenNPC = username;
                }
                else
                {
                    module.NgayChuyenNPC = null;
                    module.NguoiChuyenNPC = null;
                }

                Context.SaveChanges();
                return true;
            }
            return false;
        }

        public bool Update_ChuyenNPC_CT(List<int> kybaocaoId, bool status, string username)
        {
            foreach (var item in kybaocaoId)
            {
                var module = Context.vs_KyBaoCao.SingleOrDefault(x => x.Id == item);
                if (module != null)
                {
                    module.IsChuyenNPC = status;
                    if (status)
                    {
                        module.NgayChuyenNPC = DateTime.Now;
                        module.NguoiChuyenNPC = username;
                    }
                    else
                    {
                        module.NgayChuyenNPC = null;
                        module.NguoiChuyenNPC = null;
                    }

                    Context.SaveChanges();
                }
            }
            return true;
        }

        public bool Update_ChotBaoCao(int kybaocaoId, bool status, string username)
        {

            var module = Context.vs_KyBaoCao.SingleOrDefault(x => x.Id == kybaocaoId);
            if (module != null)
            {
                module.TrangThaiChot = status;
                if (status)
                {
                    module.NgayChot = DateTime.Now;
                    module.NguoiChot = username;
                }
                else
                {
                    module.NgayChot = null;
                    module.NguoiChot = null;
                }

                Context.SaveChanges();
                return true;
            }
            return false;
        }

        public bool Update_ChotBaoCao_CT(List<int> kybaocaoId, bool status, string username)
        {
            foreach (var item in kybaocaoId)
            {
                var module = Context.vs_KyBaoCao.SingleOrDefault(x => x.Id == item);
                if (module != null)
                {
                    module.TrangThaiChot = status;
                    if (status)
                    {
                        module.NgayChot = DateTime.Now;
                        module.NguoiChot = username;
                    }
                    else
                    {
                        module.NgayChot = null;
                        module.NguoiChot = null;
                    }

                    Context.SaveChanges();
                }
            }

            return true;
        }
    }

    public class KyBaoCaoVeSinhViewModel
    {
        public int Id { get; set; }
        public string DonViId { get; set; }
        public int Thang { get; set; }
        public int Nam { get; set; }
        public Nullable<bool> IsChuyenNPC { get; set; }
        public string NguoiChuyenNPC { get; set; }
        public Nullable<System.DateTime> NgayChuyenNPC { get; set; }
        public Nullable<bool> TrangThaiChot { get; set; }
        public string NguoiChot { get; set; }
        public Nullable<System.DateTime> NgayChot { get; set; }

    }
}

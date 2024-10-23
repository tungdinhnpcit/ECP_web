using ECP_V2.Business.UnitOfWork;
using ECP_V2.Business.ViewModels.PagingModel;
using ECP_V2.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECP_V2.Business.Repository
{
    public class DanhMucLoaiVeSinhRepository : RepositoryBase_V2
    {
        public DanhMucLoaiVeSinhRepository()
         : base()
        {
        }

        public DanhMucLoaiVeSinhRepository(WorkUnit unit)
            : base(unit)
        {
        }


        public vs_DanhMucLoaiVeSinh Add(vs_DanhMucLoaiVeSinh news)
        {
            Context.vs_DanhMucLoaiVeSinh.Add(news);
            Context.SaveChanges();


            return news;
        }

        public bool CheckExistName(string name)
        {
            return Context.vs_DanhMucLoaiVeSinh.FirstOrDefault(x => x.Ten.Equals(name)) != null;
        }

        public bool Delete(int entityId, string userId)
        {
            try
            {
                var entity = Context.vs_DanhMucLoaiVeSinh.SingleOrDefault(o => o.Id == entityId);
                Context.vs_DanhMucLoaiVeSinh.Remove(entity);

                Context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public PagedResult<vs_DanhMucLoaiVeSinh> GetAllPaging(string keyword, int page, int pageSize)
        {
            var query = Context.vs_DanhMucLoaiVeSinh.AsNoTracking().AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(x => x.Ten.Contains(keyword));


            int totalRow = query.Count();

            query = query.OrderByDescending(x => x.NgayTao)
                .Skip((page - 1) * pageSize).Take(pageSize);

            var data = query.ToList();

            var paginationSet = new PagedResult<vs_DanhMucLoaiVeSinh>()
            {
                Results = data,
                CurrentPage = page,
                RowCount = totalRow,
                PageSize = pageSize
            };
            return paginationSet;
        }

        public vs_DanhMucLoaiVeSinh GetById(int id)
        {
            return Context.vs_DanhMucLoaiVeSinh.AsNoTracking().SingleOrDefault(x => x.Id == id);
        }

        public DanhMucLoaiVeSinhViewModel GetInfoById(int id)
        {
            var a = Context.vs_DanhMucLoaiVeSinh.AsNoTracking().SingleOrDefault(x => x.Id == id);

            var config = new DanhMucLoaiVeSinhViewModel()
            {
                Ten = a.Ten,
                NgaySua = a.NgaySua,
                NgayTao = a.NgayTao,
                NguoiSua = a.NguoiSua,
                NguoiTao = a.NguoiTao,
                Id = a.Id,
                ParentId = a.ParentId,
                LoaiDonViTinh = a.LoaiDonViTinh,
                TenNoiDung = a.TenNoiDung,
                ThuTu = a.ThuTu
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

        public List<DanhMucLoaiVeSinhViewModel> GetAll()
        {
            return Context.vs_DanhMucLoaiVeSinh.AsNoTracking().OrderBy(x => x.ParentId).ThenBy(x => x.ThuTu).Select(x => new DanhMucLoaiVeSinhViewModel()
            {
                ThuTu = x.ThuTu,
                Ten = x.Ten,
                NguoiTao = x.NguoiTao,
                NguoiSua = x.NguoiSua,
                Id = x.Id,
                NgaySua = x.NgaySua,
                NgayTao = x.NgayTao,
                ParentId = x.ParentId,
                LoaiDonViTinh = x.LoaiDonViTinh,
                TenNoiDung = x.TenNoiDung
            }).ToList();
        }

        public void Update(vs_DanhMucLoaiVeSinh moduleVm)
        {

            var module = Context.vs_DanhMucLoaiVeSinh.SingleOrDefault(x => x.Id == moduleVm.Id);
            if (module != null)
            {
                module.Ten = moduleVm.Ten;
                module.TenNoiDung = moduleVm.TenNoiDung;
                module.LoaiDonViTinh = moduleVm.LoaiDonViTinh;
                module.NgaySua = moduleVm.NgaySua;
                module.NguoiSua = moduleVm.NguoiSua;
                module.ParentId = moduleVm.ParentId;
                module.ThuTu = moduleVm.ThuTu;

                Context.SaveChanges();
            }

        }
    }

    public class DanhMucLoaiVeSinhViewModel
    {
        public int Id { get; set; }
        public string Ten { get; set; }
        public Nullable<int> ThuTu { get; set; }
        public Nullable<int> ParentId { get; set; }
        public string TenNoiDung { get; set; }
        public string LoaiDonViTinh { get; set; }
        public Nullable<System.DateTime> NgayTao { get; set; }
        public string NguoiTao { get; set; }
        public Nullable<System.DateTime> NgaySua { get; set; }
        public string NguoiSua { get; set; }
    }
}

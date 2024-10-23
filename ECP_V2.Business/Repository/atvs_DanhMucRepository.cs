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
    public class atvs_DanhMucRepository : RepositoryBase_V2
    {
        public atvs_DanhMucRepository()
         : base()
        {
        }

        public atvs_DanhMucRepository(WorkUnit unit)
            : base(unit)
        {
        }


        public atvs_DanhMuc Add(atvs_DanhMuc news)
        {
            Context.atvs_DanhMuc.Add(news);
            Context.SaveChanges();


            return news;
        }


        public bool Delete(int entityId, string userId)
        {
            try
            {
                var entity = Context.atvs_DanhMuc.SingleOrDefault(o => o.Id == entityId);
                entity.IsDelete = true;
                entity.NguoiXoa = userId;
                entity.NgayXoa = DateTime.Now;

                Context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<atvs_DanhMuc> GetAllParent()
        {
            return Context.atvs_DanhMuc.AsNoTracking().Where(x => x.CapCha == null && x.IsDelete != true).OrderBy(x => x.Type).ThenBy(x => x.ThuTu).ToList();
        }

        public PagedResult<atvs_DanhMuc> GetAllPaging(int? type, string keyword, int page, int pageSize)
        {
            var query = Context.atvs_DanhMuc.AsNoTracking().Where(x => x.IsDelete != true);

            if (type.HasValue)
                query = query.Where(x => x.Type == type);

            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(x => x.Ten.Contains(keyword));


            int totalRow = query.Count();

            query = query.OrderBy(x => x.ThuTu)
                .Skip((page - 1) * pageSize).Take(pageSize);

            var paginationSet = new PagedResult<atvs_DanhMuc>()
            {
                Results = query.ToList(),
                CurrentPage = page,
                RowCount = totalRow,
                PageSize = pageSize
            };
            return paginationSet;
        }

        public atvs_DanhMuc GetById(int id)
        {
            return Context.atvs_DanhMuc.AsNoTracking().SingleOrDefault(x => x.Id == id);
        }

      


        public void Update(atvs_DanhMuc moduleVm)
        {

            var module = Context.atvs_DanhMuc.SingleOrDefault(x => x.Id == moduleVm.Id);
            if (module != null)
            {
                module.CapCha = moduleVm.CapCha;
                module.DonViTinh = moduleVm.DonViTinh;
                module.IsRequired = moduleVm.IsRequired;
                module.Ten = moduleVm.Ten;
                module.ThuTu = moduleVm.ThuTu;
                module.Type = moduleVm.Type;

                Context.SaveChanges();
            }

        }
    }
}

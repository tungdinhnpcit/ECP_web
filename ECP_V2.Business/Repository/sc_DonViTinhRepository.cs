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
    public class sc_DonViTinhRepository : RepositoryBase_V2
    {
        public sc_DonViTinhRepository()
         : base()
        {
        }

        public sc_DonViTinhRepository(WorkUnit unit)
            : base(unit)
        {
        }


        public sc_DonViTinh Add(sc_DonViTinh news)
        {
            Context.sc_DonViTinh.Add(news);
            Context.SaveChanges();


            return news;
        }


        public bool Delete(int entityId)
        {
            try
            {
                var entity = Context.sc_DonViTinh.SingleOrDefault(o => o.Id == entityId);
                Context.sc_DonViTinh.Remove(entity);
                Context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public PagedResult<sc_DonViTinh> GetAllPaging(string keyword, int page, int pageSize)
        {
            var query = Context.sc_DonViTinh.AsNoTracking().AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(x => x.Ten.Contains(keyword));


            int totalRow = query.Count();

            query = query.OrderBy(x => x.ThuTu)
                .Skip((page - 1) * pageSize).Take(pageSize);

            var paginationSet = new PagedResult<sc_DonViTinh>()
            {
                Results = query.ToList(),
                CurrentPage = page,
                RowCount = totalRow,
                PageSize = pageSize
            };
            return paginationSet;
        }

        public sc_DonViTinh GetById(int id)
        {
            return Context.sc_DonViTinh.AsNoTracking().SingleOrDefault(x => x.Id == id);
        }


        public List<sc_DonViTinh> GetAll()
        {
            return Context.sc_DonViTinh.AsNoTracking().OrderBy(x => x.ThuTu).ToList();
        }

        public void Update(sc_DonViTinh moduleVm)
        {

            var module = Context.sc_DonViTinh.SingleOrDefault(x => x.Id == moduleVm.Id);
            if (module != null)
            {
                module.MoTa = moduleVm.MoTa;
                module.Ten = moduleVm.Ten;
                module.ThuTu = moduleVm.ThuTu;

                Context.SaveChanges();
            }

        }
    }
}

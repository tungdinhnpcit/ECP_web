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
    public class sc_NhaSanXuatRepository : RepositoryBase_V2
    {
        public sc_NhaSanXuatRepository()
         : base()
        {
        }

        public sc_NhaSanXuatRepository(WorkUnit unit)
            : base(unit)
        {
        }


        public sc_NhaSanXuat Add(sc_NhaSanXuat news)
        {
            Context.sc_NhaSanXuat.Add(news);
            Context.SaveChanges();


            return news;
        }


        public bool Delete(int entityId)
        {
            try
            {
                var entity = Context.sc_NhaSanXuat.SingleOrDefault(o => o.Id == entityId);
                Context.sc_NhaSanXuat.Remove(entity);
                Context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public PagedResult<sc_NhaSanXuat> GetAllPaging(string keyword, int page, int pageSize)
        {
            var query = Context.sc_NhaSanXuat.AsNoTracking().AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(x => x.Ten.Contains(keyword));


            int totalRow = query.Count();

            query = query.OrderBy(x => x.ThuTu)
                .Skip((page - 1) * pageSize).Take(pageSize);

            var paginationSet = new PagedResult<sc_NhaSanXuat>()
            {
                Results = query.ToList(),
                CurrentPage = page,
                RowCount = totalRow,
                PageSize = pageSize
            };
            return paginationSet;
        }

        public sc_NhaSanXuat GetById(int id)
        {
            return Context.sc_NhaSanXuat.AsNoTracking().SingleOrDefault(x => x.Id == id);
        }

        public List<sc_NhaSanXuat> GetAll()
        {
            return Context.sc_NhaSanXuat.AsNoTracking().OrderBy(x => x.ThuTu).ToList();
        }

        public void Update(sc_NhaSanXuat moduleVm)
        {

            var module = Context.sc_NhaSanXuat.SingleOrDefault(x => x.Id == moduleVm.Id);
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

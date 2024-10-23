using ECP_V2.Business.UnitOfWork;
using ECP_V2.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECP_V2.Business.Repository
{

    public class vs_HinhAnhRepository : RepositoryBase_V2
    {
        public string Connectstr { get; set; }
        public vs_HinhAnhRepository(string strConn)
         : base()
        {
            this.Connectstr = strConn;
        }

        public vs_HinhAnhRepository(WorkUnit unit)
            : base(unit)
        {
        }


        public vs_HinhAnh Add(vs_HinhAnh news)
        {
            Context.vs_HinhAnh.Add(news);
            Context.SaveChanges();


            return news;
        }


        public vs_HinhAnh Delete(int entityId)
        {
            try
            {
                var entity = Context.vs_HinhAnh.SingleOrDefault(o => o.Id == entityId);
                Context.vs_HinhAnh.Remove(entity);

                Context.SaveChanges();
                return entity;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<vs_HinhAnh> GetAllByOption(int noidungId, int type)
        {
            

            return Context.vs_HinhAnh.Where(x => x.NoiDungId == noidungId && x.Type == type).OrderBy(x => x.DateCreated).ToList();
        }


        public vs_HinhAnh GetById(int id)
        {
            return Context.vs_HinhAnh.Where(x => x.Id == id).SingleOrDefault();
        }

        public vs_HinhAnh GetById_V2(int id)
        {
            return Context.vs_HinhAnh.AsNoTracking().Where(x => x.Id == id).SingleOrDefault();
        }

        public List<vs_HinhAnh> GetAll()
        {
            return Context.vs_HinhAnh.AsNoTracking().ToList();
        }


    }

}

using ECP_V2.Business.UnitOfWork;
using ECP_V2.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECP_V2.Business.Repository
{
    public class vs_CommentRepository : RepositoryBase_V2
    {
        public vs_CommentRepository()
         : base()
        {
        }

        public vs_CommentRepository(WorkUnit unit)
            : base(unit)
        {
        }


        public vs_Comment Add(vs_Comment news)
        {
            Context.vs_Comment.Add(news);
            Context.SaveChanges();


            return news;
        }


        public bool Delete(int entityId, string userId)
        {
            try
            {
                var entity = Context.vs_Comment.SingleOrDefault(o => o.Id == entityId);
                Context.vs_Comment.Remove(entity);
                Context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<vs_CommentViewModel> GetAllByBaoCaoId(int id)
        {
            var model = (from a in Context.vs_Comment.Where(x => x.BaoCaoId == id)
                         join b in Context.tblNhanViens on a.NguoiTao equals b.Id
                         orderby a.NgayTao
                         select new vs_CommentViewModel()
                         { 
                            Avatar = b.UrlImage,
                            BaoCaoId = a.BaoCaoId,
                            NgayTao = a.NgayTao, 
                            Id = a.Id,
                            FullName = b.TenNhanVien,
                            NguoiTao = a.NguoiTao,
                            NoiDung = a.NoiDung,
                            Type = a.Type,
                            UserName = b.Username
                         }).ToList();

            return model;

        }

        public vs_Comment GetById(int id)
        {
            return Context.vs_Comment.AsNoTracking().SingleOrDefault(x => x.Id == id);
        }



    }

    public class vs_CommentViewModel
    {
        public int Id { get; set; }
        public int BaoCaoId { get; set; }
        public string NoiDung { get; set; }
        public int Type { get; set; }
        public System.DateTime NgayTao { get; set; }
        public string NguoiTao { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Avatar { get; set; }
    }
}

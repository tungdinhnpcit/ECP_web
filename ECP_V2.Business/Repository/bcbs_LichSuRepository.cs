using ECP_V2.Business.UnitOfWork;
using ECP_V2.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECP_V2.Business.Repository
{
    public class bcbs_LichSuRepository : RepositoryBase_V2
    {
        public string Connectstr { get; set; }
        public bcbs_LichSuRepository(string strConn)
         : base()
        {
            this.Connectstr = strConn;
        }

        public bcbs_LichSuRepository(WorkUnit unit)
            : base(unit)
        {
        }


        public bcbs_LichSu Add(bcbs_LichSu news)
        {
            Context.bcbs_LichSu.Add(news);
            Context.SaveChanges();

            return news;
        }

        public List<bcbs_LichSu> GetAllByNoiDungId(int id)
        {
            var model = Context.bcbs_LichSu.Where(x => x.NoiDungId == id).OrderByDescending(x => x.NgaySua).ToList();

            return model;
        }
    }

    public class bcbs_LichSuViewModel
    {
        public int Id { get; set; }
        public Nullable<int> NoiDungId { get; set; }
        public string NoiDung { get; set; }
        public string PhamVi { get; set; }
        public string KhoiLuongVTTB { get; set; }
        public decimal TongGiaTri { get; set; }
        public Nullable<System.DateTime> NgayHoanThanh { get; set; }
        public Nullable<System.DateTime> NgaySua { get; set; }
        public string NguoiSua { get; set; }
        public string LyDo { get; set; }
    }
}

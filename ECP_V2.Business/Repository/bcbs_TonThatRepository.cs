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
    public class bcbs_TonThatRepository : RepositoryBase_V2
    {
        public string Connectstr { get; set; }
        public bcbs_TonThatRepository(string strConn)
         : base()
        {
            this.Connectstr = strConn;
        }

        public bcbs_TonThatRepository(WorkUnit unit)
            : base(unit)
        {
        }


        public bcbs_TonThat Add(bcbs_TonThat news)
        {
            Context.bcbs_TonThat.Add(news);
            Context.SaveChanges();

            return news;
        }


        public bcbs_TonThat Delete(int entityId, string user)
        {
            try
            {
                var entity = Context.bcbs_TonThat.SingleOrDefault(o => o.Id == entityId);
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

        public List<bcbs_TonThatViewModel> GetByOption(string donviId, int week, int year)
        {
            var temp = Context.bcbs_TonThat.Where(x => x.DonViId.Equals(donviId) && x.Tuan == week && x.Nam == year);
            List<bcbs_TonThatViewModel> query = new List<bcbs_TonThatViewModel>();
            //if (temp.Count() > 0)
            //{
            //    query = (from a in Context.bcbs_TonThat.Where(x => x.DonViId.Equals(donviId) && x.Tuan == week && x.Nam == year)
            //             join b in Context.bcbs_ChiTieu on a.ChiTieuId equals b.Id
            //             select new bcbs_TonThatViewModel()
            //             {
            //                 ChiTieuId = a.ChiTieuId,
            //                 DonViId = a.DonViId,
            //                 DonViTinh = b.DonViTinh,
            //                 Id = a.Id,
            //                 IsChuyenNPC = a.IsChuyenNPC,
            //                 IsDelete = a.IsDelete,
            //                 KieuDuLieu = b.KieuDuLieu,
            //                 Nam = a.Nam,
            //                 NgayChuyenNPC = a.NgayChuyenNPC,
            //                 NgaySua = a.NgaySua,
            //                 NgayTao = a.NgayTao,
            //                 NgayXoa = a.NgayXoa,
            //                 NguoiChuyenNPC = a.NguoiChuyenNPC,
            //                 NguoiSua = a.NguoiSua,
            //                 NguoiTao = a.NguoiTao,
            //                 NguoiXoa = a.NguoiXoa,
            //                 SauXuLy = a.SauXuLy,
            //                 TenChiTieu = b.TenChiTieu,
            //                 Thang = a.Thang,
            //                 TruocXuLy = a.TruocXuLy,
            //                 Tuan = a.Tuan
            //             }).ToList();
            //}
            //else
            //{
            //    query = (from a in Context.bcbs_ChiTieu.Where(x => x.IsDelete != true)
            //             select new bcbs_TonThatViewModel()
            //             {
            //                 ChiTieuId = a.Id,
            //                 DonViId = donviId,
            //                 DonViTinh = a.DonViTinh,
            //                 Id = 0,
            //                 KieuDuLieu = a.KieuDuLieu,
            //                 Nam = year,
            //                 TenChiTieu = a.TenChiTieu,
            //                 Tuan = week
            //             }).ToList();
            //}

            query = (from a in Context.bcbs_ChiTieu.Where(x => x.IsDelete != true)
                     join b in temp on a.Id equals b.ChiTieuId into ps
                     from p in ps.DefaultIfEmpty()
                     select new bcbs_TonThatViewModel()
                     {
                         ChiTieuId = a.Id,
                         DonViId = p != null ? p.DonViId : donviId,
                         DonViTinh = a.DonViTinh,
                         Id = p != null ? p.Id : 0,
                         IsChuyenNPC = p != null ? p.IsChuyenNPC : null,
                         IsDelete = p != null ? p.IsDelete : null,
                         KieuDuLieu = a.KieuDuLieu,
                         Nam = p != null ? p.Nam : year,
                         NgayChuyenNPC = p != null ? p.NgayChuyenNPC : null,
                         NgaySua = p != null ? p.NgaySua : null,
                         NgayTao = p != null ? p.NgayTao : DateTime.Now,
                         NgayXoa = p != null ? p.NgayXoa : null,
                         NguoiChuyenNPC = p != null ? p.NguoiChuyenNPC : null,
                         NguoiSua = p != null ? p.NguoiSua : null,
                         NguoiTao = p != null ? p.NguoiTao : null,
                         NguoiXoa = p != null ? p.NguoiXoa : null,
                         SauXuLy = p != null ? p.SauXuLy : null,
                         TenChiTieu = a.TenChiTieu,
                         Thang = p != null ? p.Thang : 1,
                         TruocXuLy = p != null ? p.TruocXuLy : null,
                         Tuan = p != null ? p.Tuan : week
                     }).ToList();


            return query;
        }

        public void Update(bcbs_TonThat moduleVm)
        {

            var module = Context.bcbs_TonThat.SingleOrDefault(x => x.Id == moduleVm.Id);
            if (module != null)
            {
                module.NgaySua = moduleVm.NgaySua;
                module.NguoiSua = moduleVm.NguoiSua;
                module.SauXuLy = moduleVm.SauXuLy;
                module.TruocXuLy = moduleVm.TruocXuLy;

                Context.SaveChanges();
            }
        }

        public void UpdateChuyenNPC(int week, int year, string donviId, string user)
        {
            var date = DateTime.Now;
            var list = Context.bcbs_TonThat.Where(x => x.Tuan == week && x.Nam == year && x.DonViId.Equals(donviId)).ToList();
            foreach (var item in list)
            {
                if (item.IsChuyenNPC != true)
                {
                    item.IsChuyenNPC = true;
                    item.NguoiChuyenNPC = user;
                    item.NgayChuyenNPC = date;

                    Context.SaveChanges();
                }
            }
        }
    }

    public class bcbs_TonThatViewModel
    {
        public int Id { get; set; }
        public string DonViId { get; set; }
        public Nullable<int> Tuan { get; set; }
        public int Thang { get; set; }
        public int Nam { get; set; }
        public int ChiTieuId { get; set; }

        public string TenChiTieu { get; set; }
        public string DonViTinh { get; set; }
        public string KieuDuLieu { get; set; }

        public string TruocXuLy { get; set; }
        public string SauXuLy { get; set; }
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
    }
}

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
    public class sc_ThietBiSuCoRepository : RepositoryBase_V2
    {
        public sc_ThietBiSuCoRepository()
         : base()
        {
        }

        public sc_ThietBiSuCoRepository(WorkUnit unit)
            : base(unit)
        {
        }


        public sc_ThietBiSuCo Add(sc_ThietBiSuCo news)
        {
            Context.sc_ThietBiSuCo.Add(news);
            Context.SaveChanges();


            return news;
        }


        public bool Delete(int entityId, string userId)
        {
            try
            {
                var entity = Context.sc_ThietBiSuCo.SingleOrDefault(o => o.Id == entityId);
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
        
        public bool DeleteAllBySuCo(int scId, string userId)
        {
            try
            {
                var list = Context.sc_ThietBiSuCo.Where(o => o.SuCoId == scId && o.IsDelete != true).ToList();
                foreach(var entity in list)
                {
                    entity.IsDelete = true;
                    entity.NguoiXoa = userId;
                    entity.NgayXoa = DateTime.Now;
                }
                

                Context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public PagedResult<sc_ThietBiSuCo> GetAllPaging(int scId, string keyword, int page, int pageSize)
        {
            var query = Context.sc_ThietBiSuCo.AsNoTracking().Where(x => x.SuCoId == scId && x.IsDelete != true);

            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(x => x.SoHD.Contains(keyword) || x.BienBanNT.Contains(keyword) || x.TenDonViThiCong.Contains(keyword) || x.LichSuVanHanh.Contains(keyword) || x.MoiTruongVH.Contains(keyword) || x.GhiChu.Contains(keyword));


            int totalRow = query.Count();

            query = query.OrderByDescending(x => x.Id)
                .Skip((page - 1) * pageSize).Take(pageSize);

            var paginationSet = new PagedResult<sc_ThietBiSuCo>()
            {
                Results = query.ToList(),
                CurrentPage = page,
                RowCount = totalRow,
                PageSize = pageSize
            };
            return paginationSet;
        }

        public sc_ThietBiSuCo GetById(int id)
        {
            return Context.sc_ThietBiSuCo.AsNoTracking().SingleOrDefault(x => x.Id == id);
        }




        public void Update(sc_ThietBiSuCo moduleVm)
        {

            var module = Context.sc_ThietBiSuCo.SingleOrDefault(x => x.Id == moduleVm.Id);
            if (module != null)
            {
                module.BienBanNT = moduleVm.BienBanNT;
                module.DonViTinhId = moduleVm.DonViTinhId;
                module.GhiChu = moduleVm.GhiChu;
                module.LichSuVanHanh = moduleVm.LichSuVanHanh;
                module.MaHieuId = moduleVm.MaHieuId;
                module.MoiTruongVH = moduleVm.MoiTruongVH;
                module.NgayDongDien = moduleVm.NgayDongDien;
                module.NgaySanXuat = moduleVm.NgaySanXuat;
                module.NgaySua = moduleVm.NgaySua;
                module.NguoiSua = moduleVm.NguoiSua;
                module.NhaSanXuatId = moduleVm.NhaSanXuatId;
                module.SoHD = moduleVm.SoHD;
                module.SoLuong = moduleVm.SoLuong;
                module.SuCoId = moduleVm.SuCoId;
                module.TenDonViThiCong = moduleVm.TenDonViThiCong;

                Context.SaveChanges();
            }

        }
    }
}

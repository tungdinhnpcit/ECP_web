using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECP_V2.DataAccess;
using System.Data;
using ECP_V2.Business.UnitOfWork;
using System.Data.Entity;
using System.Data.SqlClient;
using Dapper;
using ECP_V2.Common.Helpers;

namespace ECP_V2.Business.Repository
{
    public class NhanVienRepository : RepositoryBase<tblNhanVien>
    {
        string DBName { get; set; }
        public NhanVienRepository()
            : base()
        {
            try
            {
                var connection = new SqlConnection(Context.Database.Connection.ConnectionString);
                DBName = connection.Database;
            }
            catch (Exception ex)
            { }
        }

        public NhanVienRepository(WorkUnit unit)
            : base(unit)
        {
        }
        public override object Create(tblNhanVien entity, ref string strError)
        {
            try
            {
                Context.Entry(entity).State = EntityState.Added;
                Context.SaveChanges();

                var datacache = MemoryCacheHelper.GetValue(DBName + "_NhanVienGetAll");
                if (datacache != null)
                {
                    MemoryCacheHelper.Delete(DBName + "_NhanVienGetAll");
                }
                MemoryCacheHelper.Add(DBName + "_NhanVienGetAll", Context.tblNhanViens.ToList(), DateTimeOffset.UtcNow.AddMonths(1));

                return entity.Id;

            }
            catch (Exception ex)
            {
                strError = ex.Message;
                return null;
            }
        }


        public override string Delete(object entityId, ref string strError)
        {
            try
            {
                string id = entityId.ToString();
                var entity = Context.tblNhanViens.FirstOrDefault(o => o.Id == id);
                Context.tblNhanViens.Remove(entity);
                Context.SaveChanges();
                strError = "";

                var datacache = MemoryCacheHelper.GetValue(DBName + "_NhanVienGetAll");
                if (datacache != null)
                {
                    MemoryCacheHelper.Delete(DBName + "_NhanVienGetAll");
                }
                MemoryCacheHelper.Add(DBName + "_NhanVienGetAll", Context.tblNhanViens.ToList(), DateTimeOffset.UtcNow.AddMonths(1));

                return "success";
            }
            catch (Exception ex)
            {
                strError = ex.Message;
                return "error";
            }
        }

        public override tblNhanVien GetById(object entityId)
        {
            try
            {
                string id = entityId.ToString().Trim();
                var datacache = MemoryCacheHelper.GetValue(DBName + "_NhanVienGetAll");
                if (datacache != null)
                {
                    var lst = (List<tblNhanVien>)datacache;
                    return lst.SingleOrDefault(o => o.Id == id);
                }
                else
                {
                    var lst = Context.tblNhanViens.ToList();
                    MemoryCacheHelper.Add(DBName + "_NhanVienGetAll", lst, DateTimeOffset.UtcNow.AddMonths(1));
                    return lst.SingleOrDefault(p => p.Id == id);
                }
            }
            catch { return null; }
        }
        public tblNhanVien GetByUserName(string username)
        {
            try
            {
                var datacache = MemoryCacheHelper.GetValue(DBName + "_NhanVienGetAll");
                if (datacache != null)
                {
                    var lst = (List<tblNhanVien>)datacache;
                    return lst.SingleOrDefault(o => o.Username.ToUpper() == username.ToUpper());
                }
                else
                {
                    var lst = Context.tblNhanViens.ToList();
                    MemoryCacheHelper.Add(DBName + "_NhanVienGetAll", lst, DateTimeOffset.UtcNow.AddMonths(1));
                    return lst.SingleOrDefault(p => p.Username.ToUpper() == username.ToUpper());
                }
            }
            catch { return null; }
        }

        public tblNhanVien GetByEmail(string email)
        {
            try
            {
                var datacache = MemoryCacheHelper.GetValue(DBName + "_NhanVienGetAll");
                if (datacache != null)
                {
                    var lst = (List<tblNhanVien>)datacache;
                    return lst.SingleOrDefault(o => o.Email == email);
                }
                else
                {
                    var lst = Context.tblNhanViens.ToList();
                    MemoryCacheHelper.Add(DBName + "_NhanVienGetAll", lst, DateTimeOffset.UtcNow.AddMonths(1));
                    return lst.SingleOrDefault(p => p.Email == email);
                }
            }
            catch { return null; }
        }

        public tblNhanVien GetByDienThoai(string soDienThoai)
        {
            try
            {
                var datacache = MemoryCacheHelper.GetValue(DBName + "_NhanVienGetAll");
                if (datacache != null)
                {
                    var lst = (List<tblNhanVien>)datacache;
                    return lst.SingleOrDefault(o => o.SoDT == soDienThoai);
                }
                else
                {
                    var lst = Context.tblNhanViens.ToList();
                    MemoryCacheHelper.Add(DBName + "_NhanVienGetAll", lst, DateTimeOffset.UtcNow.AddMonths(1));
                    return lst.SingleOrDefault(p => p.SoDT == soDienThoai);
                }
            }
            catch { return null; }
        }

        public override List<tblNhanVien> List()
        {
            try
            {
                var datacache = MemoryCacheHelper.GetValue(DBName + "_NhanVienGetAll");
                if (datacache != null)
                {
                    var lst = (List<tblNhanVien>)datacache;
                    return lst;
                }
                else
                {
                    var lst = Context.tblNhanViens.ToList();
                    MemoryCacheHelper.Add(DBName + "_NhanVienGetAll", lst, DateTimeOffset.UtcNow.AddMonths(1));
                    return lst;
                }
            }
            catch (Exception ex) { return null; }
        }

        public List<tblNhanVien> ListByListId(List<string> ids)
        {
            try
            {
                var datacache = MemoryCacheHelper.GetValue(DBName + "_NhanVienGetAll");
                if (datacache != null)
                {
                    var lst = (List<tblNhanVien>)datacache;
                    return lst.Where(x => ids.Contains(x.Id)).ToList();
                }
                else
                {
                    var lst = Context.tblNhanViens.ToList();
                    MemoryCacheHelper.Add(DBName + "_NhanVienGetAll", lst, DateTimeOffset.UtcNow.AddMonths(1));
                    return lst.Where(x => ids.Contains(x.Id)).ToList();
                }
            }
            catch (Exception ex) { return null; }
        }

        public List<tblNhanVien> ListByDonViId(string ids)
        {
            try
            {
                var datacache = MemoryCacheHelper.GetValue(DBName + "_NhanVienGetAll");
                if (datacache != null)
                {
                    var lst = (List<tblNhanVien>)datacache;
                    return lst.Where(x => x.DonViId == ids).ToList();
                }
                else
                {
                    var lst = Context.tblNhanViens.ToList();
                    MemoryCacheHelper.Add(DBName + "_NhanVienGetAll", lst, DateTimeOffset.UtcNow.AddMonths(1));
                    return lst.Where(x => x.DonViId == ids).ToList();
                }
            }
            catch (Exception ex) { return null; }
        }

        public override List<tblNhanVien> ListByQuery(string strQuery)
        {
            throw new NotImplementedException();
        }

        public List<tblNhanVien> ListNhanVienByRoleId(string roleId)
        {
            try
            {
                //var datacache = MemoryCacheHelper.GetValue(DBName + "_ListNhanVienByRoleId_" + roleId);
                //if (datacache != null)
                //{
                //    return (List<tblNhanVien>)datacache;
                //}
                //else
                //{
                using (var db = new ECP_V2Entities())
                {
                    var result = db.Database.SqlQuery<tblNhanVien>("EXEC NPC_sp_AdvancedSearchNhanVienByRole @roleId",
                new SqlParameter("@roleId", roleId)).ToList();

                    //MemoryCacheHelper.Add(DBName + "_ListNhanVienByRoleId_" + roleId, result, DateTimeOffset.UtcNow.AddMonths(1));
                    return result;
                }
                //}
            }
            catch (Exception ex) { return null; }
        }

        public List<tblNhanVien> ListNhanVienNoInRoleTypeSystem(string roleId)
        {
            try
            {
                //var datacache = MemoryCacheHelper.GetValue(DBName + "_ListNhanVienNoInRoleTypeSystem_" + roleId);
                //if (datacache != null)
                //{
                //    return (List<tblNhanVien>)datacache;
                //}
                //else
                //{
                var result = Context.Database.SqlQuery<tblNhanVien>("EXEC NPC_sp_AdvancedSearchNhanVienNotInRole @roleId",
         new SqlParameter("@roleId", roleId)).ToList();

                //MemoryCacheHelper.Add(DBName + "_ListNhanVienNoInRoleTypeSystem_" + roleId, result, DateTimeOffset.UtcNow.AddMonths(1));
                return result;
                //}
            }
            catch (Exception ex) { return null; }
        }

        public List<tblNhanVien> ListNhanVienNoInRoleTypeFunction(string roleId)
        {
            try
            {
                //var datacache = MemoryCacheHelper.GetValue(DBName + "_ListNhanVienNoInRoleTypeFunction_" + roleId);
                //if (datacache != null)
                //{
                //    return (List<tblNhanVien>)datacache;
                //}
                //else
                //{
                var result = Context.Database.SqlQuery<tblNhanVien>("EXEC NPC_sp_AdvancedSearchNhanVienNotInRoleTypeFunction @roleId",
            new SqlParameter("@roleId", roleId)).ToList();

                //MemoryCacheHelper.Add(DBName + "_ListNhanVienNoInRoleTypeFunction_" + roleId, result, DateTimeOffset.UtcNow.AddMonths(1));
                return result;
                //}
            }
            catch (Exception ex) { return null; }
        }

        public string GetDonViByUser(string userName, ref string strError)
        {
            try
            {
                var datacache = MemoryCacheHelper.GetValue(DBName + "_NhanVienGetAll");
                if (datacache != null)
                {
                    var lst = (List<tblNhanVien>)datacache;
                    return lst.FirstOrDefault(x => x.Username.ToUpper() == userName.ToUpper()).DonViId;
                }
                else
                {
                    var lst = Context.tblNhanViens.ToList();
                    MemoryCacheHelper.Add(DBName + "_NhanVienGetAll", lst, DateTimeOffset.UtcNow.AddMonths(1));
                    return lst.FirstOrDefault(x => x.Username.ToUpper() == userName.ToUpper()).DonViId;
                }
            }
            catch (Exception ex) { return null; }
        }

        public override object Update(tblNhanVien entity, ref string strError)
        {
            try
            {
                Context.Entry(entity).State = EntityState.Modified;
                Context.SaveChanges();

                var datacache = MemoryCacheHelper.GetValue(DBName + "_NhanVienGetAll");
                if (datacache != null)
                {
                    MemoryCacheHelper.Delete(DBName + "_NhanVienGetAll");
                }
                MemoryCacheHelper.Add(DBName + "_NhanVienGetAll", Context.tblNhanViens.ToList(), DateTimeOffset.UtcNow.AddMonths(1));

                return entity.Id;
            }
            catch (Exception ex)
            {
                strError = ex.Message;
                return null;
            }
        }

        public string UpdateV2(tblNhanVien entity, ref string strError)
        {
            try
            {
                var connection = new SqlConnection(Context.Database.Connection.ConnectionString);
                List<tblNhanVien> lst = new List<tblNhanVien>();
                using (IDbConnection db = new SqlConnection(connection.ConnectionString))
                {
                    string query = "update tblNhanVien set DonViId = @DonViId , PhongBanId = @PhongBanId,TenNhanVien=@TenNhanVien,SoDT=@SoDT,Email=@Email,DiaChi=@DiaChi,ChucVu=@ChucVu,BacAnToan=@BacAnToan, Hsm_Serial=@Hsm_serial, ChuKySo=@ChuKySo, Hsm_Type=1, NhaMangSDT = @NhaMangSDT where Id = @Id";
                    db.Execute(query, new
                    {
                        DonViId = entity.DonViId,
                        PhongBanId = entity.PhongBanId,
                        TenNhanVien = entity.TenNhanVien,
                        SoDT = entity.SoDT,
                        Email = entity.Email,
                        DiaChi = entity.DiaChi,
                        ChucVu = entity.ChucVu,
                        BacAnToan = entity.BacAnToan,
                        Id = entity.Id,
                        Hsm_serial = entity.Hsm_serial,
                        Hsm_type=entity.Hsm_type,
                        ChuKySo = entity.ChuKySo,
                        NhaMangSDT = entity.NhaMangSDT,
                    });
                }

                using (IDbConnection db = new SqlConnection(connection.ConnectionString))
                {
                    string query = "select * from tblNhanVien";
                    lst = db.Query<tblNhanVien>(query).ToList();
                }

                var datacache = MemoryCacheHelper.GetValue(DBName + "_NhanVienGetAll");
                if (datacache != null)
                {
                    MemoryCacheHelper.Delete(DBName + "_NhanVienGetAll");
                }
                MemoryCacheHelper.Add(DBName + "_NhanVienGetAll", lst, DateTimeOffset.UtcNow.AddMonths(1));

                return entity.Id;
            }
            catch (Exception ex)
            {
                strError = ex.Message;
                return null;
            }
        }

        public override List<tblNhanVien> GetTableListById(object entityId)
        {
            throw new NotImplementedException();
        }

        public bool UpdateChuKySo(string id, byte[] ChuKySo, ref string strError)
        {
            try
            {
                var nhanVien = (from p in Context.tblNhanViens
                                where p.Id == id
                                select p).SingleOrDefault();
                if (nhanVien != null)
                {
                    nhanVien.ChuKySo = ChuKySo;
                }
                Context.SaveChanges();

                var datacache = MemoryCacheHelper.GetValue(DBName + "_NhanVienGetAll");
                if (datacache != null)
                {
                    MemoryCacheHelper.Delete(DBName + "_NhanVienGetAll");
                }
                MemoryCacheHelper.Add(DBName + "_NhanVienGetAll", Context.tblNhanViens.ToList(), DateTimeOffset.UtcNow.AddMonths(1));


                return true;
            }
            catch (Exception ex)
            {
                strError = ex.Message;
                return false;
            }
        }

        public bool CheckChuKySo(string id, ref string strError)
        {
            try
            {
                var datacache = MemoryCacheHelper.GetValue(DBName + "_NhanVienGetAll");
                if (datacache != null)
                {
                    var lst = (List<tblNhanVien>)datacache;
                    var nhanVien = lst.SingleOrDefault(o => o.Id == id);
                    if (nhanVien != null)
                    {
                        if (nhanVien.ChuKySo != null)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    var lst = Context.tblNhanViens.ToList();
                    MemoryCacheHelper.Add(DBName + "_NhanVienGetAll", lst, DateTimeOffset.UtcNow.AddMonths(1));
                    var nhanVien = lst.SingleOrDefault(o => o.Id == id);
                    if (nhanVien != null)
                    {
                        if (nhanVien.ChuKySo != null)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                strError = ex.Message;
                return false;
            }
        }

        public List<NhanVienViewModel> ListPaging(int page, int pageSize, string filter,
          string DonViId, string PhongBanId, string Connectstr)
        {

            List<NhanVienViewModel> lstData = new List<NhanVienViewModel>();
            try
            {

                using (SqlConnection db = new SqlConnection(Connectstr))
                {
                    string query =
                        "select * from ( " +
                        "select ROW_NUMBER() OVER (ORDER BY nv.[Id]) AS RowNum " +
                        ",* " +
                        ",TenPB=isnull((select TenPhongBan from tblPhongBan where Id=PhongBanId),'') " +
                        ",ExpirationDate=(select top(1) LockoutEndDateUtc from AspNetUsers where Id=nv.Id) " +
                        ",TrangThai=(case when (select top(1) isnull(LockoutEnabled,0) from AspNetUsers where Id=nv.Id)=1 then N'Khóa' else N'Mở Khóa' end ) " +
                        "from tblNhanVien nv " +
                        "where " +
                        "((nv.TenNhanVien like REPLACE(N'%'+(N'" + filter + "')+N'%',' ',N'%') or N'" + filter + "'=N'') " +
                        "or (nv.Username like REPLACE(N'%'+(N'" + filter + "')+N'%',' ',N'%') or N'" + filter + "'=N'') " +
                        "or (nv.SoDT like REPLACE(N'%'+(N'" + filter + "')+N'%',' ',N'%') or N'" + filter + "'=N'')) "
                        ;


                    if (!string.IsNullOrEmpty(DonViId))
                    {
                        query = query + "and ((nv.DonViId=@DonViId) ";
                        query = query + "or (nv.DonViId in (select Id from tblDonVi dv where dv.DviCha=@DonViId) )) ";
                    }

                    if (!string.IsNullOrEmpty(PhongBanId))
                    {
                        query = query + "and nv.PhongBanId = @PhongBanId ";
                    }

                    query = query +
                        ") as kq " +
                        "where RowNum BETWEEN ((@page-1)*@pageSize)+1 and @page*@pageSize"
                        ;

                    using (var multipleresult = db.QueryMultiple(query,
                        new
                        {
                            page = page,
                            pageSize = pageSize,
                            PhongBanId = PhongBanId,
                            DonViId = DonViId,
                        }))
                    {
                        var q = multipleresult.Read<NhanVienViewModel>();
                        lstData = q.ToList();
                    }
                }
            }
            catch (Exception ex) { }
            return lstData;
        }

        public int CountListPaging(string filter,
            string DonViId, string PhongBanId, string Connectstr)
        {

            int count = 0;
            try
            {

                using (SqlConnection db = new SqlConnection(Connectstr))
                {
                    string query = "select count(Id) " +
                        "from tblNhanVien nv " +
                        "where " +
                        "((nv.TenNhanVien like REPLACE(N'%'+(N'" + filter + "')+N'%',' ',N'%') or N'" + filter + "'=N'') " +
                        "or (nv.Username like REPLACE(N'%'+(N'" + filter + "')+N'%',' ',N'%') or N'" + filter + "'=N'') " +
                        "or (nv.SoDT like REPLACE(N'%'+(N'" + filter + "')+N'%',' ',N'%') or N'" + filter + "'=N'')) "
                        ;

                    if (!string.IsNullOrEmpty(DonViId))
                    {
                        query = query + "and ((nv.DonViId=@DonViId) ";
                        query = query + "or (nv.DonViId in (select Id from tblDonVi dv where dv.DviCha=@DonViId) )) ";
                    }

                    if (!string.IsNullOrEmpty(PhongBanId))
                    {
                        query = query + "and nv.PhongBanId = @PhongBanId ";
                    }


                    using (var multipleresult = db.QueryMultiple(query,
                        new
                        {
                            PhongBanId = PhongBanId,
                            DonViId = DonViId
                        }))
                    {
                        try
                        {
                            var q = multipleresult.Read<int>();
                            count = q.FirstOrDefault();
                        }
                        catch (Exception ex)
                        {
                            count = 0;
                        }


                    }
                }
            }
            catch (Exception ex) { }
            return count;
        }

        public List<NhanVienViewModel> Export(string filter, string DonViId, int? PhongBanId)
        {
            List<NhanVienViewModel> lstData = new List<NhanVienViewModel>();
            try
            {

                using (SqlConnection db = new SqlConnection(Context.Database.Connection.ConnectionString))
                {
                    string query = "select * " +
                        ",TenPB=(select TenPhongBan from tblPhongBan where Id=nv.PhongBanId) " +
                        ",TenDonVi=(select dv.TenDonVi from tblDonVi dv join tblPhongBan pb on pb.MaDVi=dv.Id where pb.Id=nv.PhongBanId) " +
                        ", NgayDangNhapCuoi=(select top(1) h.ThoiGianTao from AspNetUserHistory h where h.TaiKhoan=nv.Username order by h.Id desc) " +
                        ",ExpirationDate=(select top(1) LockoutEndDateUtc from AspNetUsers where Id=nv.Id) " +
                        "from tblNhanVien nv " +
                        "where " +
                        "((nv.TenNhanVien like REPLACE(N'%'+(N'" + filter + "')+N'%',' ',N'%') or N'" + filter + "'=N'') " +
                        "or (nv.Username like REPLACE(N'%'+(N'" + filter + "')+N'%',' ',N'%') or N'" + filter + "'=N'') " +
                        "or (nv.SoDT like REPLACE(N'%'+(N'" + filter + "')+N'%',' ',N'%') or N'" + filter + "'=N'')) "
                        ;

                    if (!string.IsNullOrEmpty(DonViId))
                    {
                        query = query + "and ((nv.DonViId=@DonViId) ";
                        query = query + "or (nv.DonViId in (select Id from tblDonVi dv where dv.DviCha=@DonViId) )) ";
                    }

                    if ((PhongBanId.HasValue) && (PhongBanId.Value > 0))
                    {
                        query = query + "and nv.PhongBanId = @PhongBanId ";
                    }


                    using (var multipleresult = db.QueryMultiple(query,
                        new
                        {
                            PhongBanId = PhongBanId,
                            DonViId = DonViId
                        }))
                    {
                        try
                        {
                            var q = multipleresult.Read<NhanVienViewModel>();
                            lstData = q.ToList();
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
            }
            catch (Exception ex) { }
            return lstData;
        }

    }

    public class NhanVienViewModel
    {
        public string Id { get; set; }
        public string TenNhanVien { get; set; }
        public string ChucVu { get; set; }
        public string UrlImage { get; set; }
        public string BacHT { get; set; }
        public string BacThi { get; set; }
        public string BacAnToan { get; set; }
        public string NgaySinh { get; set; }
        public string DiaChi { get; set; }
        public string SoDT { get; set; }
        public string Email { get; set; }
        public bool IsCapPhieu { get; set; }
        public bool IsLanhDaoCv { get; set; }
        public bool IschiHuyTT { get; set; }
        public bool IsNguoiChoPhep { get; set; }
        public bool IsGiamSatAT { get; set; }
        public bool IsNguoiRaLenh { get; set; }
        public bool IsThiHanhLenh { get; set; }
        public string DonViId { get; set; }
        public string TenDonVi { get; set; }
        public int PhongBanId { get; set; }
        public string RoleId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public byte[] ChuKySo { get; set; }
        public string TenPB { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string TrangThai { get; set; }
        public DateTime? NgayDangNhapCuoi { get; set; }
    }

}

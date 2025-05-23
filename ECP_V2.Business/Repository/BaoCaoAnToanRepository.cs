﻿using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using System.Collections.Generic;
using System.Runtime.Remoting.Contexts;
using System.Xml.Linq;
using System.Runtime.InteropServices;
using System.Linq;
using ECP_V2.Business.Repository;
using ECP_V2.DataAccess;
using System.Security.Cryptography;

public class BaoCaoAnToanRepository
{
    private readonly string _connectionString;
    // Constructor nhận IConfiguration để lấy chuỗi kết nối từ cấu hình
    public BaoCaoAnToanRepository()
    {
        _connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["IdentityDbContext"].ConnectionString;

    }
    public class ModelBaoCaoAnToan
    {
        public string Id { get; set; }
        public int LoaiBaoCao { get; set; }
        public int TuanThang { get; set; }
        public int Nam { get; set; }
        public string NgayBatDau { get; set; }
        public string NgayKetThuc { get; set; }
        public int TrangThai { get; set; }
        public string TenTrangThai { get; set; }
        public string IdNguoiTrinhKy { get; set; }
        public string HoTenNguoiTrinh { get; set; }
        public string IdNguoiKy { get; set; }
        public string HoTenNguoiKy { get; set; }
        public string ChucVu { get; set; }
        public string NgayCapNhat { get; set; }
        public int SoBienBan { get; set; }
        public string IdDonVi { get; set; }
        public string URL_FileBienBan { get; set; }
        public string URL_FileDinhKem { get; set; }
        public string TenDonVi { get; set; }
        public string DiaDiem { get; set; }
        public string ThanhPhanThamGia { get; set; }
        public string NoiDungDanhGia { get; set; }
        public string KiemDiemAnToan { get; set; }
        public string TrachNhiemBoPhan { get; set; }
        public string ChiDaoKhacPhuc { get; set; }
        public string ViPhamKhac { get; set; }
        public string PhanTichDanhGia { get; set; }
        public string LuuYAnToan { get; set; }
        public string ChiDaoLienQuan { get; set; }
        public string ChiDaoAnToan { get; set; }
        public int? SoLuongNguoiViPham { get; set; }
        public int? SoLuongGiamThuong { get; set; }
        public int? SoLuongCatThuong { get; set; }
        public int? SoLuongKyLuat { get; set; }
    }

    public class ModelFilePath
    {
        public int Id { get; set; }
        public int IdLoaiFile { get; set; }
        public int IdTaiLieu { get; set; }
        public string TenFile { get; set; }
        public string MimeType { get; set; }
        public int Size { get; set; }
        public string URL { get; set; }
        public int TrangThai { get; set; }
        public string IdNguoiCapNhat { get; set; }
        public string NgayCapNhat { get; set; }
        //public string URL_FileBienBan { get; set; }
        //public string URL_FileDinhKem { get; set; }
    }
    // Tạo một kết nối SQL
    private IDbConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }

    public async Task<IEnumerable<ModelBaoCaoAnToan>> GetAll()
    {
        using (var connection = CreateConnection())
        {
            string query = "SELECT * FROM BienBanAnToan";
            // Sử dụng QueryAsync thay vì Query để thực hiện truy vấn bất đồng bộ
            return await connection.QueryAsync<ModelBaoCaoAnToan>(query);
        }
    }
    public async Task<int> Insert_BienBanAnToan(ModelBaoCaoAnToan model)
    {
        using (var connection = CreateConnection())
        {
            var parameters = new
            {
                model.LoaiBaoCao,
                model.TuanThang,
                model.Nam,
                model.NgayBatDau,
                model.NgayKetThuc,
                model.TrangThai,
                model.IdNguoiTrinhKy,
                model.HoTenNguoiTrinh,
                model.HoTenNguoiKy,
                model.IdNguoiKy,
                model.IdDonVi,
                model.SoBienBan,
                model.TenDonVi,
                model.DiaDiem,
                model.ThanhPhanThamGia,
                model.NoiDungDanhGia,
                model.KiemDiemAnToan,
                model.TrachNhiemBoPhan,
                model.ChiDaoKhacPhuc,
                model.ViPhamKhac,
                model.PhanTichDanhGia,
                model.LuuYAnToan,
                model.ChiDaoLienQuan,
                model.ChiDaoAnToan,
                model.SoLuongNguoiViPham,
                model.SoLuongGiamThuong,
                model.SoLuongCatThuong,
                model.SoLuongKyLuat,
                model.ChucVu
            };
            var id = await connection.QuerySingleAsync<int>("Insert_BienBanAnToan", parameters, commandType: CommandType.StoredProcedure);
            return id;
        }
    }
    public async Task<int> Update_LuuNhap_BienBanAnToan(ModelBaoCaoAnToan model)
    {
        using (var connection = CreateConnection())
        {
            var parameters = new
            {
                model.Id,
                model.LoaiBaoCao,
                model.TuanThang,
                model.Nam,
                model.NgayBatDau,
                model.NgayKetThuc,
                model.TrangThai,
                model.IdNguoiTrinhKy,
                model.HoTenNguoiTrinh,
                model.HoTenNguoiKy,
                model.IdNguoiKy,
                model.IdDonVi,
                model.SoBienBan,
                model.TenDonVi,
                model.DiaDiem,
                model.ThanhPhanThamGia,
                model.NoiDungDanhGia,
                model.KiemDiemAnToan,
                model.TrachNhiemBoPhan,
                model.ChiDaoKhacPhuc,
                model.ViPhamKhac,
                model.PhanTichDanhGia,
                model.LuuYAnToan,
                model.ChiDaoLienQuan,
                model.ChiDaoAnToan,
                model.SoLuongNguoiViPham,
                model.SoLuongGiamThuong,
                model.SoLuongCatThuong,
                model.SoLuongKyLuat,
                model.ChucVu
            };
            var rowsAffected = await connection.ExecuteAsync("Update_BienBanAnToan", parameters, commandType: CommandType.StoredProcedure );
            return rowsAffected;
        }
    }
    public async Task<int> Delete_LuuNhap_BienBanAnToan(int id)
    {
        using (var connection = CreateConnection())
        {
            string query = "DELETE FROM BienBanAnToan WHERE Id = @Id;";
            var rowsAffected = await connection.ExecuteAsync(query, new { Id = id });
            return rowsAffected; 
        }
    }

    public async Task<int> Insert_FilePath(ModelFilePath model)
    {
        using (var connection = CreateConnection())
        {
            string query = @"
        INSERT INTO FilePath (IdLoaiFile, IdTaiLieu, TenFile, MimeType, Size, URL, TrangThai, IdNguoiCapNhat, NgayCapNhat)
        OUTPUT INSERTED.Id
        VALUES (@IdLoaiFile, @IdTaiLieu, @TenFile, @MimeType, @Size, @URL, @TrangThai, @IdNguoiCapNhat, GetDate())";

            var id = await connection.QuerySingleAsync<int>(query, model);

            return id;
        }
    }
    public async Task<IEnumerable<ModelBaoCaoAnToan>> Get_BienBan_ByTime(int LoaiBaoCao, int TuanThang, int Nam, string IdDonVi)
    {
        using (var connection = CreateConnection())
        {
            string query = @"SELECT a.*, b.URL as URL_FileBienBan, c.URL as URL_FileDinhKem, d.TenTrangThai
                FROM BienBanAnToan a
                left join filepath b on a.Id= b.IdTaiLieu and b.IdLoaiFile= 1 and b.TrangThai=1 
                left join filepath c on a.Id= c.IdTaiLieu and c.IdLoaiFile= 2 and c.TrangThai=1 
                inner join DM_TrangThai_BienBanAnToan d on a.TrangThai= d.Id 
                WHERE LoaiBaoCao = @LoaiBaoCao AND (TuanThang = @TuanThang or @TuanThang = -1) AND Nam = @Nam AND IdDonVi = @IdDonVi";
            var parameters = new { LoaiBaoCao, TuanThang, Nam, IdDonVi };
            var data = await connection.QueryAsync<ModelBaoCaoAnToan>(query, parameters);

            return data;
        }
    }

    public async Task<int> HuyTrinh_TraLai_BienBanAnToan(int Id, int TrangThai)
    {
        using (var connection = CreateConnection())
        {
            string query = @"UPDATE BienBanAnToan 
                         SET TrangThai = @TrangThai, 
                         NgayCapNhat = GETDATE() 
                         WHERE Id = @Id";

            var rowsAffected = await connection.ExecuteAsync(query, new
            {
                Id,
                TrangThai
            });

            return rowsAffected;
        }
    }
    public async Task<int> Update_TrangThaiFilePathByBienBan(int Id, int TrangThai, string IdNguoiCapNhat)
    {
        using (var connection = CreateConnection())
        {
            string query = @"update FilePath set TrangThai= @TrangThai, NgayCapNhat= getdate(), IdNguoiCapNhat= @IdNguoiCapNhat
                            where IdTaiLieu= @Id and IdLoaiFile in(1,2)";

            var rowsAffected = await connection.ExecuteAsync(query, new
            {
                Id,
                TrangThai,
                IdNguoiCapNhat
            });

            return rowsAffected;
        }
    }

    public async Task<int> TrinhKyLai_BienBanAnToan(ModelBaoCaoAnToan model)
    {
        using (var connection = CreateConnection())
        {
            string query = @"
            UPDATE BienBanAnToan
            SET 
                TrangThai = 1,        
                NgayCapNhat = GETDATE(),  
                IdNguoiKy = @IdNguoiKy,   
                HoTenNguoiKy = @HoTenNguoiKy 
            WHERE Id = @Id";

            var rowsAffected = await connection.ExecuteAsync(query, new
            {
                Id = model.Id,
                IdNguoiKy = model.IdNguoiKy,
                HoTenNguoiKy = model.HoTenNguoiKy
            });

            return rowsAffected;
        }
    }
    public int Get_SoBienBanMax_ByDonVi(string DonViID)
    {
        try
        {
            using (var db = new ECP_V2Entities())
            {
                var result = db.Database.SqlQuery<int?>(
                    @"SELECT ISNULL(MAX(SoBienBan), 0) AS MaxSoBienBan
                    FROM [BienBanAnToan]
                    WHERE IdDonVi = @DonViID;",
                    new SqlParameter("@DonViID", DonViID)
                ).FirstOrDefault();
                return result ?? 0;
            }
        }
        catch (Exception ex)
        {
            return -1;
        }
    }
    public async Task<ModelBaoCaoAnToan> Get_BienBan_ById(int Id)
    {
        using (var connection = CreateConnection())
        {
            string query = @"SELECT a.* FROM BienBanAnToan a WHERE Id = @Id";
            var parameters = new { Id };

            var data = await connection.QueryAsync<ModelBaoCaoAnToan>(query, parameters);

            return data.FirstOrDefault();
        }
    }

    public int Update_FilePath_ByURL(string URL, int TrangThai)
    {
        try
        {
            using (var db = new ECP_V2Entities())
            {
                var rowsAffected = db.Database.ExecuteSqlCommand(
                    @"UPDATE FilePath SET TrangThai = @TrangThai WHERE URL = @URL",
                    new SqlParameter("@URL", URL), 
                    new SqlParameter("@TrangThai", TrangThai)
                );

                return rowsAffected;  
            }
        }
        catch (Exception ex)
        {
            return -1;  
        }
    }

}

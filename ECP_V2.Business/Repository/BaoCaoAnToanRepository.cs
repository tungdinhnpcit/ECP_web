using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using System.Collections.Generic;
using System.Runtime.Remoting.Contexts;
using System.Xml.Linq;
using System.Runtime.InteropServices;

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
        public string IdNguoiTrinhKy { get; set; }
        public string HoTenNguoiTrinh { get; set; }
        public string IdNguoiKy { get; set; }
        public string HoTenNguoiKy { get; set; }
        public string NgayCapNhat { get; set; }
        public int oBienBan { get; set; }
        public string IdDonVi { get; set; }
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
            string query = @"
            INSERT INTO BienBanAnToan (LoaiBaoCao, TuanThang, Nam, NgayBatDau, NgayKetThuc, TrangThai, IdNguoiTrinhKy, HoTenNguoiTrinh, IdDonVi, NgayCapNhat)
            OUTPUT INSERTED.ID
            VALUES (@LoaiBaoCao, @TuanThang, @Nam, @NgayBatDau, @NgayKetThuc, @TrangThai, @IdNguoiTrinhKy, @HoTenNguoiTrinh, @IdDonVi, GetDate())";
            var id = await connection.QuerySingleAsync<int>(query, model);

            return id;  // Trả về ID của bản ghi vừa được thêm
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
    public async Task<IEnumerable<dynamic>> Get_BienBan_ByTime(int LoaiBaoCao, int TuanThang, int Nam, string IdDonVi)
    {
        using (var connection = CreateConnection())
        {
            string query = @"SELECT a.*, b.URL
                FROM BienBanAnToan a
                left join filepath b on a.Id= b.IdTaiLieu
                WHERE LoaiBaoCao = @LoaiBaoCao AND TuanThang = @TuanThang AND Nam = @Nam AND IdDonVi = @IdDonVi";

            var parameters = new { LoaiBaoCao, TuanThang, Nam, IdDonVi };
            var data = await connection.QueryAsync<dynamic>(query, parameters);
            return data;
        }
    }

}

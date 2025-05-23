//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ECP_V2.DataAccess
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblPhienLamViec
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblPhienLamViec()
        {
            this.tblComments = new HashSet<tblComment>();
            this.tblPhienLamViec_TheoDoi = new HashSet<tblPhienLamViec_TheoDoi>();
            this.tblPhienLamViec_ThuocTinh = new HashSet<tblPhienLamViec_ThuocTinh>();
        }
    
        public int Id { get; set; }
        public int PhongBanID { get; set; }
        public string NoiDung { get; set; }
        public string DiaDiem { get; set; }
        public System.DateTime NgayLamViec { get; set; }
        public System.DateTime GioBd { get; set; }
        public System.DateTime GioKt { get; set; }
        public string NguoiDuyet_SoPa { get; set; }
        public string NguoiChiHuy { get; set; }
        public string GiamSatVien { get; set; }
        public string NguoiKiemSoat { get; set; }
        public string NguoiKiemTraPhieu { get; set; }
        public string LanhDaoTrucBan { get; set; }
        public Nullable<System.DateTime> NgayTao { get; set; }
        public string NguoiTao { get; set; }
        public Nullable<System.DateTime> NgaySua { get; set; }
        public string NguoiSua { get; set; }
        public Nullable<int> TT_Phien { get; set; }
        public Nullable<int> TrangThai { get; set; }
        public string NguoiDuyet { get; set; }
        public Nullable<System.DateTime> NgayDuyet { get; set; }
        public string LyDoThayDoi { get; set; }
        public Nullable<int> MaPCT { get; set; }
        public string NguoiDuyet_SoPa_Id { get; set; }
        public string NguoiChiHuy_Id { get; set; }
        public string GiamSatVien_Id { get; set; }
        public string NguoiKiemSoat_Id { get; set; }
        public string NguoiKiemTraPhieu_Id { get; set; }
        public string LanhDaoTrucBan_Id { get; set; }
        public string DonViId { get; set; }
        public Nullable<bool> IsChuyenNPC { get; set; }
        public Nullable<System.DateTime> NgayDuyetNPC { get; set; }
        public string NguoiDuyetNPC { get; set; }
        public Nullable<System.DateTime> NgayKetThuc { get; set; }
        public Nullable<bool> IsKiemTra { get; set; }
        public Nullable<System.DateTime> NgayGioKT { get; set; }
        public Nullable<decimal> KinhDo { get; set; }
        public Nullable<decimal> ViDo { get; set; }
        public string NguoiKetThuc { get; set; }
        public Nullable<bool> IsEndByWeb { get; set; }
        public string LanhDaoCongViec { get; set; }
        public string LanhDaoCongViec_Id { get; set; }
        public string NguoiCapPhieu { get; set; }
        public string NguoiCapPhieu_Id { get; set; }
        public Nullable<int> PhongBanIDCreate { get; set; }
        public string DieuKienAnToan { get; set; }
        public string ID_KTGS { get; set; }
        public string GhiChu { get; set; }
    
        public virtual plv_TinhChatPhien plv_TinhChatPhien { get; set; }
        public virtual plv_TrangThaiPhien plv_TrangThaiPhien { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblComment> tblComments { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblPhienLamViec_TheoDoi> tblPhienLamViec_TheoDoi { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblPhienLamViec_ThuocTinh> tblPhienLamViec_ThuocTinh { get; set; }

    }
}

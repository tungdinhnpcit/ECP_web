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
    
    public partial class pccc_ThietBiPCCC
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public pccc_ThietBiPCCC()
        {
            this.pccc_SoTheoDoiPCCC = new HashSet<pccc_SoTheoDoiPCCC>();
        }
    
        public int ID { get; set; }
        public string TenThietBi { get; set; }
        public string MaHieu { get; set; }
        public Nullable<int> MaHSX { get; set; }
        public Nullable<int> MaNSX { get; set; }
        public Nullable<int> NamSX { get; set; }
        public Nullable<System.DateTime> NgayDuaVaoSuDung { get; set; }
        public Nullable<int> PhongBanID { get; set; }
        public string DonViId { get; set; }
        public Nullable<System.DateTime> NgayTao { get; set; }
        public string NguoiTao { get; set; }
        public Nullable<System.DateTime> NgaySua { get; set; }
        public string NguoiSua { get; set; }
        public Nullable<int> MaNhom { get; set; }
        public Nullable<int> HanKiemDinh { get; set; }
        public string SoCheTao { get; set; }
        public string SoDangKy { get; set; }
        public Nullable<decimal> TaiTrong { get; set; }
        public string HoSoThietBi { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public Nullable<int> MaLoai { get; set; }
        public string QuyTacDanhMa { get; set; }
        public Nullable<int> MaTT { get; set; }
    
        public virtual HangSanXuat HangSanXuat { get; set; }
        public virtual NuocSanXuat NuocSanXuat { get; set; }
        public virtual pccc_LoaiThietBi pccc_LoaiThietBi { get; set; }
        public virtual pccc_NhomThietBi pccc_NhomThietBi { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<pccc_SoTheoDoiPCCC> pccc_SoTheoDoiPCCC { get; set; }
        public virtual pccc_TrangThai pccc_TrangThai { get; set; }
        public virtual tblPhongBan tblPhongBan { get; set; }
    }
}

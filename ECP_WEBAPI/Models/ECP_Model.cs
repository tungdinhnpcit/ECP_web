namespace ECP_WEBAPI.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class ECP_Model : DbContext
    {
        public ECP_Model()
            : base("name=ECP_Model")
        {
        }

        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<Log> Logs { get; set; }
        public virtual DbSet<tblBaoCao> tblBaoCaos { get; set; }
        public virtual DbSet<tblBienBanKiemTra> tblBienBanKiemTras { get; set; }
        public virtual DbSet<tblComment> tblComments { get; set; }
        public virtual DbSet<tblDonVi> tblDonVis { get; set; }
        public virtual DbSet<tblGroupImage> tblGroupImages { get; set; }
        public virtual DbSet<tblImage> tblImages { get; set; }
        public virtual DbSet<tblNhanVien> tblNhanViens { get; set; }
        public virtual DbSet<tblPhienLamViec> tblPhienLamViecs { get; set; }
        public virtual DbSet<tblPhongBan> tblPhongBans { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AspNetRole>()
                .HasMany(e => e.AspNetUsers)
                .WithMany(e => e.AspNetRoles)
                .Map(m => m.ToTable("AspNetUserRoles").MapLeftKey("RoleId").MapRightKey("UserId"));

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.AspNetUserClaims)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.AspNetUserLogins)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<tblBaoCao>()
                .HasMany(e => e.tblBienBanKiemTras)
                .WithOptional(e => e.tblBaoCao)
                .HasForeignKey(e => e.BaoCaoId)
                .WillCascadeOnDelete();

            modelBuilder.Entity<tblDonVi>()
                .HasMany(e => e.tblPhongBans)
                .WithOptional(e => e.tblDonVi)
                .HasForeignKey(e => e.MaDVi);

            modelBuilder.Entity<tblGroupImage>()
                .HasMany(e => e.tblImages)
                .WithOptional(e => e.tblGroupImage)
                .HasForeignKey(e => e.GroupId);

            modelBuilder.Entity<tblPhienLamViec>()
                .HasMany(e => e.tblComments)
                .WithOptional(e => e.tblPhienLamViec)
                .HasForeignKey(e => e.PhienLamViecId)
                .WillCascadeOnDelete();

            modelBuilder.Entity<tblPhienLamViec>()
                .HasMany(e => e.tblImages)
                .WithOptional(e => e.tblPhienLamViec)
                .HasForeignKey(e => e.PhienLamViecId);

            modelBuilder.Entity<tblPhongBan>()
                .HasMany(e => e.tblNhanViens)
                .WithOptional(e => e.tblPhongBan)
                .HasForeignKey(e => e.PhongBanId);
        }
    }
}

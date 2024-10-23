using System;

namespace ECP_V2.WebApplication.Models
{
    public class ImageModel
    {
        public enum ImgSort
        {
            THOIGIAN = 0,
            PHIENLV = 1,
            NHANVIEN = 2,
            DACOPHIEN = 3,
            CHUACOPHIEN = 4
        }
    }

    public class ListImage
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Note { get; set; }
        public string Comment { get; set; }
        public string Tag { get; set; }
        public System.DateTime NgayCapNhat { get; set; }
        public Nullable<int> GroupId { get; set; }
        public Nullable<int> PhienLamViecId { get; set; }
        public string UserUp { get; set; }
        public Nullable<int> isVideo { get; set; }
        public string VideoPath { get; set; }
    }
}
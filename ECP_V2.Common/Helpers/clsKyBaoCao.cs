using System;
using System.Collections.Generic;
using System.Text;

namespace ECP_V2.Common.Helpers
{
    public class clsKyBaoCao
    {
        public List<KyBaoCao> Source = new List<KyBaoCao>()
        {
            new KyBaoCao() {ID=0,Ten= "Hôm nay"},
            new KyBaoCao() {ID=1,Ten= "Tuần này"},
            new KyBaoCao() {ID=2,Ten= "Đầu tuần tới hiện tại"},
            new KyBaoCao() {ID=3,Ten= "Tháng này"},
            new KyBaoCao() {ID=4,Ten= "Đầu tháng đến hiện tại"},
            new KyBaoCao() {ID=5,Ten= "Quý này"},
            new KyBaoCao() {ID=6,Ten= "Đầu quý đến hiện tại"},
            new KyBaoCao() {ID=7,Ten= "Năm này"},
            new KyBaoCao() {ID=8,Ten= "Đầu năm đến hiện tại"},
            new KyBaoCao() {ID=9,Ten= "Tháng 1"},
            new KyBaoCao() {ID=10,Ten= "Tháng 2"},
            new KyBaoCao() {ID=11,Ten= "Tháng 3"},
            new KyBaoCao() {ID=12,Ten= "Tháng 4"},
            new KyBaoCao() {ID=13,Ten= "Tháng 5"},
            new KyBaoCao() {ID=14,Ten= "Tháng 6"},
            new KyBaoCao() {ID=15,Ten= "Tháng 7"},
            new KyBaoCao() {ID=16,Ten= "Tháng 8"},
            new KyBaoCao() {ID=17,Ten= "Tháng 9"},
            new KyBaoCao() {ID=18,Ten= "Tháng 10"},
            new KyBaoCao() {ID=19,Ten= "Tháng 11"},
            new KyBaoCao() {ID=20,Ten= "Tháng 12"},
            new KyBaoCao() {ID=21,Ten= "Quý I"},
            new KyBaoCao() {ID=22,Ten= "Quý II"},
            new KyBaoCao() {ID=23,Ten= "Quý III"},
            new KyBaoCao() {ID=24,Ten= "Quý IV"},
            new KyBaoCao() {ID=25,Ten= "Tuần trước"},
            new KyBaoCao() {ID=26,Ten= "Tháng trước"},
            new KyBaoCao() {ID=27,Ten= "Quý trước"},
            new KyBaoCao() {ID=28,Ten= "Năm trước"},
            new KyBaoCao() {ID=29,Ten= "Tuần sau"},
            new KyBaoCao() {ID=30,Ten= "Bốn tuần sau"},
            new KyBaoCao() {ID=31,Ten= "Tháng sau" },
            new KyBaoCao() {ID=32,Ten= "Quý sau" },
            new KyBaoCao() {ID=33,Ten= "Năm sau" },
            new KyBaoCao() {ID=34,Ten= "Tự chọn" }
        };

        public List<KyBaoCao> SourceWeek = new List<KyBaoCao>()
        {
            new KyBaoCao() {ID=0,Ten= "Tuần này"},
            new KyBaoCao() {ID=1,Ten= "Đầu tuần tới hiện tại"},
            new KyBaoCao() {ID=2,Ten= "Tuần trước"},
            new KyBaoCao() {ID=3,Ten= "Tuần sau"},
            new KyBaoCao() {ID=4,Ten= "Bốn tuần sau"} 
        };


        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public int Index { get; set; }

        public int FirstMonth(int month)
        {
            if (month <= 3)
                return 1;
            else if (month <= 6)
                return 4;
            else if (month <= 9)
                return 7;
            else
                return 10;
        }

        public void SetToDate()
        {
            DateTime dateNow = DateTime.Now;
            switch (this.Index)
            {
                case 0: //Ngay nay
                    DateFrom = dateNow;
                    DateTo = dateNow;
                    break;
                case 1: //Tuan nay
                        //DateFrom = dateNow.AddDays(1 - (int)dateNow.DayOfWeek);
                        //DateTo = dateNow.AddDays(7 - (int)dateNow.DayOfWeek);
                    DateFrom = dateNow.AddDays(1 - (int)dateNow.DayOfWeek);
                    DateTo = dateNow.AddDays(7 - (int)dateNow.DayOfWeek);
                    break;
                case 2: //Dau tuan den hien tai
                    //DateFrom = dateNow.AddDays(1 - (int)dateNow.DayOfWeek);
                    DateFrom = dateNow.AddDays( - (int)dateNow.DayOfWeek-5);
                    DateTo = dateNow;
                    break;
                case 3: //Thang nay
                    DateFrom = new DateTime(dateNow.Year, dateNow.Month, 1);
                    DateTo = new DateTime(dateNow.Year, dateNow.Month, 1).AddMonths(1).AddDays(-1);
                    break;
                case 4: //Dau thang den hien tai
                    DateFrom = new DateTime(dateNow.Year, dateNow.Month, 1);
                    DateTo = dateNow;
                    break;
                case 5: //Quy nay
                    DateFrom = new DateTime(dateNow.Year, FirstMonth(dateNow.Month), 1);
                    DateTo = new DateTime(dateNow.Year,
                        FirstMonth(dateNow.Month) + 2, 1).AddMonths(1).AddDays(-1);
                    break;
                case 6: //Dau quy den hien tai
                    DateFrom = new DateTime(dateNow.Year, FirstMonth(dateNow.Month), 1);
                    DateTo = dateNow;
                    break;
                case 7: //Nam nay
                    DateFrom = new DateTime(dateNow.Year, 1, 1);
                    DateTo = new DateTime(dateNow.Year, 12, 31);
                    break;
                case 8: //Dau nam den hien tai
                    DateFrom = new DateTime(dateNow.Year, 1, 1);
                    DateTo = dateNow;
                    break;
                case 9: //Thang 1
                    DateFrom = new DateTime(dateNow.Year, 1, 1);
                    DateTo = new DateTime(dateNow.Year, 2, 1).AddDays(-1);
                    break;
                case 10: //Thang 2
                    DateFrom = new DateTime(dateNow.Year, 2, 1);
                    DateTo = new DateTime(dateNow.Year, 3, 1).AddDays(-1);
                    break;
                case 11: //Thang 3
                    DateFrom = new DateTime(dateNow.Year, 3, 1);
                    DateTo = new DateTime(dateNow.Year, 4, 1).AddDays(-1);
                    break;
                case 12: //Thang 4
                    DateFrom = new DateTime(dateNow.Year, 4, 1);
                    DateTo = new DateTime(dateNow.Year, 5, 1).AddDays(-1);
                    break;
                case 13: //Thang 5
                    DateFrom = new DateTime(dateNow.Year, 5, 1);
                    DateTo = new DateTime(dateNow.Year, 6, 1).AddDays(-1);
                    break;
                case 14: //Thang 6
                    DateFrom = new DateTime(dateNow.Year, 6, 1);
                    DateTo = new DateTime(dateNow.Year, 7, 1).AddDays(-1);
                    break;
                case 15: //Thang 7
                    DateFrom = new DateTime(dateNow.Year, 7, 1);
                    DateTo = new DateTime(dateNow.Year, 8, 1).AddDays(-1);
                    break;
                case 16: //Thang 8
                    DateFrom = new DateTime(dateNow.Year, 8, 1);
                    DateTo = new DateTime(dateNow.Year, 9, 1).AddDays(-1);
                    break;
                case 17: //Thang 9
                    DateFrom = new DateTime(dateNow.Year, 9, 1);
                    DateTo = new DateTime(dateNow.Year, 10, 1).AddDays(-1);
                    break;
                case 18: //Thang 10
                    DateFrom = new DateTime(dateNow.Year, 10, 1);
                    DateTo = new DateTime(dateNow.Year, 11, 1).AddDays(-1);
                    break;
                case 19: //Thang 11
                    DateFrom = new DateTime(dateNow.Year, 11, 1);
                    DateTo = new DateTime(dateNow.Year, 12, 1).AddDays(-1);
                    break;
                case 20: //Thang 12
                    DateFrom = new DateTime(dateNow.Year, 12, 1);
                    DateTo = new DateTime(dateNow.Year, 12, DateTime.DaysInMonth(dateNow.Year, 12));
                    break;
                case 21: //Quy I
                    DateFrom = new DateTime(dateNow.Year, 1, 1);
                    DateTo = new DateTime(dateNow.Year, 4, 1).AddDays(-1);
                    break;
                case 22: //Quy II
                    DateFrom = new DateTime(dateNow.Year, 4, 1);
                    DateTo = new DateTime(dateNow.Year, 7, 1).AddDays(-1);
                    break;
                case 23: //Quy III
                    DateFrom = new DateTime(dateNow.Year, 7, 1);
                    DateTo = new DateTime(dateNow.Year, 10, 1).AddDays(-1);
                    break;
                case 24: //Quy IV
                    DateFrom = new DateTime(dateNow.Year, 10, 1);
                    DateTo = new DateTime(dateNow.Year, 12, 31);
                    break;
                case 25: //Tuan truoc
                    //DateFrom = dateNow.AddDays(-(int)dateNow.DayOfWeek - 6);
                    //DateTo = dateNow.AddDays(-(int)dateNow.DayOfWeek);
                    DateFrom = dateNow.AddDays(-(int)dateNow.DayOfWeek - 5);
                    DateTo = dateNow.AddDays(1-(int)dateNow.DayOfWeek);
                    break;
                case 26: //Thang truoc
                    DateFrom = new DateTime(dateNow.Year, dateNow.Month, 1).AddMonths(-1);
                    DateTo = new DateTime(dateNow.Year, dateNow.Month, 1).AddDays(-1);
                    break;
                case 27: //Quy truoc
                    DateFrom = new DateTime(dateNow.Year, FirstMonth(dateNow.Month), 1).AddMonths(-3);
                    DateTo = new DateTime(dateNow.Year, FirstMonth(dateNow.Month), 1).AddDays(-1);
                    break;
                case 28: //Nam truoc
                    DateFrom = new DateTime(dateNow.Year - 1, 1, 1);
                    DateTo = new DateTime(dateNow.Year - 1, 12, DateTime.DaysInMonth(dateNow.Year, 12));
                    break;
                case 29: //Tuan sau
                         //DateFrom = dateNow.AddDays(8 - (int)dateNow.DayOfWeek);
                         //DateTo = dateNow.AddDays(14 - (int)dateNow.DayOfWeek);
                    DateFrom = dateNow.AddDays(9 - (int)dateNow.DayOfWeek);
                    DateTo = dateNow.AddDays(15 - (int)dateNow.DayOfWeek);
                    break;
                case 30: //Bon tuan sau
                         //DateFrom = dateNow;
                         //DateTo = dateNow.AddMonths(4);
                    DateFrom = dateNow.AddDays(2 - (int)dateNow.DayOfWeek);
                    DateTo = dateNow.AddDays(29 - (int)dateNow.DayOfWeek);
                    break;
                case 31: //Thang sau
                    DateFrom = new DateTime(dateNow.Year, dateNow.Month, 1).AddMonths(1);
                    DateTo = new DateTime(dateNow.Year, dateNow.Month, 1).AddMonths(2).AddDays(-1);
                    break;
                case 32: //Quy sau
                    DateTime QuySau = new DateTime(dateNow.Year, FirstMonth(dateNow.Month) + 3, 1);
                    DateFrom = QuySau;
                    DateTo = QuySau.AddMonths(3).AddDays(-1);
                    break;
                case 33: //Nam sau
                    DateFrom = new DateTime(dateNow.Year + 1, 1, 1);
                    DateTo = new DateTime(dateNow.Year + 1, 12, 31);
                    break;
                case 34: //Tu chon
                    DateFrom = new DateTime(2010, 1, 1);
                    DateTo = dateNow;
                    break;
                default:
                    break;
            }
        }

        public void SetToDateWeek()
        {
            DateTime dateNow = DateTime.Now;
            switch (this.Index)
            {
                
                case 0: //Tuan nay
                        //DateFrom = dateNow.AddDays(1 - (int)dateNow.DayOfWeek);
                        //DateTo = dateNow.AddDays(7 - (int)dateNow.DayOfWeek);
                    DateFrom = dateNow.AddDays(2 - (int)dateNow.DayOfWeek);
                    DateTo = dateNow.AddDays(8 - (int)dateNow.DayOfWeek);
                    break;
                case 1: //Dau tuan den hien tai
                    //DateFrom = dateNow.AddDays(1 - (int)dateNow.DayOfWeek);
                    DateFrom = dateNow.AddDays(-(int)dateNow.DayOfWeek - 5);
                    DateTo = dateNow;
                    break;
                case 2: //Tuan truoc
                    //DateFrom = dateNow.AddDays(-(int)dateNow.DayOfWeek - 6);
                    //DateTo = dateNow.AddDays(-(int)dateNow.DayOfWeek);
                    DateFrom = dateNow.AddDays(-(int)dateNow.DayOfWeek - 5);
                    DateTo = dateNow.AddDays(1 - (int)dateNow.DayOfWeek);
                    break;
                case 3: //Tuan sau
                         //DateFrom = dateNow.AddDays(8 - (int)dateNow.DayOfWeek);
                         //DateTo = dateNow.AddDays(14 - (int)dateNow.DayOfWeek);
                    DateFrom = dateNow.AddDays(9 - (int)dateNow.DayOfWeek);
                    DateTo = dateNow.AddDays(15 - (int)dateNow.DayOfWeek);
                    break;
                case 4: //Bon tuan sau
                         //DateFrom = dateNow;
                         //DateTo = dateNow.AddMonths(4);
                    DateFrom = dateNow.AddDays(2 - (int)dateNow.DayOfWeek);
                    DateTo = dateNow.AddDays(29 - (int)dateNow.DayOfWeek);
                    break;
                default:
                    break;
            }
        }

    }
    public class KyBaoCao
    {
        public int ID { get; set; }
        public string Ten { get; set; }
    }
}

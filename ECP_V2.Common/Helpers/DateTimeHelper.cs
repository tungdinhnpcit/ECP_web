using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECP_V2.Common.Helpers
{
    public class DateTimeHelper
    {
        /// <summary>
        /// Lấy ra thứ tự trong năm của tuần có chứa ngày nhập vào 
        /// với Culture mặc định là Culture hiện tại
        /// </summary>
        /// <param name="time">Ngày nhập vào</param>
        /// <returns>Trả về kiểu int là thứ tự của tuần trong năm</returns>
        public static int GetWeekOrderInYear(DateTime time)
        {
            CultureInfo myCI = CultureInfo.CurrentCulture;
            Calendar myCal = myCI.Calendar;
            CalendarWeekRule myCWR = myCI.DateTimeFormat.CalendarWeekRule;
            DayOfWeek myFirstDOW = myCI.DateTimeFormat.FirstDayOfWeek;

            return myCal.GetWeekOfYear(time, myCWR, myFirstDOW);
        }
    }
}

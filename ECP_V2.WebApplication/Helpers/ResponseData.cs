using System;

namespace ECP_V2.WebApplication.Helpers
{
    public class ResponseData
    {
        public int State;
        public object Data;
        public String Mess;
        public static ResponseData ReturnSucess(object d)
        {
            return new ResponseData() { State = 1, Data = d, Mess = "" };

        }

        public static ResponseData ReturnFail(string mess)
        {
            return new ResponseData() { State = 0, Data = null, Mess = mess };

        }
    }
}
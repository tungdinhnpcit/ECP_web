using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace ECP_V2.WebApplication.Helpers
{
    public class UltilHelper
    {
        public List<DropDownItems> GetTypeOfRoles()
        {
            var priority = new List<DropDownItems>();
            priority.Add(new DropDownItems { Id = 1, Name = "Nhóm quyền hệ thống" });
            priority.Add(new DropDownItems { Id = 2, Name = "Nhóm quyền chức năng" });
            return priority;
        }
    }

    public class DropDownItems
    {
        public short Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
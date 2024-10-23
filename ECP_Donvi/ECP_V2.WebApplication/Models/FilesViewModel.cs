using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ECP_V2.WebApplication.Helpers;

namespace ECP_V2.WebApplication.Models
{
    public class FilesViewModel
    {
        public ViewDataUploadFilesResult[] Files { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobTransferMiddleware.model
{
    public class FileUploadModel
    {
        public Guid Id { get; set; }

        public string FileName { get; set; }

        public string PhysicalPath { get; set; }
    }
}

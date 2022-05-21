using System;
using System.Collections.Generic;
using System.Text;

namespace KLTN.Core.StudentServices.DTOs
{
    public class StudentCertificateDTO
    {
        public string CertificateName { get; set; }
        public long StartTime { get; set; }
        public long EndTime { get; set; }
    }
}

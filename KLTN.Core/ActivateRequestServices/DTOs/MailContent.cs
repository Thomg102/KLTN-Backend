using System;
using System.Collections.Generic;
using System.Text;

namespace KLTN.Core.ActivateRequestServices.DTOs
{
    public class MailContent
    {
        public string To { get; set; }             
        public string Subject { get; set; }    
        public string Body { get; set; }
    }
}

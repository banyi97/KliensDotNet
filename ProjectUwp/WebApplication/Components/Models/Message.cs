using SharedLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Components.Models
{
    public class Message : IAuditableEntity, IMessage
    {
        [Key]
        public string Id { get; set; }

        public string Data { get; set; }
        public string SenderId { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime LatestUpdate { get; set; }
    }
}

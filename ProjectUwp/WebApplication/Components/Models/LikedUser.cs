using SharedLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Components.Models
{
    public class LikedUser : IAuditableEntity
    {
        [Key]
        public string Id { get; set; }
        public string UserId { get; set; }
        public bool IsLiked { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime LatestUpdate { get; set; }
    }
}

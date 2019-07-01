using SharedLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Components.Models
{
    public class Match : IAuditableEntity
    {
        [Key]
        public string Id { get; set; }

        public IList<Message> Messages { get; set; } = new List<Message>();

        public bool IsActive { get; set; }
        public string UserId_1 { get; set; }
        public string UserId_2 { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime LatestUpdate { get; set; }

        public virtual IOrderedEnumerable<Message> GetOrderedMessages()
        {           
            var coll = Messages.OrderBy(q => q.CreatedDate);
            return coll;
        }

        public virtual Message GetLastMessage()
        {          
            var q = this.GetOrderedMessages();
            var last = q.LastOrDefault();
            return last;
        }
    }
}

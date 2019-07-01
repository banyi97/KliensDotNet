using SharedLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Components.Models
{
    public class Photo : IPhoto, IAuditableEntity
    {
        [Key]
        public string Id { get; set; }

        public bool Main { get; set; }
        public string FileName { get; set; }
        public string Extenstion { get; set; }
        [NotMapped]
        public string Url { get => "https://projectuwpstorage.blob.core.windows.net/images/" + this.FileName; set => throw new NotImplementedException(); }

        public DateTime CreatedDate { get; set; }
        public DateTime LatestUpdate { get; set; }
    }
}

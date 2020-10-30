using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities
{
    [Table("Photos")]
    public class Photo
    {
        public int id { get; set; }
        public int MyProperty { get; set; }
        public string Url { get; set; }
        public bool IsMain { get; set; }
        public string PublicId { get; set; }
        public AppUsers AppUser { get; set; }
        public int AppUserId { get; set; }
    }
}
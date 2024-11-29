// Models/Customer.cs
/*ควรต่อ DB ด้วย Models*/
using System.ComponentModel.DataAnnotations;

namespace DotnetWebApiWithEFCodeFirst.Models
{
    public class Customer
    {
        [Key]
        public int Customer_Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Customer_firstname { get; set; }
        [Required]
        [MaxLength(50)]
        public string Customer_lastName { get; set; }
        [Required]
        [MaxLength(15)]
        public string Phone_number { get; set; }
    }
}
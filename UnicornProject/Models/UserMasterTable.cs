using System;
using System.Collections.Generic;

namespace UnicornProject.Models
{
    public partial class UserMasterTable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Adress { get; set; }
        public string Password { get; set; }
        public int? RoleId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? Isactive { get; set; }

        public virtual Role Role { get; set; }
    }
}

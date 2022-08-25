using System;
using System.Collections.Generic;

namespace UnicornProject.Models
{
    public partial class Role
    {
        public Role()
        {
            UserMasterTable = new HashSet<UserMasterTable>();
        }

        public int Id { get; set; }
        public string RoleName { get; set; }
        public DateTime? CretaedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? Isactive { get; set; }

        public virtual ICollection<UserMasterTable> UserMasterTable { get; set; }
    }
}

using Dapper.Moon;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Moon.Test
{
    [Table("t_moon_user")]
    public class User
    {
        [Key]
        public string Id { get; set; }

        public string Account { get; set; }

        public string Password { get; set; }

        public string NickName { get; set; }

        public string Mobile { get; set; }

        public string Email { get; set; }

        public string Expire { get; set; }

        public int? Flag { get; set; }// = 1;

        public int? UserType { get; set; }

        public string Icon { get; set; }

        public DateTime CreateDate { get; set; }// = DateTime.Now;

        public string CreatorId { get; set; }

        public DateTime? ModifyDate { get; set; }

        public string Modifier { get; set; }

        public string Version { get; set; }

        [Column("Expire")]
        [Ignored]
        public string ExpireX2 { get; set; }
    }

    [Table("t_moon_user_role")]
    public class UserRole
    {
        [Key]
        [Sequence("SEQ_LOG")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string UserId { get; set; }
        public int RoleId { get; set; }
    }

    [Table("t_moon_role")]
    public class Role
    {
        [Key]
        [Sequence("SEQ_LOG")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string RoleCode { get; set; }
        public string RoleName { get; set; }
    }

    public class UserDto
    {
        public string Id { get; set; }

        public string Account { get; set; }

        public string NickName { get; set; }

        public string RoleCode { get; set; }

        public string RoleName { get; set; }

        public string x1 { get; set; }
    }
}

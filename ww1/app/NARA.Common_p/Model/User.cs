using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NARA.Common_p.Model
{
    /// <summary>
    /// Mapping class for storing User entity from
    /// museu.ms platform API
    /// </summary>
    public class User
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string RegistrationUserName { get; set; }
        public string Email { get; set; }
        public string RegistrationPassword { get; set; }
        public string RegistrationConfirmPassword { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public string ImgUrl { get; set; }
        public string Token { get; set; }
        public string ImageUrl { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public bool? IsCurator { get; set; }
        public string OldPassword { get; set; }
    }
}
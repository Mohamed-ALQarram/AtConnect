using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtConnect.BLL.DTOs
{
    public class UserProfileDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; } 
        public string UserName { get; set; } 
        public string ProfileImageUrl { get; set; }
        public bool isActive { get; set; }
        public string AboutUser { get; set; }
        public UserProfileDTO(string firstName, string lastName, string userName, string profileImageUrl, bool isActive, string aboutUser)
        {
            FirstName = firstName;
            LastName = lastName;
            UserName = userName;
            ProfileImageUrl = profileImageUrl;
            this.isActive = isActive;
            AboutUser = aboutUser;
        }

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace ShitheadApi.Models.Entities
{
    public class User : IdentityUser<Guid>
    {
        [Key]
        public override Guid Id { get; set; }

        public List<UserFriend> Friends { get; set; }

        [NotMapped]
        public bool NotSplitUserName { get; set; }

        public override string UserName
        {
            get
            {
                if (!NotSplitUserName && _userName.Contains('@')) return _userName.Split("@", 2)[0];
                return _userName;
            }
            set => _userName = value;
        }
        private string _userName;

        public string Surname { get; set; }

        public override string Email { get; set; }

        public override bool EmailConfirmed { get; set; }

        public override string PhoneNumber { get; set; }

        public override bool PhoneNumberConfirmed { get; set; }

        public string Image { get; set; }

        public DateTime Created { get; set; }

        public bool Blocked { get; set; }

        #region NotMapped

        [NotMapped]
        public string Password { get; set; }

        [NotMapped]
        public Credentials Credentials { get; set; }

        [NotMapped]
        public bool Conditions { get; set; }

        [NotMapped]
        public Email HtmlEmail { get; set; }

        #endregion
    }
}

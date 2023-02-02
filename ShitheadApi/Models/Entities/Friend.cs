using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShitheadApi.Models.Entities
{
    public class UserFriend
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("User")]
        public Guid UserId { get; set; }

        public User User { get; set; }

        [ForeignKey("Friend")]
        public Guid FriendId { get; set; }

        public User Friend { get; set; }
    }
}

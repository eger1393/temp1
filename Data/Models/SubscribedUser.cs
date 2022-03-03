using System;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Data.Models
{
    public class SubscribedUser
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        
        [ForeignKey(typeof(User))]
        public Guid FromUserId { get; set; }
        
        [ForeignKey(typeof(User))]
        public Guid SubscribedUserId { get; set; }
    }
}
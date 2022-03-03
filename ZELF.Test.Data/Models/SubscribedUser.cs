using System;
using SQLite;

namespace ZELF.Test.Data.Models
{
    public class SubscribedUser
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
    }
}
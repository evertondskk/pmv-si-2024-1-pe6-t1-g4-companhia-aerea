using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Domain.Dtos.UserAggregate
{
    public class UserDtoCreateResult
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string DateBirth { get; set; }
        public DateTime CreateAt { get; set; }
    }
}

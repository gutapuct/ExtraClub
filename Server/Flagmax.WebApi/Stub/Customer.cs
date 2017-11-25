using System;
using System.ComponentModel.DataAnnotations;

namespace Flagmax.WebApi.Stub
{
    public class Customer
    {
        [Key]
        public Guid Id { get; set; }

        public string Name { get; set; }
    }
}

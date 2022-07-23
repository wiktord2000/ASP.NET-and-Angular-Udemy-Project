using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

// DTO is the best place for validation the request data
namespace API.DTOs
{
    public class RegisterDto
    {
        // Validation the data annotation. 
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

    }
}
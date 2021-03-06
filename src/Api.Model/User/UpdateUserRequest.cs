﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SourceName.Api.Model.User
{
    public class UpdateUserRequest
    {
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        [Required]
        [MaxLength(255)]
        public string Username { get; set; }
    }
}

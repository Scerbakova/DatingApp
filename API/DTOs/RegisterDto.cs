﻿using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class RegisterDto
{
    [Required]
    public string UserName { get; set; }

    [Required]
    public string KnownAs { get; set; }

    [Required]
    public string Gender { get; set; }

    [Required]
    public DateOnly? DateOFBirth { get; set; } //optional to make required work

    [Required]
    public string City { get; set; }

    [Required]
    public string Country { get; set; }

    [StringLength(8, MinimumLength = 4)]
    [Required]
    public string Password { get; set; }
}

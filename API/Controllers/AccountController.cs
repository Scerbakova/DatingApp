using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(UserManager<AppUser> userManager, ITokenService tokenService, IMapper mapper) : BaseApiController
{
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly ITokenService _tokenService = tokenService;
    private readonly IMapper _mapper = mapper;

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)

    {
        if (await UserExists(registerDto.UserName)) return BadRequest("Username is taken");

        AppUser user = _mapper.Map<AppUser>(registerDto);

        user.UserName = registerDto.UserName.ToLower();

        IdentityResult result = await _userManager.CreateAsync(user, registerDto.Password);

        if(!result.Succeeded) return BadRequest(result.Errors);

        IdentityResult roleResult = await _userManager.AddToRoleAsync(user, "Member");

        if(!roleResult.Succeeded) return BadRequest(result.Errors);

        return new UserDto
        {
            UserName = user.UserName,
            Token = await _tokenService.CreateToken(user),
            KnownAs = user.KnownAs,
            Gender = user.Gender
        };
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        AppUser user = await _userManager.Users
        .Include(p => p.Photos)
        .SingleOrDefaultAsync(x => x.UserName == loginDto.UserName.ToLower());

        if (user == null) return Unauthorized("invalid username");

        bool result = await _userManager.CheckPasswordAsync(user, loginDto.Password);

        if(!result) return Unauthorized("Invalid password");

        return new UserDto
        {
            UserName = user.UserName,
            Token = await _tokenService.CreateToken(user),
            PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
            KnownAs = user.KnownAs,
            Gender = user.Gender
        };
    }

    private async Task<bool> UserExists(string username)
    {
        return await _userManager.Users.AnyAsync(x => x.UserName == username.ToLower());
    }
}

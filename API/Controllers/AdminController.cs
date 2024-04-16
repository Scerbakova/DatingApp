using API.Entities;
using API.Interfaces;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AdminController(UserManager<AppUser> userManager, IUnitOfWork uow, IPhotoService photoService) : BaseApiController
{
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly IUnitOfWork _uow = uow;
    private readonly IPhotoService _photoService = photoService;

    [Authorize(Policy = "RequireAdminRole")]
    [HttpGet("users-with-roles")]
    public async Task<ActionResult> GetUsersWithRoles()
    {
        var users = await _userManager.Users
        .OrderBy(u => u.UserName)
        .Select(u => new
        {
            u.Id,
            Username = u.UserName,
            Roles = u.UserRoles.Select(r => r.Role.Name).ToList()
        })
        .ToListAsync();

        return Ok(users);
    }

    [Authorize(Policy = "RequireAdminRole")]
    [HttpPost("edit-roles/{username}")]
    public async Task<ActionResult> EditRoles(string username, [FromQuery] string roles)
    {
        if (string.IsNullOrEmpty(roles)) return BadRequest("You must select at least one role");

        string[] selectedRoles = [.. roles.Split(",")];

        AppUser user = await _userManager.FindByNameAsync(username);
        if (user == null) return NotFound();

        IList<string> UserRoles = await _userManager.GetRolesAsync(user);

        var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(UserRoles));
        if (!result.Succeeded) return BadRequest("Failed to add to roles");

        result = await _userManager.RemoveFromRolesAsync(user, UserRoles.Except(selectedRoles));
        if (!result.Succeeded) return BadRequest("Failed to remove from roles");

        return Ok(await _userManager.GetRolesAsync(user));
    }


    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpGet("photos-to-moderate")]
    public async Task<ActionResult> GetPhotosForApproval()
    {
        return Ok(await _uow.PhotosRepository.GetUnapprovedPhotos());
    }

    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpPost("approve-photo/{photoId}")]

    public async Task<ActionResult> ApprovePhoto(int photoId)
    {
        Photo photo = await _uow.PhotosRepository.GetPhotoById(photoId);

        AppUser user = await _uow.UserRepository.GetUserByPhotoId(photoId);

        if (photo == null) return NotFound();

        photo.IsApproved = true;

        if (!user.Photos.Any(p => p.IsMain)) photo.IsMain = true;

        await _uow.Complete();

        return Ok();
    }

    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpPost("reject-photo/{photoId}")]

    public async Task<ActionResult> RejectPhoto(int photoId)
    {
        Photo photo = await _uow.PhotosRepository.GetPhotoById(photoId);
        if (photo.PublicId != null)
        {
            var result = await
            _photoService.DeletePhotoAsync(photo.PublicId);
            if (result.Result == "ok")
            {
                _uow.PhotosRepository.RemovePhoto(photo);
            }
        }
        else
        {
            _uow.PhotosRepository.RemovePhoto(photo);
        }
        await _uow.Complete();
        return Ok();
    }
}


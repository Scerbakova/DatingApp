using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
public class UsersController(IUnitOfWork uow, IMapper mapper, IPhotoService photoService) : BaseApiController
{
    private readonly IUnitOfWork _uow = uow;
    private readonly IMapper _mapper = mapper;
    private readonly IPhotoService _photoService = photoService;

    [HttpGet]
    public async Task<ActionResult<PagedList<MemberDto>>> GetUsers([FromQuery] UserParams userParams)
    {
        string gender = await _uow.UserRepository.GetUserGender(User.GetUserName());

        userParams.CurrentUsername = gender;

        if (string.IsNullOrEmpty(userParams.Gender))
        {
            userParams.Gender = gender == "male" ? "female" : "male";
        }

        PagedList<MemberDto> users = await _uow.UserRepository.GetMembersAsync(userParams);

        Response.AddPaginationHeader(new PaginationHeader
        (users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages));

        return Ok(users);
    }

    [HttpGet("{userName}")]
    public async Task<ActionResult<MemberDto>> GetUser(string userName)
    {
        var currentUsername = User.GetUserName();
        return await _uow.UserRepository.GetMemberAsync(userName,
        isCurrentUser: currentUsername == userName
        );
    }

    [HttpPut]
    public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
    {
        AppUser user = await _uow.UserRepository.GetUserByUserNameAsync(User.GetUserName());

        if (user == null) return NotFound();

        _mapper.Map(memberUpdateDto, user);

        if (await _uow.Complete()) return NoContent();

        return BadRequest("Failed to update user");
    }

    [HttpPost("add-photo")]
    public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
    {
        AppUser user = await _uow.UserRepository.GetUserByUserNameAsync(User.GetUserName());

        if (user == null) return NotFound();

        ImageUploadResult result = await _photoService.AddPhotoAsync(file);

        if (result.Error != null) return BadRequest(result.Error.Message);

        Photo photo = new()
        {
            Url = result.SecureUrl.AbsoluteUri,
            PublicId = result.PublicId
        };

        user.Photos.Add(photo);

        if (await _uow.Complete())
        {
            return CreatedAtAction(nameof(GetUser),
            new { userName = user.UserName },
            _mapper.Map<PhotoDto>(photo));
        }

        return BadRequest("Problem adding photo");
    }

    [HttpPut("set-main-photo/{photoId}")]
    public async Task<ActionResult> SetMainPhoto(int photoId)
    {
        AppUser user = await _uow.UserRepository.GetUserByUserNameAsync(User.GetUserName());

        if (user == null) return NotFound();

        Photo photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

        if (photo == null) return NotFound();

        if (photo.IsMain) return BadRequest("this is already your main photo");

        Photo currentMain = user.Photos.FirstOrDefault(x => x.IsMain);

        if (currentMain != null) currentMain.IsMain = false;

        photo.IsMain = true;

        if (await _uow.Complete()) return NoContent();

        return BadRequest("Problem setting the main photo");
    }

    [HttpDelete("delete-photo/{photoId}")]
    public async Task<ActionResult> DeletePhoto(int photoId)
    {
        AppUser user = await _uow.UserRepository.GetUserByUserNameAsync(User.GetUserName());

        if (user == null) return NotFound();

        Photo photo = await _uow.PhotosRepository.GetPhotoById(photoId);

        if (photo == null) return NotFound();

        if (photo.IsMain) return BadRequest("You cannot delete your main photo");

        if (photo.PublicId != null)
        {
            DeletionResult result = await _photoService.DeletePhotoAsync(photo.PublicId);
            if (result.Error != null) return BadRequest(result.Error.Message);
        }

        user.Photos.Remove(photo);

        if (await _uow.Complete()) return Ok();

        return BadRequest("Problem deleting photo");
    }
}

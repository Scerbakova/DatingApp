using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class LikesController(IUserRepository userRepository, ILikesRepository likesRepository) : BaseApiController
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly ILikesRepository _likesRepository = likesRepository;

    [HttpPost("{username}")]
    public async Task<ActionResult> AddLike(string username)
    {
        int sourceUserId = User.GetUserId();
        var likedUser = await _userRepository.GetUserByUserNameAsync(username);
        var sourceUser = await _likesRepository.GetUserWithLikes(sourceUserId);

        if (likedUser == null) return NotFound();

        if (sourceUser.UserName == username) return BadRequest("You cannot like yourself");

        var userLike = await _likesRepository.GetUserLike(sourceUserId, likedUser.Id);

        if (userLike != null) return BadRequest("You already like this user");

        userLike = new UserLike
        {
            SourceUserId = sourceUserId,
            TargetUserId = likedUser.Id
        };

        sourceUser.LikedUsers.Add(userLike);

        if (await _userRepository.SaveAllAsync()) return Ok();
        return BadRequest("Failed to like user");
    }
}

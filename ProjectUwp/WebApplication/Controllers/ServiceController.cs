using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Dtos;
using WebApplication.Components.Database;
using WebApplication.Components.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ServiceController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _dbContext;

        public ServiceController(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext dbContext
            )
        {
            _userManager = userManager ?? throw new NullReferenceException();
            _dbContext = dbContext ?? throw new NullReferenceException();
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> SetShool([FromBody] UserDataSettingsDto dto)
        {
            var user = await _userManager.FindByIdAsync(this.User.Identity.Name);
            user.School = dto.School;
            await _dbContext.SaveChangesAsync();
            return Ok(new UserDataSettingsDto { Description = user.Description, Job = user.Job, School = user.School });
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> SetJob([FromBody] UserDataSettingsDto dto)
        {
            var user = await _userManager.FindByIdAsync(this.User.Identity.Name);
            user.Job = dto.Job;
            await _dbContext.SaveChangesAsync();
            return Ok(new UserDataSettingsDto { Description = user.Description, Job = user.Job, School = user.School });
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> SetDescription([FromBody] UserDataSettingsDto dto)
        {
            var user = await _userManager.FindByIdAsync(this.User.Identity.Name);
            user.Description = dto.Description;
            await _dbContext.SaveChangesAsync();
            return Ok(new UserDataSettingsDto { Description = user.Description, Job = user.Job, School = user.School });
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> SetProfileData([FromBody] UserDataSettingsDto dto)
        {
            var user = await _userManager.FindByIdAsync(this.User.Identity.Name);
            user.Description = dto.Description;
            user.Job = dto.Job;
            user.School = dto.School;
            await _dbContext.SaveChangesAsync();
            return Ok(new UserDataSettingsDto { Description = user.Description, Job = user.Job, School = user.School });
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> SetSearchParameters([FromBody] SearchParameterDto dto)
        {
            var user = await _userManager.FindByIdAsync(this.User.Identity.Name);
            user.MinAge = dto.MinAge;
            user.MaxAge = dto.MaxAge;
            user.MaxDist = dto.MaxDist;
            user.SearchedGender = dto.SearchedGender;
            await _dbContext.SaveChangesAsync();
            return Ok(new SearchParameterDto { MinAge = user.MinAge, MaxAge = user.MaxAge, MaxDist = user.MaxDist, SearchedGender = user.SearchedGender });
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetSearchParameters()
        {
            var user = await _userManager.FindByIdAsync(this.User.Identity.Name);
            return Ok(new SearchParameterDto { MinAge = user.MinAge, MaxAge = user.MaxAge, MaxDist = user.MaxDist, SearchedGender = user.SearchedGender });
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetMyProfile()
        {
            var user = await _userManager.FindByIdAsync(this.User.Identity.Name);
            var userWithPhotos = await _dbContext.Users.Where(q => q.Id == user.Id).Include(q => q.Photos).FirstAsync();
            var ret = new ProfileDto
            {
                Age = user.Age,
                Description = user.Description,
                Gender = user.Gender,
                Id = user.Id,
                Job = user.Job,
                Name = user.Name,
                School = user.School,
            };
            foreach (var item in userWithPhotos.Photos)
            {
                ret.Photos.Add(new PhotoDto { Id = item.Id, Url = item.Url, Main = item.Main, Extenstion = item.Extenstion, FileName = item.FileName });
            }
            return Ok(ret);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetProfile()
        {
            var user = await _dbContext.Users.Where(q => q.Id == this.User.Identity.Name).Include(q => q.LikedUsers).FirstOrDefaultAsync();
            if (user is null)
                return BadRequest();

            var userWithPhotosList = await _dbContext.Users
                .Where(q => user.IsAgeOk(q.Age))
                .Where(q => user.IsGenderOk(q.Gender) && q.IsGenderOk(user.Gender))
                .Where(q => user.IsDistOk(q.Latitude, q.Longitude))
                .Where(q => !user.IsElementLikedUsersWithDate(q.Id, DateTime.Now.AddMonths(-3)))
                .Include(q => q.Photos).ToListAsync();

            if (userWithPhotosList.Count == 0)
                return NotFound();

            var userWithPhotos = userWithPhotosList[new Random().Next(0, userWithPhotosList.Count)];

            var ret = new ProfileDto
            {
                Age = userWithPhotos.Age,
                Description = userWithPhotos.Description,
                Gender = userWithPhotos.Gender,
                Id = userWithPhotos.Id,
                Job = userWithPhotos.Job,
                Name = userWithPhotos.Name,
                School = userWithPhotos.School,
            };
            foreach (var item in userWithPhotos.Photos)
            {
                ret.Photos.Add(new PhotoDto { Id = item.Id, Url = item.Url, Main = item.Main, Extenstion = item.Extenstion, FileName = item.FileName });
            }
            return Ok(ret);
        }
        
        [HttpPost("[action]")]
        public async Task<IActionResult> Liked(string Id)
        {
            if (string.IsNullOrEmpty(Id) || Id == this.User.Identity.Name)
                return BadRequest();

            var likedUser = await _dbContext.Users.Where(q => q.Id == Id).Include(q => q.LikedUsers).FirstOrDefaultAsync();
            if (likedUser is null)
                return BadRequest();

            var user = await _dbContext.Users.Where(q => q.Id == this.User.Identity.Name).Include(q => q.LikedUsers).FirstOrDefaultAsync();
            if (user is null)
                return BadRequest();
            if (user.IsElementLikedUsers(likedUser.Id))
                return BadRequest();

            if(likedUser.IsLikedThisUser(user.Id))
            {
                await _dbContext.Matches.AddAsync(new Match { UserId_1 = user.Id, UserId_2 = likedUser.Id, IsActive = true });               
            }
            user.LikedUsers.Add(new LikedUser { UserId = likedUser.Id, IsLiked = true });

            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Passed(string Id)
        {
            if (string.IsNullOrEmpty(Id) || Id == this.User.Identity.Name)
                return BadRequest();

            var likedUser = await _userManager.FindByIdAsync(Id);
            if (likedUser is null)
                return BadRequest();

            var user = await _dbContext.Users.Where(q => q.Id == this.User.Identity.Name).Include(q => q.LikedUsers).FirstOrDefaultAsync();
            if (user is null)
                return BadRequest();
            if (user.IsElementLikedUsers(likedUser.Id))
            {
                var q = user.LikedUsers.Where(w => w.UserId == likedUser.Id).First();
                q.IsLiked = false;
                return Ok();
            }
            user.LikedUsers.Add(new LikedUser { IsLiked = false, UserId = likedUser.Id });
            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetMatches()
        {
            try
            {
                var matches = await _dbContext.Matches.Where(q => q.IsActive == true && (q.UserId_1 == User.Identity.Name || q.UserId_2 == User.Identity.Name)).Include(q => q.Messages).ToListAsync();
                if (matches is null || matches.Count == 0)
                    return NotFound();
                var ret = new List<MatchDto>();
                foreach (var item in matches)
                {
                    var id = item.UserId_1 == this.User.Identity.Name ? item.UserId_2 : item.UserId_1;
                    var userWithPicture = await _dbContext.Users.Where(q => q.Id == id).Include(q => q.Photos).FirstOrDefaultAsync();
                    var mess = item.GetLastMessage();
                    ret.Add(new MatchDto { Id = item.Id, LastMessage = mess?.Data, LastSenderId = mess?.SenderId, Name = userWithPicture?.Name, PicUrl = userWithPicture.GetMainPhoto()?.Url });
                }
                return Ok(ret);
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> CanceMatch(string matchId)
        {
            var match = await _dbContext.Matches.FindAsync(matchId);
            match.IsActive = false;

            var user1 = await _dbContext.Users.Where(q => q.Id == match.UserId_1).Include(q => q.LikedUsers).FirstOrDefaultAsync();
            var user2 = await _dbContext.Users.Where(q => q.Id == match.UserId_2).Include(q => q.LikedUsers).FirstOrDefaultAsync();
            user1.SetLikedUserLiked(user2.Id, false);
            user2.SetLikedUserLiked(user1.Id, false);

            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetMessages(string matchId)
        {
            var match = await _dbContext.Matches.Where(q => q.IsActive == true && q.Id == matchId).Include(q => q.Messages).FirstOrDefaultAsync();
            var ret = new List<MessageDto>();
            var mess = match.GetOrderedMessages();
            foreach (var item in mess)
            {
                ret.Add(new MessageDto { Data = item.Data, SenderId = item.SenderId, Created = item.LatestUpdate });
            }
            return Ok(ret);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> SendMessage(string matchId, [FromBody] MessageDto dto)
        {
            if (String.IsNullOrEmpty(matchId) || dto is null || dto.Data is null)
                return Ok();
            var match = await _dbContext.Matches.Where(q => q.Id == matchId).FirstOrDefaultAsync();
            //if (match.UserId_1 != this.User.Identity.Name || match.UserId_2 != this.User.Identity.Name)
            //    return BadRequest();
            match.Messages.Add(new Message { Data = dto.Data, SenderId = this.User.Identity.Name });
            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> UpdateLocation([FromBody] LocationDto dto)
        {
            if (dto is null)
                return BadRequest();
            var user = await _userManager.FindByIdAsync(this.User.Identity.Name);
            user.Longitude = dto.Longitude;
            user.Latitude = dto.Latitude;
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}

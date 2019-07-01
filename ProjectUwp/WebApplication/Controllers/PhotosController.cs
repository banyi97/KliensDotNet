using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Dtos;
using WebApplication.Components.Database;
using WebApplication.Components.Models;
using WebApplication.Components.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class PhotosController : Controller
    {
        private readonly AzureStorageConfig _storageConfig;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _dbContext;
        public PhotosController(
            AzureStorageConfig storageConfig,
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext dbContext
            )
        {
            _storageConfig = storageConfig ?? throw new NullReferenceException();
            _userManager = userManager ?? throw new NullReferenceException();
            _dbContext = dbContext ?? throw new NullReferenceException();
        }     

        [HttpPost("[action]")]
        public async Task<IActionResult> Upload(ICollection<IFormFile> files)
        {
            bool isUploaded = false;

            try
            {
                if (files is null || files.Count == 0)
                    return BadRequest("No files received from the upload");

                if (_storageConfig.AccountKey == string.Empty || _storageConfig.AccountName == string.Empty)
                    return BadRequest("sorry, can't retrieve your azure storage details from appsettings.js, make sure that you add azure storage details there");

                if (_storageConfig.ImageContainer == string.Empty)
                    return BadRequest("Please provide a name for your image container in the azure blob storage");
                var user = await _userManager.FindByIdAsync(this.User.Identity.Name);
                foreach (var formFile in files)
                {
                    if (StorageHelper.IsImage(formFile))
                    {
                        if (formFile.Length > 0)
                        {
                            using (Stream stream = formFile.OpenReadStream())
                            {
                                var ext = formFile.FileName.Substring(formFile.FileName.LastIndexOf("."));
                                var fileName = Guid.NewGuid().ToString() + ext;                               
                                if(isUploaded = await StorageHelper.UploadFileToStorage(stream, fileName, _storageConfig))
                                {
                                    user.Photos.Add(new Photo { FileName = fileName, Main = false });
                                }
                            }                           
                        }
                    }
                    else
                    {
                        return new UnsupportedMediaTypeResult();
                    }
                }
                await _dbContext.SaveChangesAsync();
                if (isUploaded)
                {
                    return new AcceptedResult();
                }
                else
                    return BadRequest("Look like the image couldnt upload to the storage");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("[action]")]     
        public async Task<IActionResult> PostFile(IFormFile uploadedFile)
        {
            bool isUploaded = false;

            try
            {
                if (uploadedFile is null)
                    return BadRequest();
                if (_storageConfig.AccountKey == string.Empty || _storageConfig.AccountName == string.Empty)
                    return BadRequest("sorry, can't retrieve your azure storage details from appsettings.js, make sure that you add azure storage details there");

                if (_storageConfig.ImageContainer == string.Empty)
                    return BadRequest("Please provide a name for your image container in the azure blob storage");

                var user = await _userManager.FindByIdAsync(this.User.Identity.Name);
                if (StorageHelper.IsImage(uploadedFile))
                    {
                        if (uploadedFile.Length > 0)
                        {
                            using (Stream stream = uploadedFile.OpenReadStream())
                            {
                            var ext = uploadedFile.FileName.Substring(uploadedFile.FileName.LastIndexOf("."));
                            var fileName = Guid.NewGuid().ToString() + ext;
                            if (isUploaded = await StorageHelper.UploadFileToStorage(stream, fileName, _storageConfig))
                            {
                                user.Photos.Add(new Photo { FileName = fileName, Main = false });
                            }
                        }
                        }
                    }
                    else
                    {
                        return new UnsupportedMediaTypeResult();
                    }
                await _dbContext.SaveChangesAsync();
                if (isUploaded)
                {
                        return new AcceptedResult();
                }
                else
                    return BadRequest("Look like the image couldnt upload to the storage");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpGet("[action]")]
        public async Task<IActionResult> GetPhotos()
        {
            try
            {
                var user = await _dbContext.Users.Where(q => q.Id == this.User.Identity.Name).Include(q => q.Photos).FirstOrDefaultAsync();
                return Ok(user.Photos);
            }
            catch(Exception e)
            {
                return BadRequest(e);
            }        
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> SetMainPhoto([FromQuery] string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return BadRequest();
                var photos = await _dbContext.Users.Where(q => q.Id == this.User.Identity.Name).Include(q => q.Photos).Select(q => q.Photos).FirstOrDefaultAsync();
                if (photos is null)
                    return BadRequest();
                foreach (var item in photos)
                {
                    if (item.Main == true)
                        item.Main = false;
                    if (item.Id == id)
                        item.Main = true;
                }
                await _dbContext.SaveChangesAsync();
                return Ok(photos);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpDelete("[action]")]
        public async Task<IActionResult> DeletePhoto([FromQuery] string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return BadRequest();
                var photos = await _dbContext.Users.Where(q => q.Id == this.User.Identity.Name).Include(q => q.Photos).Select(q => q.Photos).FirstOrDefaultAsync();
                if (photos is null)
                    return BadRequest(); 
                foreach (var item in photos)
                {
                    if (item.Id == id)
                    {
                        photos.Remove(item);
                        _dbContext.Photos.Remove(item);
                        break;
                    }
                }
                await _dbContext.SaveChangesAsync();
                return Ok(photos);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
    }
}

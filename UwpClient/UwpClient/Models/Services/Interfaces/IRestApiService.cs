using Refit;
using SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UwpClient.Models.Services.Interfaces
{
    public interface IRestApiService
    {
        // ### Auth ###

        [Post("/api/Auth/SignIn")]
        Task<AuthDto> SignIn([Body] SignInDto dto);
        [Post("/api/Auth/Register")]
        Task<AuthDto> Register([Body] RegisterDto dto);

        // ### Photo ###

        [Multipart]
        [Post("/api/Photos/Upload")]
        Task Upload([AliasAs("files")] IEnumerable<StreamPart> streams, [Header("Authorization")] string authorization);
        [Multipart]
        [Post("/api/Photos/PostFile")]
        Task PostFile([AliasAs("uploadedFile")] StreamPart uploadedFile, [Header("Authorization")] string authorization);
        [Get("/api/Photos/GetPhotos")]
        Task<List<PhotoDto>> GetPhotos([Header("Authorization")] string authorization);
        [Put("/api/Photos/SetMainPhoto")]
        Task<List<PhotoDto>> SetMainPhoto([Query] string id, [Header("Authorization")] string authorization);
        [Delete("/api/Photos/DeletePhoto")]
        Task<List<PhotoDto>> DeletePhoto([Query] string id, [Header("Authorization")] string authorization);

        // ### Service ###

        [Put("/api/Service/SetShool")]
        Task<UserDataSettingsDto> SetShool([Body] UserDataSettingsDto dto, [Header("Authorization")] string authorization);
        [Put("/api/Service/SetJob")]
        Task<UserDataSettingsDto> SetJob([Body] UserDataSettingsDto dto, [Header("Authorization")] string authorization);
        [Put("/api/Service/SetDescription")]
        Task<UserDataSettingsDto> SetDescription([Body] UserDataSettingsDto dto, [Header("Authorization")] string authorization);
        [Put("/api/Service/SetProfileData")]
        Task<UserDataSettingsDto> SetProfileData ([Body] UserDataSettingsDto dto, [Header("Authorization")] string authorization);
        [Put("/api/Service/SetSearchParameters")]
        Task<SearchParameterDto> SetSearchParameters([Body] SearchParameterDto dto, [Header("Authorization")] string authorization);
        [Put("/api/Service/UpdateLocation")]
        Task UpdateLocation([Body] LocationDto dto, [Header("Authorization")] string authorization);

        [Get("/api/Service/GetSearchParameters")]
        Task<SearchParameterDto> GetSearchParameters([Header("Authorization")] string authorization);
        [Get("/api/Service/GetMyProfile")]
        Task<ProfileDto> GetMyProfile([Header("Authorization")] string authorization);
        [Get("/api/Service/GetProfile")]
        Task<ProfileDto> GetProfile([Header("Authorization")] string authorization);
        [Get("/api/Service/GetMatches")]
        Task<IList<MatchDto>> GetMatches([Header("Authorization")] string authorization);
        [Get("/api/Service/CanceMatch")]
        Task CanceMatch([Query] string matchId, [Header("Authorization")] string authorization);
        [Get("/api/Service/GetMessages")]
        Task<IList<MessageDto>> GetMessages([Query] string matchId, [Header("Authorization")] string authorization);

        [Post("/api/Service/Liked")]
        Task Liked([Query] string Id, [Header("Authorization")] string authorization);
        [Post("/api/Service/Passed")]
        Task Passed([Query] string Id, [Header("Authorization")] string authorization);
        [Post("/api/Service/SendMessage")]
        Task SendMessage([Query] string matchId, [Body] MessageDto dto , [Header("Authorization")] string authorization);

        [Post("/api/FacebookAuth/SignInWithFacebook")]
        Task<AuthDto> SignInWithFacebook([Query] string authToken);

    }
}

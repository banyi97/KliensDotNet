using Microsoft.AspNetCore.Identity;
using SharedLibrary.Interfaces;
using SharedLibrary.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Components.Models
{
    public class ApplicationUser : IdentityUser, IUserData, IPosition, ISearchParameters, IAuditableEntity
    {
        public IList<Photo> Photos { get; set; } = new List<Photo>();
        public IList<LikedUser> LikedUsers { get; set; } = new List<LikedUser>();

        public DateTime DateOfBirth { get; set; }

        public string Name { get; set; }
        [NotMapped]
        public int Age // https://stackoverflow.com/questions/9/how-do-i-calculate-someones-age-in-c
        {
            get
            {
                var today = DateTime.Today;
                // Calculate the age.
                var age = today.Year - DateOfBirth.Year;
                // Go back to the year the person was born in case of a leap year
                if (DateOfBirth > today.AddYears(-age)) age--;
                return age;
            }
            set { throw new NotImplementedException(); }
        }
        public string Job { get; set; }
        public string School { get; set; }
        public string Description { get; set; }
        public Gender Gender { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public int MinAge { get; set; }
        public int MaxAge { get; set; }
        public double MaxDist { get; set; }
        public Gender SearchedGender { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime LatestUpdate { get; set; }

        public virtual bool IsAgeOk(int age)
        {
            if(age <= this.MaxAge && age >= this.MinAge)
                return true;
            return false;
        }

        public virtual bool IsGenderOk(Gender gender)
        {
            switch (this.SearchedGender)
            {
                case Gender.Female: if (gender == Gender.Female) return true;
                    break;
                case Gender.Male: if (gender == Gender.Male) return true;
                    break;
                case Gender.All: return true;
            }
            return false;
        }

        public virtual bool IsDistOk(double latitude, double longitude)
        {
            return true;
        }

        public virtual bool IsElementLikedUsers(string Id)
        {
            foreach (var item in this.LikedUsers)
            {
                if (item.UserId == Id)
                    return true;
            }
            return false;
        }

        public virtual bool IsLikedThisUser(string Id)
        {
            foreach (var item in this.LikedUsers)
            {
                if (item.UserId == Id && item.IsLiked == true)
                    return true;
            }
            return false;
        }

        public virtual bool IsElementLikedUsersWithDate(string Id, DateTime date)
        {
            foreach (var item in this.LikedUsers)
            {
                if (item.UserId == Id)
                {
                    if (item.IsLiked == true)
                        return true;
                    else if (item.LatestUpdate <= date)
                        return false;
                    else
                        return true;
                }
            }
            return false;
        }

        public virtual void SetLikedUserLiked(string id, bool isLiked)
        {
            foreach (var item in this.LikedUsers)
            {
                if(item.UserId == id)
                {
                    item.IsLiked = isLiked;
                    return;
                }
            }
        }

        public virtual Photo GetMainPhoto()
        {
            foreach (var item in this.Photos)
            {
                if (item.Main == true)
                    return item;
            }
            return null;
        }
    }
}

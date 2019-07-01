using FluentValidation;
using FluentValidation.Validators;
using SharedLibrary.Dtos;
using SharedLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Components.Validation
{
    public class RegisterValidatior : AbstractValidator<RegisterDto>
    {
        public RegisterValidatior()
        {
            RuleFor(q => q.Email).EmailAddress();
            RuleFor(q => q.DateOfBirth).LessThanOrEqualTo(DateTime.Now.AddYears(-18));
            RuleFor(q => q.Gender).Custom((q, w) => 
            {
                if (q is Gender.Female || q is Gender.Male)
                    return;
                else w.AddFailure("Gender is not valid");
            });
            RuleFor(q => q.Name).MinimumLength(2);
        }
    }
}

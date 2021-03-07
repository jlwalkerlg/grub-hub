﻿using Web.Services.Validation;

namespace Web.Features.Users.UpdateAccountDetails
{
    public class UpdateAccountDetailsValidator : FluentValidator<UpdateAccountDetailsCommand>
    {
        public UpdateAccountDetailsValidator()
        {
            CascadeRuleFor(x => x.FirstName)
                .Required();

            CascadeRuleFor(x => x.LastName)
                .Required();

            CascadeRuleFor(x => x.MobileNumber)
                .Required()
                .MobileNumber();
        }
    }
}

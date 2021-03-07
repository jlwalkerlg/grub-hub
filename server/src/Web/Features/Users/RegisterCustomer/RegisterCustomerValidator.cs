﻿using Web.Services.Validation;

namespace Web.Features.Users.RegisterCustomer
{
    public class RegisterCustomerValidator : FluentValidator<RegisterCustomerCommand>
    {
        public RegisterCustomerValidator()
        {
            CascadeRuleFor(x => x.FirstName)
                .Required();

            CascadeRuleFor(x => x.LastName)
                .Required();

            CascadeRuleFor(x => x.Email)
                .Required()
                .Email();

            CascadeRuleFor(x => x.Password)
                .Required()
                .Password();
        }
    }
}

﻿using System.Threading;
using System.Threading.Tasks;
using FoodSnap.Application.Services.Authentication;
using FoodSnap.Application.Services.Hashing;
using FoodSnap.Domain;

namespace FoodSnap.Application.Users.Login
{
    public class LoginHandler : IRequestHandler<LoginCommand>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IAuthenticator authenticator;
        private readonly IHasher hasher;

        public LoginHandler(
            IUnitOfWork unitOfWork,
            IAuthenticator authenticator,
            IHasher hasher)
        {
            this.unitOfWork = unitOfWork;
            this.authenticator = authenticator;
            this.hasher = hasher;
        }

        public async Task<Result> Handle(LoginCommand command, CancellationToken cancellationToken)
        {
            var user = await unitOfWork.Users.GetByEmail(command.Email);

            if (user == null)
            {
                return Result.Fail(Error.BadRequest("Invalid credentials."));
            }

            if (!hasher.CheckMatch(command.Password, user.Password))
            {
                return Result.Fail(Error.BadRequest("Invalid credentials."));
            }

            authenticator.SignIn(user);

            return Result.Ok();
        }
    }
}

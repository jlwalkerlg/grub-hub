﻿using System.Threading.Tasks;
using Web.Services.Validation;
using Web;

namespace ApplicationTests.Validation
{
    public class DummyCommandValidatorSpy : IValidator<DummyCommand>
    {
        public Result Result { get; set; } = Result.Ok();

        public Task<Result> Validate(DummyCommand request)
        {
            return Task.FromResult(Result);
        }
    }
}

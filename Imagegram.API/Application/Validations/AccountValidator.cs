using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Imagegram.API.Application.Validations
{
    public class CreateAccountValidator : AbstractValidator<CreateAccountQuery>
    {
        public CreateAccountValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("The 'Name' parameter is required.");
            RuleFor(x => x.Name).MinimumLength(3).WithMessage("The 'Name' should be minimum 3 characters");
        }
    }
    public class CreateAccountQuery
    {
        /// <summary>
        /// The name to for the account to be created
        /// </summary>
        public string Name { get; set; }
    }

    public class DeleteAccountValidator : AbstractValidator<DeleteAccountQuery>
    {
        public DeleteAccountValidator()
        {
            RuleFor(x => x.UUID).NotEmpty().WithMessage("'UUID' parameter is required.");
        }
    }

    public class DeleteAccountQuery
    {
        /// <summary>
        /// The UUID (account) to be deleted
        /// </summary>
        public string UUID { get; set; }
    }
}

using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Imagegram.API.Application.Validations
{
    public class CommentValidator : AbstractValidator<CreateCommentQuery>
    {
        public CommentValidator()
        {
            RuleFor(x => x.Comment).NotEmpty().WithMessage("The 'Comment' parameter is required.");
            RuleFor(x => x.Comment).MinimumLength(2).WithMessage("The 'Comment' parameter should be minimum 2 characters");
        }
    }
    public class CreateCommentQuery
    {
        /// <summary>
        /// Comment to be posted
        /// </summary>
        public string Comment { get; set; }
    }

    public class SearchCommentValidator
    {
    }

    public class SearchCommentQuery : SearchBase
    {
    }
}

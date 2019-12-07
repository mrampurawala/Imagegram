using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Imagegram.API.Application.Validations
{
    public class SearchPostValidator
    {
    }

    public class SearchPostQuery : SearchBase
    {
    }

    public class CreatePostValidator : AbstractValidator<CreatePostQuery>
    {
        //validateImage..
        public CreatePostValidator()
        {
            //RuleFor(x => x.File)...NotEmpty().WithMessage("The 'Name' parameter is required.");
        }
    }

    public class CreatePostQuery
    {
        public byte[] Image { get; set; }
        public string Comment { get; set; }
        public IFormFile File { get; set; }
    }
}

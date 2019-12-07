using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Imagegram.API.Application.Entities;
using Imagegram.API.Application.Repositories;
using Imagegram.API.Application.Validations;
using Imagegram.API.Helpers;
using Imagegram.API.Infrastructure.Configurations;
using Imagegram.API.Infrastructure.Middlewares;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using static Imagegram.API.Application.Enums.Constants;

namespace Imagegram.API.Controllers
{
    [ApiExplorerSettings(GroupName = "imagegram")]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IPostRepository _postRepository;
        private readonly IRequestHeaders _requestHeaders;
        private readonly ImageContentType _imageContentType;
        private readonly IImageFormatter _imageFormatter;

        private readonly string UUID = string.Empty;

        public PostController(IPostRepository postRepository, IRequestHeaders requestHeaders, ImageContentType imageContentType, IImageFormatter imageFormatter)
        {
            _postRepository = postRepository;
            _requestHeaders = requestHeaders;
            _imageContentType = imageContentType;
            _imageFormatter = imageFormatter;
            if (!string.IsNullOrWhiteSpace(_requestHeaders.UUID))
            {
                UUID = _requestHeaders.UUID;
            }
        }

        /// <summary>
        /// Get a list of posts
        /// </summary>
        /// <remarks>
        /// Returns a list of postsfrom all users along with last 3 comments to each post.
        /// </remarks>
        /// <returns>Returns a list of postsfrom all users along with last 3 comments to each post</returns>
        /// <response code="200">
        /// {
        /// }
        /// </response>
        /// <response code="204">Current page is empty</response>
        /// <response code="400">List of validation errors</response>
        [ProducesResponseType(typeof(List<Post>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesErrorResponseType(typeof(void))]
        [HttpGet]
        public async Task<IActionResult> GetAllPosts([FromQuery] SearchPostQuery query)
        {
            var result = await _postRepository.GetAllPosts(query.limit, query.page);
            if (result != null && Enumerable.Count(result) > 0) 
            {
                //var totalRecords = result.Select(row => row.TotalCount).FirstOrDefault();
                //Response.Headers.Add("total-count", totalRecords.ToString());
                //Response.Headers.Add("page-index", query.page.ToString());

                return Ok(result.ToList());
            }
            else
                return NoContent();
        }

        /// <summary>
        /// Create Post with image and comment
        /// </summary>
        /// <remarks>
        /// Returns a post id after a successful Post creation.
        /// </remarks>
        /// <returns>post id</returns>
        /// <response code="200">
        /// {
        ///     "Post ID": string
        /// }
        /// </response>
        /// <response code="201">Successful Post creation</response>
        /// <response code="400">List of validation errors</response>
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesErrorResponseType(typeof(void))]
        [HttpPost]
        public async Task<IActionResult> PostImage([FromForm] IFormFile uploadedFile, [FromForm] string comment) //CreatePostQuery query)
        {
            int width = Convert.ToInt32(_imageContentType.Width);
            if (uploadedFile.ContentType != null)
            {
                if (!_imageContentType.AllowedContentType.Contains(uploadedFile.ContentType))
                    return BadRequest("File type not supported");
                else
                {
                    if (uploadedFile.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            await uploadedFile.CopyToAsync(ms);
                            var fileBytes = ms.ToArray();
                            var convertedImageByte = _imageFormatter.CropAndConvert(fileBytes, 0, width, ImageFormat.JPG);
                            
                            var result = await _postRepository.CreatePost(convertedImageByte, UUID, comment);
                            if (!string.IsNullOrEmpty(result))
                                return Created("/post/" + result, result);
                            else
                                return BadRequest();
                        }
                    }
                    else
                        return BadRequest("Empty file");
                }
            }
            else
                return BadRequest();
        }

        /// <summary>
        /// Create a Comment for a post
        /// </summary>
        /// <remarks>
        /// Returns a comment id after a successful Comment creation.
        /// </remarks>
        /// <returns>comment id</returns>
        /// <response code="200">
        /// {
        ///     "Comment ID": string
        /// }
        /// </response>
        /// <response code="201">Successful Comment creation</response>
        /// <response code="400">List of validation errors</response>
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesErrorResponseType(typeof(void))]
        [HttpPost]
        [Consumes("application/json")]
        [Route("{postid}/comment")]
        public async Task<IActionResult> CreateComment(int postid, [FromBody] CreateCommentQuery query)
        {
            var result = await _postRepository.CreateComment(postid, query.Comment, UUID);
            if (string.IsNullOrEmpty(result))
                return BadRequest();
            else
                return Created("/post/" + postid + "/comment/" + result, result);
        }

        /// <summary>
        /// Delete Comment
        /// </summary>
        /// <remarks>
        /// Returns a comment id for a successful Comment deletion.
        /// </remarks>
        /// <returns>commentid</returns>
        /// <response code="200">Successful Comment Deletion</response>
        /// <response code="204">No account exists</response>
        [HttpDelete]
        [Route("{postid}/comment/{commentid}")]
        public async Task<IActionResult> DeleteComment(int postid, int commentid)
        {
            var result = await _postRepository.DeleteComment(postid, commentid);
            if (string.IsNullOrEmpty(result))
                return NoContent();
            else
                return Ok();
        }

        /// <summary>
        /// Get a list of comments
        /// </summary>
        /// <remarks>
        /// Returns a list of comments on a particular post.
        /// </remarks>
        /// <returns>A list of comments for a post</returns>
        /// <response code="200">
        /// {
        /// }
        /// </response>
        /// <response code="204">Current page is empty</response>
        /// <response code="400">List of validation errors</response>
        [ProducesResponseType(typeof(Comment), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesErrorResponseType(typeof(void))]
        [HttpGet]
        [Route("{postid}/comment")]
        public async Task<IActionResult> GetComment(int postid, [FromQuery] SearchCommentQuery query)
        {
            var comments = await _postRepository.GetComment(postid, query.limit, query.page);
            //var displayProps = new List<string>() { "CommentId", "Content", "CreatedDate", "CreatorUUID", "PostId" };

            //var result = comments.Select(row => new Dictionary<string, object>(row, StringComparer.OrdinalIgnoreCase)).
                //Select(row => displayProps.ToDictionary(c => c, c => row.ContainsKey(c) ? row[c] : null));

            if (comments != null && Enumerable.Count(comments) > 0)
            {
                var totalRecords = comments.Select(row => row.TotalCount).FirstOrDefault();
                Response.Headers.Add("total-count", totalRecords.ToString());
                Response.Headers.Add("page-index", query.page.ToString());

                return Ok(comments);
            }
            else
                return NoContent();
        }
    }
}
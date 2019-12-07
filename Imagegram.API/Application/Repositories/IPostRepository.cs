using Imagegram.API.Application.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Imagegram.API.Application.Repositories
{
    public interface IPostRepository : IDisposable
    {
        Task<string> CreatePost(byte[] image, string UUID, string comment);
        Task<IEnumerable<dynamic>> GetAllPosts(int limit, int page);
        Task<string> CreateComment(int postid, string comment, string UUID);
        Task<string> DeleteComment(int postid, int commentid);
        Task<IEnumerable<dynamic>> GetComment(int postid, int limit, int page);
    }
}

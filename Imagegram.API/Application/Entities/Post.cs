using Imagegram.API.Application.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Imagegram.API.Application.Entities
{
    public class Post
    {
        public Post()
        {
            Comments = new List<Comment>();
        }
        public int PostId { get; set; }
        public byte[] ImageContent { get; set; }
        public string CreatorUUID { get; set; }
        public DateTime CreatedDate { get; set; }
        public IList<Comment> Comments { get; set; }
    }
}

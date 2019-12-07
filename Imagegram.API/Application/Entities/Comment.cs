using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Imagegram.API.Application.Entities
{
    /// <summary>
    /// Comment Model
    /// </summary>
    public class Comment
    {
        /// <summary>
        /// Comment ID
        /// </summary>
        public int CommentId { get; set; }
        /// <summary>
        /// Content of this Comment
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// Post Id this comment belongs to
        /// </summary>
        public int PostId { get; set; }
        /// <summary>
        /// UUID of creator for this Comment
        /// </summary>
        public string CreatorUUID { get; set; }
        /// <summary>
        /// Creation Date of this Comment
        /// </summary>
        public DateTime CreatedDate { get; set; }
    }
}

using Dapper;
using Imagegram.API.Application.Entities;
using Imagegram.API.Application.Repositories;
using Imagegram.API.Infrastructure.Configurations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Imagegram.API.Infrastructure.Repositories.Dapper
{
    public class PostRepository : IPostRepository
    {
        private readonly ConnectionStrings _connStrings;

        public PostRepository(ConnectionStrings connStrings)
        {
            _connStrings = connStrings;
        }
        public async Task<string> CreatePost(byte[] image, string UUID, string comment)
        {
            string result;
            using (IDbConnection dbConnection = new SqlConnection(_connStrings.ImagegramDB))
            {
                dbConnection.Open();

                var queryParameters = new DynamicParameters();
                queryParameters.Add("@ImageContent", image);
                queryParameters.Add("@Comment", comment);
                queryParameters.Add("@UUID", UUID);

                result = await dbConnection.ExecuteScalarAsync<string>("[dbo].[WS_CreatePost]", queryParameters, commandType: CommandType.StoredProcedure);
            }
            return result;
        }

        public async Task<IEnumerable<dynamic>> GetAllPosts(int limit, int page)
        {
            IEnumerable<dynamic> result;
            using (IDbConnection dbConnection = new SqlConnection(_connStrings.ImagegramDB))
            {
                dbConnection.Open();

                var queryParameters = new DynamicParameters();
                queryParameters.Add("@Page", page);
                queryParameters.Add("@Limit", limit);
                var lookup = new Dictionary<int, Post>();
                result = await dbConnection.QueryAsync<Post, Comment, Post>("[dbo].[WS_GetAllPost]",
                    (possibleDupeCategory, widget) =>
                    {
                        Post category;

                        // Look for the current category, storing it in `category` if it
                        // exists.
                        if (!lookup.TryGetValue(possibleDupeCategory.PostId, out category))
                        {
                            // If the lookup doesn't contain the current category, add it
                            // and store it in `category` as well.
                            lookup.Add(possibleDupeCategory.PostId, possibleDupeCategory);

                            category = possibleDupeCategory;
                        }

                        // Regardless of the state of the lookup before this mapping,
                        // `category` now refers to a distinct category.

                        category.Comments.Add(widget);
                        //widget.Category = category;

                        return category;
                    }
                    , queryParameters, splitOn: "CommentId", commandType: CommandType.StoredProcedure);

            }
            return result.Distinct();
        }

        public async Task<string> CreateComment(int postid, string comment, string UUID)
        {
            string result;
            using (IDbConnection dbConnection = new SqlConnection(_connStrings.ImagegramDB))
            {
                dbConnection.Open();

                var queryParameters = new DynamicParameters();
                queryParameters.Add("@PostID", postid);
                queryParameters.Add("@Comment", comment);
                queryParameters.Add("@UUID", UUID);

                result = await dbConnection.ExecuteScalarAsync<string>("[dbo].[WS_CreateComment]", queryParameters, commandType: CommandType.StoredProcedure);
            }
            return result;
        }

        public async Task<string> DeleteComment(int postid, int commentid)
        {
            string result;
            using (IDbConnection dbConnection = new SqlConnection(_connStrings.ImagegramDB))
            {
                dbConnection.Open();

                var queryParameters = new DynamicParameters();
                queryParameters.Add("@PostID", postid);
                queryParameters.Add("@CommentID", commentid);
                //queryParameters.Add("@UUID", UUID);

                result = await dbConnection.ExecuteScalarAsync<string>("[dbo].[WS_DeleteComment]", queryParameters, commandType: CommandType.StoredProcedure);
            }
            return result;
        }

        public async Task<IEnumerable<dynamic>> GetComment(int postid, int limit, int page)
        {
            IEnumerable<dynamic> result;
            using (IDbConnection dbConnection = new SqlConnection(_connStrings.ImagegramDB))
            {
                dbConnection.Open();

                var queryParameters = new DynamicParameters();
                queryParameters.Add("@PostID", postid);
                queryParameters.Add("@Limit", limit);
                queryParameters.Add("@Page", page);
                //queryParameters.Add("@UUID", UUID);

                result = await dbConnection.QueryAsync<dynamic>("[dbo].[WS_GetComment]", queryParameters, commandType: CommandType.StoredProcedure);
            }
            return result;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~PostRepository()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}

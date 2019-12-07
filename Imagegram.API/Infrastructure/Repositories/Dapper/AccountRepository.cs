using Dapper;
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
    public class AccountRepository : IAccountRepository
    {
        private readonly ConnectionStrings _connStrings;
       // private readonly IRequestHeaders _requestHeaders;
        private readonly string UUID;

        public AccountRepository(ConnectionStrings connStrings)//, IRequestHeaders requestHeaders)
        {
            _connStrings = connStrings;
            //_requestHeaders = requestHeaders;

            //if (!string.IsNullOrWhiteSpace(_requestHeaders.UUID))
            //{
            //    UUID = _requestHeaders.UUID;
            //}
            //SqlMapper.Settings.CommandTimeout = _appSettings.CommandTimeout;
        }

        public async Task<string> CreateAccount(string name, string UUID)
        {
            string result;
            using (IDbConnection dbConnection = new SqlConnection(_connStrings.ImagegramDB))
            {
                dbConnection.Open();

                var queryParameters = new DynamicParameters();
                queryParameters.Add("@Name", name);
                queryParameters.Add("@UUID", UUID);

                result = await dbConnection.ExecuteScalarAsync<string>("[dbo].[WS_CreateAccount]", queryParameters, commandType: CommandType.StoredProcedure);
            }
            return result;
        }

        public async Task<string> DeleteAccount(string UUID)
        {
            string result;
            using (IDbConnection dbConnection = new SqlConnection(_connStrings.ImagegramDB))
            {
                dbConnection.Open();

                var queryParameters = new DynamicParameters();
                queryParameters.Add("@UUID", UUID);

                result = await dbConnection.ExecuteScalarAsync<string>("[dbo].[WS_DeleteAccount]", queryParameters, commandType: CommandType.StoredProcedure);
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

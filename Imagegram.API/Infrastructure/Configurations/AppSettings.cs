using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Imagegram.API.Infrastructure.Configurations
{
    public class AppSettings
    {
        public string SiteUrl { get; set; }
        public int CommandTimeout { get; set; }
        public string RepositoryInterfaceNS { get; set; }
        public string RepositoryImplementationNS { get; set; }
    }
}

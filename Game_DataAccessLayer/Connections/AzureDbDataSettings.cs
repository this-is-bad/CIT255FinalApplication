using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_DataAccessLayer
{   
    /// <summary>
    /// Contains the connection information for an Azure database
    /// </summary>
    public static class AzureDbDataSettings
    {

        public const string connectionString = "Server=tcp:cit255.database.windows.net,1433;Initial Catalog=Gamiverse;Persist Security Info=False;User ID=user;Password=Password1;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        
    }
}

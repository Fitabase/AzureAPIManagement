using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitabase.Azure.ApiManagement
{
    public class SwaggerConfiguration
    {
        
        private string ApiKey { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public SwaggerConfiguration(string apiKey, string username, string password)
        {
            this.ApiKey = apiKey;
            this.Username = username;
            this.Password = password;
        }

      
    
    }
}

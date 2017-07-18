using Fitabase.Azure.ApiManagement.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Azure.ApiManagement.Test
{

    [TestClass]
    public class ModelErrorTest
    {
        
        [TestMethod]
        [ExpectedException(typeof(InvalidEntityException))]
        public void CreateAPI_error()
        {
            string name = null;
            string description = "This is a calculator created in the server";
            string serviceUrl = "http://echoapi.cloudapp.net/calc";
            string path = "/add/calc";
            string[] protocols = new string[] { "http", "https" };
            API newAPI = API.Create(name, description, serviceUrl, path, protocols);
        }
    }
}

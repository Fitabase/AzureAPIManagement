using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmallStepsLabs.Azure.ApiManagement;
using SmallStepsLabs.Azure.ApiManagement.Model;

namespace Azure.ApiManagement.Test
{
    [TestClass]
    public class ManagementClientTests
    {
        protected ManagementClient Client { get; set; }

        [TestInitialize]
        public void Setup()
        {
            var host = "devav";
            var serviceId = "56ca4cd99e1436035c030003";
            var accessKey = "NaS+Pv8qOESQ3H4HvnVz3JEQRVq16sVCgnkW+7ldqaT8cIqcKfFe089bSZUnhyHhVu1BXXEz0udjHHEh1w6JBw==";

            this.Client = new ManagementClient(host, serviceId, accessKey);
        }

        #region Arguments Validation

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void InvalidArgumentGetProduct()
        {
            Client.GetProductAsync(null).Wait();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InvalidArgumentCreateProduct()
        {
            Client.CreateProductAsync(null).Wait();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void InvalidArgumentIdCreateProduct()
        {
            Client.CreateProductAsync(new Product()).Wait();
        }

        #endregion

        [TestMethod]
        public void GetProducts()
        {
            var task = Client.GetProductsAsync();
            task.Wait();
        }
    }
}

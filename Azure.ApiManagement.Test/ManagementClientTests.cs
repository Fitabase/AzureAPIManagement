using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmallStepsLabs.Azure.ApiManagement;
using SmallStepsLabs.Azure.ApiManagement.Model;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
        [ExpectedException(typeof(OperationCanceledException))]
        public void CancelRequest()
        {
            var cts = new CancellationTokenSource();
            var task = Client.GetProductsAsync(cancellationToken: cts.Token);

            cts.CancelAfter(250);
            task.Wait(cts.Token);
        }

        [TestMethod]
        public void GetProducts()
        {
            var task = Client.GetProductsAsync();
            task.Wait();

            Assert.AreNotEqual(null, task.Result);
            Assert.AreNotEqual(null, task.Result.Values);
        }

        [TestMethod]
        public void GetProductsExpandGroups()
        {
            var task = Client.GetProductsAsync(expandGroups: true);
            task.Wait();

            Assert.AreEqual(true, task.Result.Values.Any(p => p.Groups != null));
        }

        [TestMethod]
        public void GetProduct()
        {
            var task = Client.GetProductsAsync();
            task.Wait();

            var productId = task.Result.Values[0].Id;

            var pTask = Client.GetProductAsync(productId);

            Assert.AreNotEqual(null, pTask.Result);
            Assert.AreNotEqual(productId, pTask.Id);
        }


    }
}

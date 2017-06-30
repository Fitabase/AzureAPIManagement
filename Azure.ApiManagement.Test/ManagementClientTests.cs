﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Fitabase.Azure.ApiManagement;
using Fitabase.Azure.ApiManagement.Model;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Azure.ApiManagement.Test
{
    [TestClass]
    public class ManagementClientTests
    {
        protected ManagementClient Client { get; set; }

        [TestInitialize]
        public void Setup()
        {
            string apiKeysContent;
            var host = "";
            var serviceId = "";
            var accessKey = "";

            try
            {
                //provide to reader your complete text file
                using (StreamReader sr = new StreamReader(".\\APIMKeys.json")) //make sure this file has "Copy to output directory" Set to "Copy Always"
                {
                    apiKeysContent = sr.ReadToEnd();
                    //Console.WriteLine(line);
                    var json = JObject.Parse(apiKeysContent);
                    host = json["host"].ToString();
                    serviceId = json["serviceId"].ToString();
                    accessKey = json["accessKey"].ToString();

                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }

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
        }

        [TestMethod]
        public void GetProductsExpandGroups()
        {
            var task = Client.GetProductsAsync(expandGroups: true);
            task.Wait();

            Assert.AreEqual(true, task.Result.Any(p => p.Groups != null));
        }

        [TestMethod]
        public void GetProduct()
        {
            var task = Client.GetProductsAsync();
            task.Wait();

            var productId = task.Result[0].Id;

            var pTask = Client.GetProductAsync(productId);

            Assert.IsNotNull(pTask.Result);
            Assert.AreNotEqual(productId, pTask.Id);
        }

        [TestMethod]
        public void GetProductByFilter()
        {
            var task = Client.GetProductsAsync();
            task.Wait();

            var productId = task.Result.Last().Id;

            var filter = String.Format("id eq '{0}'", productId);
            var pTask = Client.GetProductsAsync(filter);
            pTask.Wait();

            Assert.IsTrue(pTask.Result.Count == 1);
            Assert.AreEqual(productId, pTask.Result.First().Id);
        }

        [TestMethod]
        public void UpdateProduct()
        {
            var task = Client.GetProductsAsync();
            task.Wait();

            var product = task.Result.First();

            var testDesc = DateTime.Now.ToLongTimeString();

            product.Description = testDesc;

            Client.UpdateProductAsync(product).Wait();

            var pickTask = Client.GetProductAsync(product.Id);
            pickTask.Wait();

            Assert.AreEqual(testDesc, pickTask.Result.Description);
        }

        [TestMethod]
        public void CreateProduct()
        {
            var id = Guid.NewGuid().ToString("N");
            var newProduct = new Product()
            {
                Id = id,
                Name = "Testing product " + id,
                Description = "Testing"
            };

            Client.CreateProductAsync(newProduct).Wait();

            var pickTask = Client.GetProductAsync(newProduct.Id);
            pickTask.Wait();

            Assert.AreEqual(newProduct.Id, pickTask.Result.Id);
        }

        [TestMethod]
        [ExpectedException(typeof(HttpResponseException))]
        public void DeleteProduct()
        {
            var id = Guid.NewGuid().ToString("N");
            var newProduct = new Product()
            {
                Id = id,
                Name = "Testing product " + id,
                Description = "Testing"
            };

            Client.CreateProductAsync(newProduct).Wait();
            Client.DeleteProductAsync(newProduct.Id, true).Wait();

            try
            {

                Client.GetProductAsync(newProduct.Id).Wait();
            }
            catch (AggregateException ex)
            {
                throw ex.InnerException;
            }
        }


        [TestMethod]
        public void GetProductAPIs()
        {
            var task = Client.GetProductsAsync();
            task.Wait();

            var productId = task.Result[0].Id;

            var pTask = Client.GetProductAPIsAsync(productId);
            pTask.Wait();

            Assert.IsNotNull(pTask.Result);
        }

        [TestMethod]
        public void GetProductAddRemoveAPIs()
        {
            var task = Client.GetProductsAsync();
            task.Wait();

            var productId = task.Result[0].Id;

            var pTask = Client.GetProductAPIsAsync(productId);
            pTask.Wait();

            if (pTask.Result.Count > 0)
            {
                var apiId = pTask.Result[0].Id;

                Client.RemoveProductAPIAsync(productId, apiId).Wait();
                Client.AddProductAPIAsync(productId, apiId).Wait();

                // assert
                pTask = Client.GetProductAPIsAsync(productId);
                pTask.Wait();

                Assert.IsNotNull(pTask.Result.First(apis => apis.Id == apiId));
            }

            else Assert.Inconclusive("Selected Product didn`t have assigned APIs");
        }

        [TestMethod]
        public void GetAPIs()
        {
            var task = Client.GetAPIsAsync();
            task.Wait();

            Assert.IsNotNull(task.Result);
        }

        [TestMethod]
        public void GetAPI()
        {
            var task = Client.GetAPIsAsync();
            task.Wait();

            if (task.Result.Count > 0)
            {
                var apiId = task.Result.First().Id;

                var aTask = Client.GetAPIAsync(apiId);
                aTask.Wait();

                Assert.IsNotNull(aTask.Result);
                Assert.AreEqual(apiId, aTask.Result.Id);
            }

            else Assert.Inconclusive("No APIs defined");
        }

        [TestMethod]
        public void CheckProductPolicy()
        {
            var task = Client.GetProductsAsync();
            task.Wait();
            if (task.Result.Count > 0)
            {
                var productId = task.Result[0].Id;

                var pTask = Client.CheckProductPolicyAsync(productId);
                pTask.Wait();

            }

            else Assert.Inconclusive("No Products found.");
        }

        [TestMethod]
        public void GetProductPolicy()
        {
            var task = Client.GetProductsAsync();
            task.Wait();

            if (task.Result.Count > 0)
            {
                var productId = task.Result[0].Id;

                var pTask = Client.GetProductPolicyAsync(productId);
                pTask.Wait();

                Assert.IsNotNull(pTask.Result);

            }

            else Assert.Inconclusive("No Products found.");
        }

        [TestMethod]
        public void SetProductPolicy()
        {
            var task = Client.GetProductsAsync();
            task.Wait();

            if (task.Result.Count > 0)
            {
                var productId = task.Result[0].Id;

                var pTask = Client.GetProductPolicyAsync(productId);
                pTask.Wait();

                var policy = pTask.Result;

                var updateTask = Client.SetProductPolicyAsync(productId, policy);

                Assert.IsNotNull(pTask.Result);

            }

            else Assert.Inconclusive("No Products found.");
        }

        [TestMethod]
        public void CreateUserGetLogin()
        {
            var uid = Guid.NewGuid().ToString("N");
            var newUser = new User()
            {
                Id = uid,
                Email = String.Format("test_{0}@test.com", uid),
                FirstName = "First Name",
                LastName = "Last Name",
                Password = "P@ssWo3d",
                State = UserState.active,
                Note = "notes.."
            };

            Client.CreateUserAsync(newUser).Wait();

            var uTask = Client.GetUserSsoLoginAsync(uid);
            uTask.Wait();

            Assert.IsNotNull(uTask.Result);
            Assert.IsTrue(Uri.IsWellFormedUriString(uTask.Result.Url, UriKind.Absolute));
        }


        [TestMethod]
        public void AddRemoveUserSubscription()
        {
            var uid = Guid.NewGuid().ToString("N");
            var newUser = new User()
            {
                Id = uid,
                Email = String.Format("test_{0}@test.com", uid),
                FirstName = "First Name",
                LastName = "Last Name",
                Password = "P@ssWo3d",
                State = UserState.active,
                Note = "notes.."
            };

            Client.CreateUserAsync(newUser).Wait();

            var ptask = Client.GetProductsAsync();
            ptask.Wait();

            uid = "/users/" + uid;
            var pid = ptask.Result.First(p => p.State == ProductState.published).Uri; // only state which can be used in subscriptions

            var sid = Guid.NewGuid().ToString("N");
            var newsubsc = new Subscription()
            {
                Id = sid,
                UserId = uid,
                ProductId = pid,
                State = SubscriptionState.active,
            };

            Client.AddProductSubscriptionAsync(newsubsc).Wait();
            Client.RemoveProductSubscriptionAsync(newsubsc.Id).Wait();
        }
    }
}

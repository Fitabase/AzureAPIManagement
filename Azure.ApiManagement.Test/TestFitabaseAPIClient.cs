using Microsoft.VisualStudio.TestTools.UnitTesting;
using Fitabase.Azure.ApiManagement;
using Fitabase.Azure.ApiManagement.Model;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Azure.ApiManagement.Test
{
    [TestClass]
    public class TestFitabaseAPIClient
    {
        protected FitabaseApiClient Client;

        [TestInitialize]
        public void SetUp()
        {
            Client = new FitabaseApiClient();
        }

        #region TEST_USER

        [TestMethod]
        public void CreateUser()
        {
            var uid = Guid.NewGuid().ToString("N");
            var newUser = new User()
            {
                Id = uid,
                Email = String.Format("test_{0}@test.com", uid),
                FirstName = "Derek100",
                LastName = "Nguyen100",
                Password = "P@ssWo3d",
                State = UserState.active,
                Note = "notes.."
            };
            Client.CreateUser(uid, newUser);
        }

        [TestMethod]
        public void GetUserCollection()
        {
            var userCollection = Client.AllUsers();
            Assert.IsNotNull(userCollection);
            PrintMessage.Debug(this.GetType().Name, userCollection.Count);
            PrintMessage.Debug(this.GetType().Name, "Total Apis: " + userCollection.Items.Count);
            foreach (var data in userCollection.Items)
            {
                System.Diagnostics.Debug.WriteLine("user_id: " + data.Id);
            }
        }

        [TestMethod]
        public void GetUser()
        {
            string userId = "66da331f7a1c49d98ac8a4ad136c7c64";
            var user = Client.GetUser(userId);
            Assert.IsNotNull(user);
        }

        #endregion



        #region TEST_API
        [TestMethod]
        public void CreateApi()
        {

            var id = Guid.NewGuid().ToString("N");


            var newAPI = new API();
            newAPI.Id = id;
            newAPI.Name = "serverAPI";
            newAPI.Description = "create api from server";
            newAPI.ServiceUrl = "http://echoapi.cloudapp.net/calc";
            newAPI.Path = "GoodCall";
            newAPI.Protocols = new List<String>() { "http", "https" };
            newAPI.Authentication = null;
            newAPI.CustomNames = null;

            Client.CreateAPI(id, newAPI);
        }

        [TestMethod]
        public void GetApi()
        {
            string apiId = "5956a87a2f02d30b88dfad7b";
            var api = Client.GetAPI(apiId);

            Assert.IsNotNull(api);
            PrintMessage.Debug(this.GetType().Name, api);
        }


        [TestMethod]
        public void ApiCollection()
        {
            var apis = Client.AllAPIs();
            Assert.IsNotNull(apis);
            PrintMessage.Debug(this.GetType().Name, "Total Apis: " + apis.Items.Count);
            foreach (var data in apis.Items)
            {
                PrintMessage.Debug(this.GetType().Name, "API_id: " + data.Id);
            }
        }
        #endregion



        #region TEST_PRODUCT
        [TestMethod]
        public void CreateProduct()
        {
            var id = "";
            var product = new Product("Server product");
            //product.Id = id;

            Client.CreateProduct(id, product);

        }

        [TestMethod]
        public void GetProduct()
        {
            var productId = "29f79d2acfab453eac057ddf3656a31b";
            var product = Client.GetProduct(productId);

            Assert.IsNotNull(product);
            Assert.AreEqual(productId, product.Id);

            PrintMessage.Debug(this.GetType().Name, product);

        }

        [TestMethod]
        public void UpdateProduct()
        {
            var productId = "29f79d2acfab453eac057ddf3656a31b";
            Hashtable parameters = new Hashtable();
            parameters.Add("name", "new product name");
            var product = Client.UpdateProduct(productId, parameters);

            Assert.IsNotNull(product);
            Assert.AreEqual(productId, product.Id);

            PrintMessage.Debug(this.GetType().Name, product);

        }

        [TestMethod]
        public void ProductCollection()
        {
            var products = Client.AllProducts();
            foreach (var product in products.Items)
            {
                PrintMessage.Debug(this.GetType().Name, product.Name);
            }
        }
        #endregion




        #region Test_Subscription


        [TestMethod]
        public void SubscriptionCollection()
        {
            var collection = Client.AllSubscriptions();
            foreach (var c in collection.Items)
            {
                PrintMessage.Debug(this.GetType().Name, c.Id);
            }
        }
        #endregion


        #region Test_Group


        [TestMethod]
        public void GroupCollection()
        {
            var collection = Client.AllGroups();
            foreach (var c in collection.Items)
            {
                PrintMessage.Debug(this.GetType().Name, c.Id);
            }
        }
        #endregion


    }
}

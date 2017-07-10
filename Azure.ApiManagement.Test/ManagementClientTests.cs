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
    public class Testuser
    {
        protected ManagementClient Client { get; set; }
        

        [TestInitialize]
        public void SetUp()
        {
            Client = new ManagementClient();
        }



        #region User TestCases

        [TestMethod]
        public void CreateUser()
        {
            var preCount = Client.AllUsers().Count;
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
            var user = Client.CreateUser(uid, newUser);
            var postCount = Client.AllUsers().Count;
            Assert.IsNotNull(newUser);
            Assert.AreEqual(preCount + 1, postCount);
        }

        [TestMethod]
        public void GetUserCollection()
        {
            var userCollection = Client.AllUsers();
            Assert.IsNotNull(userCollection);
            Assert.AreEqual(userCollection.Count, userCollection.Values.Count);
        }

        [TestMethod]
        public void GetUser()
        {
            string userId = "66da331f7a1c49d98ac8a4ad136c7c64";
            var user = Client.GetUser(userId);
            Assert.IsNotNull(user);
            Assert.AreEqual(userId, user.Id);
        }

        [TestMethod]
        public void GetUserSsoURL()
        {
            string userId = "66da331f7a1c49d98ac8a4ad136c7c64";
            var user = Client.GenerateSsoURL(userId);
            Assert.IsNotNull(user);
            Assert.IsNotNull(user.Url);
        }

        [TestMethod]
        public void UpdateUser()
        {
            var userId = "66da331f7a1c49d98ac8a4ad136c7c64";
            var firstName = "omg";

            Hashtable parameters = new Hashtable();
            parameters.Add("firstName", firstName);

            Client.UpdateUser(userId, parameters);

            var currUser = Client.GetUser(userId);
            Assert.AreEqual(currUser.FirstName, firstName);
        }

       

        #endregion User TestCases

        #region API TestCases

        [TestMethod]
        public void CreateApi()
        {

            var id = Guid.NewGuid().ToString("N");


            var newAPI = new API();
            newAPI.Id = id;
            newAPI.Name = "Server Calculator";
            newAPI.Description = "This is a calculator created in the server";
            newAPI.ServiceUrl = "http://echoapi.cloudapp.net/calc";
            newAPI.Path = "/derek/calc";
            newAPI.Protocols = new List<String>() { "http", "https" };
            newAPI.Authentication = null;
            newAPI.CustomNames = null;

             var api = Client.CreateAPI(id, newAPI);

        }

        [TestMethod]
        public void GetApi()
        {
            string apiId = "65d17612d5074d8bbfde4026357a24da";
            var api = Client.GetAPI(apiId);

            Assert.IsNotNull(api);
            Assert.AreEqual(api.Id, apiId);
        }


        [TestMethod]
        public void ApiCollection()
        {
            var apis = Client.AllAPIs();
            Assert.IsNotNull(apis);
        }

        #endregion APITestCases

        #region API OperationsTestCases
        [TestMethod]
        public void CreateAPIOperation()
        {
            var apiId = "65d17612d5074d8bbfde4026357a24da";
            var operationId = Guid.NewGuid().ToString("N");
            var name = "Onemore API operation";
            var method = "POST";
            var urlTemplate = "/add/{c}/{d}";
            var parameters = new List<TemplateParameter>()
            {
                new TemplateParameter("c", ParameterType.NUMBER),
                new TemplateParameter("d", ParameterType.NUMBER)
            };


            RequestContract request = new RequestContract();

            request.Headers = new List<RequestHeader>() {
                                            new RequestHeader("Ocp-Apim-Subscription-Key", ParameterType.STRING)
                                        };
            request.QueryParameters = new List<QueryParameter>() {
                                           new QueryParameter("queryParameter1", ParameterType.NUMBER)
                                        };

            var operation = new APIOperation(operationId, name, method, urlTemplate, parameters, request);

            var entity = Client.CreateAPIOperation(apiId, operationId, operation);
        }



        [TestMethod]
        public void GetAPIOperationByAPI()
        {
            var apiId = "65d17612d5074d8bbfde4026357a24da";
            EntityCollection<APIOperation> collection = Client.GetByAPI(apiId);
            Assert.IsNotNull(collection);
        }


        [TestMethod]
        public void DelteAPIOperation()
        {
            var apiId = "65d17612d5074d8bbfde4026357a24da";
            var operationId = "d6be400efb924ea18c615cdcc486d278";
            var precount = Client.GetByAPI(apiId).Count;

            APIOperation operation = Client.DeleteOperation(apiId, operationId);
            var postCount = Client.GetByAPI(apiId).Count;
            Assert.IsNotNull(operation);
            Assert.AreEqual(precount - 1, postCount);
        }

        #endregion API OperationsTestCases

        #region ProductTestCases

        [TestMethod]
        public void CreateProduct()
        {

            var pid = Guid.NewGuid().ToString("N");
            var product = new Product(pid, "Server producta", "server description");
            var p = Client.CreateProduct(pid, product);

            Assert.IsNotNull(p);
            Assert.IsNotNull(p.Id);
            Assert.AreEqual(p.Id, pid);
        }

        [TestMethod]
        public void GetProduct()
        {
            var productId = "29f79d2acfab453eac057ddf3656a31b";
            var product = Client.GetProduct(productId);

            Assert.IsNotNull(product);
            Assert.AreEqual(productId, product.Id);
        }

        [TestMethod]
        public void UpdateProduct()
        {
            var productId = "5956a9b92f02d30b88dfad7d";
            var currProduct = Client.GetProduct(productId);
            var desc = "much better description";
            currProduct.Description = desc;
            Client.UpdateProduct(currProduct);
            var product = Client.GetProduct(productId);

            Assert.IsNotNull(product);
            Assert.AreEqual(productId, product.Id);
            Assert.AreEqual(product.Description, desc);


        }

        [TestMethod]
        public void DeletProduct()
        {
            var productId = "Fitabase.Azure.ApiManagement.Model.Product";
            var collection = Client.AllProducts();
            Client.DeleteProduct(productId);
            var currCollection = Client.AllProducts();

            Assert.AreEqual(collection.Count - 1, currCollection.Count);

        }

        [TestMethod]
        public void ProductCollection()
        {
            var products = Client.AllProducts();
            Assert.IsNotNull(products);
        }


        [TestMethod]
        public void GetProductSubscriptions()
        {
            var productId = "5956a9b92f02d30b88dfad7d";
            var collection = Client.GetProductSubscriptions(productId);
            Assert.IsNotNull(collection);
        }


        [TestMethod]
        public void GetProductAPIs()
        {
            var productId = "5956a9b92f02d30b88dfad7d";
            var collection = Client.GetProductAPIs(productId);
            Assert.IsNotNull(collection);
        }
        [TestMethod]
        public void AddProductApi()
        {
            var productId = "5956a9b92f02d30b88dfad7d";
            var apiId = "65d17612d5074d8bbfde4026357a24da";
            var preCount = Client.GetProductAPIs(productId).Count;
            Client.AddProductAPI(productId, apiId);
            var postCount = Client.GetProductAPIs(productId).Count;

            Assert.AreEqual(preCount + 1, postCount);
        }
        [TestMethod]
        public void DeleteProductApi()
        {
            var productId = "5956a9b92f02d30b88dfad7d";
            var apiId = "65d17612d5074d8bbfde4026357a24da";
            var preCount = Client.GetProductAPIs(productId).Count;
            Client.DeleteProductAPI(productId, apiId);
            var postCount = Client.GetProductAPIs(productId).Count;

            Assert.AreEqual(preCount - 1, postCount);
        }


        [TestMethod]
        public void GetProductGroups()
        {
            var productId = "5956a9b92f02d30b88dfad7d";
            var collection = Client.GetProductGroups(productId);
            Assert.IsNotNull(collection);
        }
        [TestMethod]
        public void AddProductGroup()
        {
            var productId = "5956a9b92f02d30b88dfad7d";
            var groupId = "5870184f9898000087020002";
            var preCount = Client.GetProductGroups(productId).Count;
            Client.AddProductGroup(productId, groupId);
            var postCount = Client.GetProductGroups(productId).Count;
            Assert.AreEqual(preCount + 1, postCount);
        }
        [TestMethod]
        public void DeleteProductGroup()
        {
            var productId = "5956a9b92f02d30b88dfad7d";
            var groupId = "5870184f9898000087020002";
            var preCount = Client.GetProductGroups(productId).Count;
            Client.DeleteProductGroup(productId, groupId);
            var postCount = Client.GetProductGroups(productId).Count;
            Assert.AreEqual(preCount - 1, postCount);
        }


        #endregion ProductTestCases

        #region Subscription TestCases


        [TestMethod]
        public void SubscriptionCollection()
        {
            var collection = Client.AllSubscriptions();
            Assert.IsNotNull(collection);
        }

        [TestMethod]
        public void GetSubscription()
        {
            var subscriptionId = "5870184f9898000087070001";
            var subscription = Client.GetSubscription(subscriptionId);
            Assert.IsNotNull(subscription);
            Assert.AreEqual(subscriptionId, subscription.Id);
        }


        [TestMethod]
        public void GeneratePrimaryKey()
        {
            var subscriptionId = "5870184f9898000087070001";
            var key = Client.GeneratePrimaryKey(subscriptionId);
        }

        [TestMethod]
        public void GenerateSecondaryKey()
        {
            var subscriptionId = "5870184f9898000087070001";
            Client.GenerateSecondaryKey(subscriptionId);

        }

        #endregion Subscription TestCases

        #region GroupTestCases


        [TestMethod]
        public void GroupCollection()
        {
            var collection = Client.AllGroups();
            Assert.IsNotNull(collection);
        }

        #endregion GroupTestCases



    }
}

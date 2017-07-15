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
using Fitabase.Azure.ApiManagement.DataModel.Properties;

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
            var firstName = "Derek2";
            var lastName = "Nguyen";
            var email = String.Format("{0}{1}@test.com", firstName, lastName);
            var password = "P@ssWo3d";
            //var newUser = new User(firstName, lastName, email,  password);
            var newUser = new User()
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Password = password
            };

            //PrintMessage.Debug(this.GetType().Name, Utility.SerializeToJson(newUser));
            var user = Client.CreateUser(newUser);
            var postCount = Client.AllUsers().Count;
            Assert.AreEqual(preCount + 1, postCount);
            Assert.IsNotNull(user);
            Assert.IsNotNull(user.Id);
        }

        [TestMethod]
        public void GetUserCollection()
        {
            var userCollection = Client.AllUsers();
            PrintMessage.Debug(this.GetType().Name, userCollection);
            PrintMessage.Debug(this.GetType().Name, userCollection.Values.ElementAt(3).Id);
            Assert.IsNotNull(userCollection);
            Assert.AreEqual(userCollection.Count, userCollection.Values.Count);
        }

        [TestMethod]
        public void GetUser()
        {
            string userId = "user_dd1836806e864ba8acea1caaba13268d";
            var user = Client.GetUser(userId);
            PrintMessage.Debug("GetUser", Utility.SerializeToJson(user));
            PrintMessage.Debug("GetUser", Utility.SerializeToJson(user.Id));
            PrintMessage.Debug("GetUser", Utility.SerializeToJson(user));
            Assert.IsNotNull(user);
            //Assert.AreEqual(userId, user.Id);
        }

        [TestMethod]
        public void GetUserSubscriptions()
        {
            var userId = "1";
            var collection = Client.GetUserSubscription(userId);
            PrintMessage.Debug(this.GetType().Name, Utility.SerializeToJson(collection));
        }


        [TestMethod]
        public void DeleteUser()
        {
            var preCount = Client.AllUsers().Count;
            var userId = "";
            var deletedUser = Client.DeleteUser(userId);
            var postCount = Client.AllUsers().Count;
            Assert.AreEqual(preCount - 1, postCount);
        }
        [TestMethod]
        public void DeleteUserWithSubscription()
        {
            var preCount = Client.AllUsers().Count;
            var userId = "";
            var deletedUser = Client.DeleteUser(userId);
            var postCount = Client.AllUsers().Count;
            Assert.AreEqual(preCount - 1, postCount);
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
            var userId = "1";
            var lastName = "Fitabase";

            Hashtable parameters = new Hashtable();
            parameters.Add("lastName", lastName);

            Client.UpdateUser(userId, parameters);

            var currUser = Client.GetUser(userId);
            Assert.AreEqual(currUser.LastName, lastName);
        }

       

        #endregion User TestCases

        #region API TestCases

        [TestMethod]
        public void CreateApi()
        {
            var name = "Server Calculator";
            var description = "This is a calculator created in the server";
            var serviceUrl = "http://echoapi.cloudapp.net/calc";
            var path = "/derek/calc";
            var protocols = new List<String>() { "http", "https" };
            
            var newAPI = new API(name, description, serviceUrl, path, protocols);

            var api = Client.CreateAPI(newAPI);
            PrintMessage.Debug(this.GetType().Name, newAPI);

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
            foreach(API api in apis.Values)
            {
                PrintMessage.Debug(this.GetType().Name, Utility.SerializeToJson(api));
            }
        }

        #endregion APITestCases

        #region API OperationsTestCases
        [TestMethod]
        public void CreateAPIOperation()
        {
            var apiId = "65d17612d5074d8bbfde4026357a24da";
            var operationId = Guid.NewGuid().ToString("N");
            var name = "Onemore API operation";
            var method = RequestMethod.POST;
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

            var operation = new APIOperation(name, method, urlTemplate, parameters, request);

            var entity = Client.CreateAPIOperation(operationId, operation);
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
            var product = new Product("Server producta", "server description");
            var p = Client.CreateProduct(product);

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
            PrintMessage.Debug(this.GetType().Name, Utility.SerializeToJson(collection));
            Assert.IsNotNull(collection);
        }

        [TestMethod]
        public void GetSubscription()
        {
            var subscriptionId = "5870184f9898000087070001";
            var subscription = Client.GetSubscription(subscriptionId);
            PrintMessage.Debug(this.GetType().Name, Utility.SerializeToJson(subscription));
            Assert.IsNotNull(subscription);
            Assert.AreEqual(subscriptionId, subscription.Id);
        }


        [TestMethod]
        public void GeneratePrimaryKey()
        {
            var subscriptionId = "5870184f9898000087070001";
            Client.GeneratePrimaryKey(subscriptionId);
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

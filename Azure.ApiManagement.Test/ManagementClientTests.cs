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
    public class ManagementClientTests
    {
        protected ManagementClient Client { get; set; }
        

        [TestInitialize]
        public void SetUp()
        {
            Client = new ManagementClient();
        }

        private static void Print(object obj)
        {
            PrintMessage.Debug("TestClass", obj);
        }

          
        #region User TestCases

        [TestMethod]
        public void CreateUser()
        {
            var preCount = Client.GetUsers().Count;
            var firstName = "Derek";
            var lastName = "Nguyen";
            var email = String.Format("{0}{1}@test.com", firstName, DateTimeOffset.Now.ToUnixTimeMilliseconds());
            var password = "P@ssWo3d";
            var newUser = User.Create(firstName, lastName, email,  password);
            Client.CreateUser(newUser);
            var postCount = Client.GetUsers().Count;
            Assert.AreEqual(preCount + 1, postCount);
        }

        [TestMethod]
        public void GetUserCollection()
        {
            var userCollection = Client.GetUsers();
            PrintMessage.Debug(this, userCollection);
            Assert.IsNotNull(userCollection);
            Assert.AreEqual(userCollection.Count, userCollection.Values.Count);
        }

        [TestMethod]
        public void GetUser()
        {
            string userId = "66da331f7a1c49d98ac8a4ad136c7c64";
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
            var userId = "66da331f7a1c49d98ac8a4ad136c7c64";
            var collection = Client.GetUserSubscription(userId);
            PrintMessage.Debug(this.GetType().Name, Utility.SerializeToJson(collection));
        }


        [TestMethod]
        public void DeleteUser()
        {
            var preCount = Client.GetUsers().Count;
            var userId = "user__1c83a712efdb41fe8b9ef0687d3e7b17";
            Client.DeleteUser(userId);
            var postCount = Client.GetUsers().Count;
            Assert.AreEqual(preCount - 1, postCount);
        }
        [TestMethod]
        public void DeleteUserWithSubscription()
        {
            var preCount = Client.GetUsers().Count;
            var userId = "";
            Client.DeleteUser(userId);
            var postCount = Client.GetUsers().Count;
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
            var userId = "66da331f7a1c49d98ac8a4ad136c7c64";
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
            var preCount = Client.GetAPIs().Count;
            var name = "add Calculator";
            var description = "This is a calculator created in the server";
            var serviceUrl = "http://echoapi.cloudapp.net/calc";
            var path = "/add/calc";
            var protocols = new string[]{ "http", "https" };
            
            var newAPI = API.Create(name, description, serviceUrl, path, protocols);
            var api = Client.CreateAPI(newAPI);
            
            var postCount = Client.GetAPIs().Count;
            Assert.IsNotNull(api.Id);
            Assert.AreEqual(preCount + 1, postCount);
        }

       

        [TestMethod]
        public void GetApi()
        {
            string apiId = "api_e64e0c2082124df581b2fba70c4de904";
            var api = Client.GetAPI(apiId);
            Print(api);
            Assert.IsNotNull(api);
            Assert.AreEqual(api.Id, apiId);
        }


        [TestMethod]
        public void ApiCollection()
        {
            var apis = Client.GetAPIs();
            Assert.IsNotNull(apis);
            Assert.IsTrue(apis.Count > 0);
        }

        [TestMethod]
        public void DeleteAPI()
        {
            var preCount = Client.GetAPIs().Count;
            string apiId = "api_1342cede25b54c2e88a1ba34b26191f9";
            Client.DeleteAPI(apiId);
            var postCount = Client.GetAPIs().Count;
            Assert.AreEqual(preCount - 1, postCount);
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

            var operation = APIOperation.Create(name, method, urlTemplate, parameters, request);

            Client.CreateAPIOperation(operationId, operation);
        }
        [TestMethod]
        public void GetAPIOperationByAPI()
        {
            var apiId = "5956a87a2f02d30b88dfad7b";
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




        #region Product TestCases

        [TestMethod]
        public void CreateProduct()
        {
            var preCount = Client.GetProducts().Count;
            var pid = Guid.NewGuid().ToString("N");
            var product = Product.Create("ServerProduct1", "server description");
            Client.CreateProduct(product);
            var postCount = Client.GetProducts().Count;
            Assert.AreEqual(preCount + 1, postCount);
        }

        [TestMethod]
        public void GetProduct()
        {
            var productId = "product_735904e546284cab8d47f0ea0c58a16e";
            var product = Client.GetProduct(productId);
            Print(product);
            Assert.IsNotNull(product);
            Assert.AreEqual(productId, product.Id);
        }

        [TestMethod]
        public void UpdateProduct()
        {
            var productId = "product_735904e546284cab8d47f0ea0c58a16e";
            var currProduct = Client.GetProduct(productId);
            var name = "other name";
            currProduct.Name = name;
            Client.UpdateProduct(currProduct);
            var product = Client.GetProduct(productId);

            Assert.IsNotNull(product);
            Assert.AreEqual(productId, product.Id);
            Assert.AreEqual(product.Name, name);


        }

        [TestMethod]
        public void DeletProduct()
        {
            var productId = "product_9ddcf499a9e540eab24bf6f3323d956b";
            var collection = Client.GetProducts();
            Client.DeleteProduct(productId);
            var currCollection = Client.GetProducts();

            Assert.AreEqual(collection.Count - 1, currCollection.Count);

        }

        [TestMethod]
        public void ProductCollection()
        {
            var products = Client.GetProducts();
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
            var apiId = "5956a87a2f02d30b88dfad7b";
            var preCount = Client.GetProductAPIs(productId).Count;
            Client.AddProductAPI(productId, apiId);
            var postCount = Client.GetProductAPIs(productId).Count;

            Assert.AreEqual(preCount + 1, postCount);
        }
        [TestMethod]
        public void DeleteProductApi()
        {
            var productId = "5956a9b92f02d30b88dfad7d";
            var apiId = "5956a87a2f02d30b88dfad7b";
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
            var groupId = "5870184f9898000087020001";
            var preCount = Client.GetProductGroups(productId).Count;
            Client.AddProductGroup(productId, groupId);
            var postCount = Client.GetProductGroups(productId).Count;
            Assert.AreEqual(preCount + 1, postCount);
        }
        [TestMethod]
        public void DeleteProductGroup()
        {
            var productId = "5956a9b92f02d30b88dfad7d";
            var groupId = "5870184f9898000087020001";
            var preCount = Client.GetProductGroups(productId).Count;
            Client.DeleteProductGroup(productId, groupId);
            var postCount = Client.GetProductGroups(productId).Count;
            Assert.AreEqual(preCount - 1, postCount);
        }


        #endregion Product TestCases




        #region Subscription TestCases


        [TestMethod]
        public void GetSubscriptionCollection()
        {
            var collection = Client.GetSubscriptions();
            Print(collection);
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

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

        public static string API_DOC = @"C:\Users\inter\Desktop\FitabaseAPI\Azure\api.json";
        public static string API_OPERATION_DOC = @"C:\Users\inter\Desktop\FitabaseAPI\Azure\apioperation.json";



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
            Print(userCollection);
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
        public void GetUserGroup()
        {
            var userId = "user_bef163ba98af433c917914dd4c208115";
            var collection = Client.GetUserGroups(userId);
            Print(collection);
            Assert.IsNotNull(collection);
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
            string apiId = "api_05247cffcf9d4817adc81663625c18a1";
            var api = Client.GetAPI(apiId);
            Documents.Write(API_DOC, api);
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
            string apiId = "api_05247cffcf9d4817adc81663625c18a1";
            Client.DeleteAPI(apiId);
            var postCount = Client.GetAPIs().Count;
            Assert.AreEqual(preCount - 1, postCount);
    }
        #endregion APITestCases





        #region API OperationsTestCases

        [TestMethod]
        public void CreateAPIOperation()
        {
            var apiId = "api_f98e6f1c4f674a35888aa1e8979e331e";
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

            Client.CreateAPIOperation(apiId, operation);
        }

        [TestMethod]
        public void GetAPIOperation()
        {
            var apiId = "5956a87a2f02d30b88dfad7b";
            var operationId = "5956a9612f02d30b88dfad7c";
            var operation = Client.GetAPIOperation(apiId, operationId);
            Print(operation);
            Assert.IsNotNull(operation);
        }

        [TestMethod]
        public void GetOperationsByAPI()
        {
            var apiId = "api_f98e6f1c4f674a35888aa1e8979e331e";
            EntityCollection<APIOperation> collection = Client.GetOperationsByAPI(apiId);
            Print(collection);
            Assert.IsNotNull(collection);
        }
        [TestMethod]

        public void DelteAPIOperation()
        {
            var apiId = "65d17612d5074d8bbfde4026357a24da";
            var operationId = "d6be400efb924ea18c615cdcc486d278";
            var precount = Client.GetOperationsByAPI(apiId).Count;

            APIOperation operation = Client.DeleteOperation(apiId, operationId);
            var postCount = Client.GetOperationsByAPI(apiId).Count;
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
            var product = Product.Create("Server product", "This product is created from the server");
            Product entity = Client.CreateProduct(product);
            Assert.IsNotNull(entity);
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
        public void CreateSubscription()
        {
            var c1 = Client.GetSubscriptions().Count;
            string userId = "user_bef163ba98af433c917914dd4c208115";
            string productId = "5870184f9898000087060002";
            string name = "server subscriptions";
            var now = DateTime.Now;
            SubscriptionDateSettings dateSettings = new SubscriptionDateSettings(now.AddDays(1), now.AddMonths(2));
            Subscription subscription = Subscription.Create(userId, productId, name, dateSettings, SubscriptionState.active);
            Subscription entity = Client.CreateSubscription(subscription);
            Assert.IsNotNull(entity);
            var c2 = Client.GetSubscriptions().Count;
            Assert.AreEqual(c1 + 1, c2);
        }

        [TestMethod]
        public void DeleteSubscription()
        {
            var c1 = Client.GetSubscriptions().Count;
            var subscriptionId = "subscription_c0ddc8fd75934e1eb2325ff507908140";
            Client.DeleteSubscription(subscriptionId);
            var c2 = Client.GetSubscriptions().Count;
            Assert.AreEqual(c1 - 1, c2);
        }

        [TestMethod]
        public void UpdateSubscription()
        {
            var subscriptionId  = "subscription_72208da5700b45e8a016605ccdc78aa1";
            var entity_v1 = Client.GetSubscription(subscriptionId);

            UpdateSubscription updateSubscription = new UpdateSubscription(subscriptionId);
            DateTime now = DateTime.Now;
            updateSubscription.ExpirationDate = now.AddMonths(20);
            Client.UpdateSubscription(subscriptionId, updateSubscription);
            var entity_v2 = Client.GetSubscription(subscriptionId);
            
            Assert.AreNotEqual(entity_v1.ExpirationDate, entity_v2.ExpirationDate);
        }

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
            var subscriptionId = "subscription_801c5e5a9a9d4e9fba47b1561d1e19f6";
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
        public void CreateGroup()
        {
            var preCount = Client.GetGroups().Count;
            var name = "server group 3";
            var description = "this group is created from server";
            Group group = Group.Create(name, description, GroupType.system);
            var entity = Client.CreateGroup(group);
            Assert.IsNotNull(entity);
            var postCount = Client.GetGroups().Count;
            Assert.AreEqual(preCount + 1, postCount);
        }

        [TestMethod]
        public void GetGroup()
        {
            var groupId = "group_7ac29362a8c743aaa798b331cc87919e";
            var group = Client.GetGroup(groupId);
            Assert.IsNotNull(group);
            Assert.AreEqual(groupId, group.Id);
        }

        [TestMethod]
        public void DeleteGroup()
        {
            var preCount = Client.GetGroups().Count;
            var groupId = "5963e39d2f02d312f01a7dcf";
            Client.DeleteGroup(groupId);
            var postCount = Client.GetGroups().Count;
            Assert.AreEqual(preCount - 1, postCount);
        }

        [TestMethod]
        public void GroupCollection()
        {
            var collection = Client.GetGroups();
            Print(collection);
            Assert.IsNotNull(collection);
        }

        [TestMethod]
        public void GetGroupUsers()
        {
            var groupId = "5870184f9898000087020002";
            var collection = Client.GetGroupUsers(groupId);
            Print(collection);
        }


        [TestMethod]
        public void AddUserToGroup()
        {
            var userId = "user_bef163ba98af433c917914dd4c208115";
            var groupId = "group_7ac29362a8c743aaa798b331cc87919e";
            var preCount = Client.GetGroupUsers(groupId).Count;
            Client.AddUserToGroup(groupId, userId);
            var postCount = Client.GetGroupUsers(groupId).Count;
            Assert.AreEqual(preCount + 1, postCount);
        }



        [TestMethod]
        public void RemoveUserFromGroup()
        {
            var userId = "user_bef163ba98af433c917914dd4c208115";
            var groupId = "group_7ac29362a8c743aaa798b331cc87919e";
            var preCount = Client.GetGroupUsers(groupId).Count;
            Client.RemoveUserFromGroup(groupId, userId);
            var postCount = Client.GetGroupUsers(groupId).Count;
            Assert.AreEqual(preCount - 1, postCount);
        }


        #endregion GroupTestCases




      
    }
}

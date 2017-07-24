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
        
       

        [TestMethod]
        public void GetSchema()
        {
            string schemaId = "597265b42f02d31290052f88";
            Client.GetSchema(schemaId);
        }
        

        #region User TestCases

        [TestMethod]
        public void CreateUser()
        {
            int count_v1 = Client.GetUsers().Count;
            string firstName = "Derek";
            string lastName = "Nguyen";
            string email = String.Format("{0}{1}@test.com", firstName, DateTimeOffset.Now.ToUnixTimeMilliseconds());
            string password = "P@ssWo3d";
            User newUser = User.Create(firstName, lastName, email,  password);
            User entity = Client.CreateUser(newUser);
            Assert.IsNotNull(entity);
            Assert.IsNotNull(entity.Id);
            int count_v2 = Client.GetUsers().Count;
            Assert.AreEqual(count_v1 + 1, count_v2);
        }

        [TestMethod]
        public void GetUserCollection()
        {
            EntityCollection<User> userCollection = Client.GetUsers();
            Print(userCollection);
            Assert.IsNotNull(userCollection);
            Assert.AreEqual(userCollection.Count, userCollection.Values.Count);
        }

        [TestMethod]
        public void GetUser()
        {
            string userId = "66da331f7a1c49d98ac8a4ad136c7c64";
            User user = Client.GetUser(userId);
            Assert.IsNotNull(user);
            Assert.AreEqual(userId, user.Id);
        }

        [TestMethod]
        public void GetUserSubscriptions()
        {
            string userId = "66da331f7a1c49d98ac8a4ad136c7c64";
            EntityCollection<Subscription> collection = Client.GetUserSubscription(userId);
            Assert.IsNotNull(collection);
            Print(collection);
        }

        [TestMethod]
        public void GetUserGroup()
        {
            string userId = "user_bef163ba98af433c917914dd4c208115";
            EntityCollection<Group> collection = Client.GetUserGroups(userId);
            Assert.IsNotNull(collection);
            Print(collection);
        }

        [TestMethod]
        public void DeleteUser()
        {
            int count_v1 = Client.GetUsers().Count;
            string userId = "user__1c83a712efdb41fe8b9ef0687d3e7b17";
            Client.DeleteUser(userId);
            int count_v2 = Client.GetUsers().Count;
            Assert.AreEqual(count_v1 - 1, count_v2);
        }
        
        [TestMethod]
        public void DeleteUserWithSubscription()
        {
            int count_v1 = Client.GetUsers().Count;
            string userId = "";
            Client.DeleteUser(userId);
            int count_v2 = Client.GetUsers().Count;
            Assert.AreEqual(count_v1 - 1, count_v2);
        }

        [TestMethod]
        public void GetUserSsoURL()
        {
            string userId = "66da331f7a1c49d98ac8a4ad136c7c64";
            SsoUrl user = Client.GenerateSsoURL(userId);
            Assert.IsNotNull(user);
            Assert.IsNotNull(user.Url);
        }

        [TestMethod]
        public void UpdateUser()
        {
            string userId = "66da331f7a1c49d98ac8a4ad136c7c64";
            
            UpdateUser updateUser = new UpdateUser(userId);
            updateUser.FirstName = "NewName";
            updateUser.Email = "updateEmail@gmail.com";

            Client.UpdateUser(updateUser);

            User currUser = Client.GetUser(userId);
            Assert.AreEqual(currUser.FirstName, updateUser.FirstName);
        }
        
        #endregion User TestCases


       


        #region API TestCases

     

        [TestMethod]
        public void CreateApi()
        {
            int count_v1 = Client.GetAPIs().Count;
            string name = "Server API";
            string description = "An example to create apis from service";
            string serviceUrl = "server.com";
            string path = "/v1";
            string[] protocols = new string[]{ "http", "https" };
            
            API newAPI = API.Create(name, serviceUrl, path, protocols, description);
            API api = Client.CreateAPI(newAPI);
            int count_v2 = Client.GetAPIs().Count;
            Assert.IsNotNull(api.Id);
            Assert.AreEqual(count_v1 + 1, count_v2);
        }

       

        [TestMethod]
        public void GetApi()
        {
            string apiId = "api_05247cffcf9d4817adc81663625c18a1";
            API api = Client.GetAPI(apiId);
            Assert.IsNotNull(api);
            Assert.AreEqual(api.Id, apiId);
            Print(api);
        }


        [TestMethod]
        public void ApiCollection()
        {
            EntityCollection<API> apis = Client.GetAPIs();
            Assert.IsNotNull(apis);
            Assert.IsTrue(apis.Count > 0);
            Print(apis);
        }

        [TestMethod]
        public void DeleteAPI()
        {
            int count_v1 = Client.GetAPIs().Count;
            string apiId = "api_05247cffcf9d4817adc81663625c18a1";
            Client.DeleteAPI(apiId);
            int count_v2 = Client.GetAPIs().Count;
            Assert.AreEqual(count_v1 - 1, count_v2);
    }
        #endregion APITestCases





        #region API Operations TestCases

        [TestMethod]
        public void CreateAPIOperation()
        {
            string apiId = "api_2fece789626a4b11a71e040c6ba63b8f";
            int count_v1 = Client.GetOperationsByAPI(apiId).Count;

            string name = "Server API operation";
            RequestMethod method = RequestMethod.GET;
            string urlTemplate = "/Get/a/{a}/b/{b}";

            ParameterContract[] parameters =
            {
                ParameterContract.Create("a", ParameterType.NUMBER.ToString()),
                ParameterContract.Create("b", ParameterType.NUMBER.ToString())
            };


            RequestContract request = new RequestContract();
            request.Headers = new ParameterContract[] {
                                            ParameterContract.Create("Ocp-Apim-Subscription-Key", ParameterType.STRING.ToString())
                                        };
            request.QueryParameters = new ParameterContract[] {
                                            ParameterContract.Create("filter", ParameterType.STRING.ToString())
                                        };


            ResponseContract[] responses = {
                   ResponseContract.Create(200, "good 200", new RepresentationContract[]{
                       RepresentationContract.Create("application/json", "schemaId", "typeName", "sample data", GetFormParameters())
                   }),
                   ResponseContract.Create(201, "not much better", null),
            };
            APIOperation operation = APIOperation.Create(name, method, urlTemplate, parameters, request, responses);
           

            APIOperation entity = Client.CreateAPIOperation(apiId, operation);
            Assert.IsNotNull(entity);
            Assert.IsNotNull(entity.Id);

            int count_v2 = Client.GetOperationsByAPI(apiId).Count;

            Assert.AreEqual(count_v1 + 1, count_v2);
        }

        private ParameterContract[] GetFormParameters()
        {
            return new ParameterContract[]{
                            ParameterContract.Create("accountId", "long"),
                            ParameterContract.Create("profileId", "long")
                       };

        }
        


        //[TestMethod]
        //public void Create()
        //{
        //    string apiId = "api_f8c105c775dd4123b201cf11adacede3";
        //    string ResourcePath = @"C:\Repositories\AzureAPIManagement\Azure.ApiManagement.Test\SwaggerObject.json";
        //    String json = File.ReadAllText(ResourcePath);
        //    APIOperation operation = JsonConvert.DeserializeObject<APIOperation>(json);
        //    operation.Id = "zzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz";
        //    Print(operation);
        //    APIOperation entity = Client.CreateAPIOperation(apiId, operation);
        //    Assert.IsNotNull(entity);
        //    Assert.IsNotNull(entity.Id);
        //}


        [TestMethod]
        public void GetAPIOperation()
        {
            string apiId = "597265b42f02d30ff48b3264";
            string operationId = "597265b42f02d31290052f89";
            APIOperation operation = Client.GetAPIOperation(apiId, operationId);
            Print(operation);
            Assert.IsNotNull(operation);
        }

        [TestMethod]
        public void GetOperationsByAPI()
        {
            string apiId = "api_f98e6f1c4f674a35888aa1e8979e331e";
            EntityCollection<APIOperation> collection = Client.GetOperationsByAPI(apiId);
            Assert.IsNotNull(collection);
            Print(collection);
        }

        [TestMethod]

        public void DelteAPIOperation()
        {
            string apiId = "65d17612d5074d8bbfde4026357a24da";
            string operationId = "d6be400efb924ea18c615cdcc486d278";
            int count_v1 = Client.GetOperationsByAPI(apiId).Count;

            APIOperation operation = Client.DeleteOperation(apiId, operationId);
            int count_v2 = Client.GetOperationsByAPI(apiId).Count;
            Assert.IsNotNull(operation);
            Assert.AreEqual(count_v1 - 1, count_v2);
        }

        #endregion API Operations TestCases




        #region Product TestCases

        [TestMethod]
        public void CreateProduct()
        {
            int count_v1 = Client.GetProducts().Count;
            Product product = Product.Create("Server product", "This product is created from the server");
            Product entity = Client.CreateProduct(product);
            Assert.IsNotNull(entity);
            Assert.IsNotNull(entity.Id);
            int count_v2 = Client.GetProducts().Count;
            Assert.AreEqual(count_v1 + 1, count_v2);
        }

        [TestMethod]
        public void GetProduct()
        {
            string productId = "29f79d2acfab453eac057ddf3656a31b";
            Product product = Client.GetProduct(productId);
            Assert.IsNotNull(product);
            Assert.AreEqual(productId, product.Id);
            Print(product);
        }

        //[TestMethod]
        //public void PublishProduct()
        //{
        //    string productId = "5870184f9898000087060001";
        //    ProductState state = ProductState.notPublished;
        //    Client.UpdateProductState(productId, state);
        //    Product product = Client.GetProduct(productId);
        //    Assert.AreEqual(product.State, state);


        //    //foreach (Product product in collection.Values)
        //    //{
        //    //    Print(product.State.ToString() + " " + product.Id);
        //    //}
        //}

        [TestMethod]
        public void UpdateProduct()
        {
            string productId = "product_5cdf0c46784b4e98b326f426bb6c2c81";

            UpdateProduct updateProduct = new UpdateProduct(productId);
            updateProduct.State = ProductState.published;
            updateProduct.Name = "updated name";
            updateProduct.Description = "This is updated description";

            Client.UpdateProduct(updateProduct);
            Product product = Client.GetProduct(productId);

            Assert.AreEqual(product.State, updateProduct.State);
            Assert.AreEqual(product.Description, updateProduct.Description);
        }

        [TestMethod]
        public void DeletProduct()
        {
            string productId = "product_9ddcf499a9e540eab24bf6f3323d956b";
            int count_v1 = Client.GetProducts().Count;
            Client.DeleteProduct(productId);
            int count_v2 = Client.GetProducts().Count;

            Assert.AreEqual(count_v1, count_v2);
        }

        [TestMethod]
        public void ProductCollection()
        {
            EntityCollection<Product> products = Client.GetProducts();
            Assert.IsNotNull(products);
        }


        [TestMethod]
        public void GetProductSubscriptions()
        {
            string productId = "5956a9b92f02d30b88dfad7d";
            EntityCollection<Subscription> collection = Client.GetProductSubscriptions(productId);
            Assert.IsNotNull(collection);
        }


        [TestMethod]
        public void GetProductAPIs()
        {
            string productId = "5956a9b92f02d30b88dfad7d";
            EntityCollection<API> collection = Client.GetProductAPIs(productId);
            Assert.IsNotNull(collection);
        }
        [TestMethod]
        public void AddProductApi()
        {
            string productId = "5956a9b92f02d30b88dfad7d";
            string apiId = "5956a87a2f02d30b88dfad7b";
            int count_v1 = Client.GetProductAPIs(productId).Count;
            Client.AddProductAPI(productId, apiId);
            int count_v2 = Client.GetProductAPIs(productId).Count;

            Assert.AreEqual(count_v1 + 1, count_v2);
        }

        [TestMethod]
        public void DeleteProductApi()
        {
            string productId = "5956a9b92f02d30b88dfad7d";
            string apiId = "5956a87a2f02d30b88dfad7b";
            int count_v1 = Client.GetProductAPIs(productId).Count;
            Client.DeleteProductAPI(productId, apiId);
            int count_v2 = Client.GetProductAPIs(productId).Count;

            Assert.AreEqual(count_v1 - 1, count_v2);
        }


        [TestMethod]
        public void GetProductGroups()
        {
            string productId = "5956a9b92f02d30b88dfad7d";
            EntityCollection<Group> collection = Client.GetProductGroups(productId);
            Assert.IsNotNull(collection);
        }
        [TestMethod]
        public void AddProductGroup()
        {
            string productId = "5956a9b92f02d30b88dfad7d";
            string groupId = "5870184f9898000087020001";
            int count_v1 = Client.GetProductGroups(productId).Count;
            Client.AddProductGroup(productId, groupId);
            int count_v2 = Client.GetProductGroups(productId).Count;
            Assert.AreEqual(count_v1 + 1, count_v2);
        }
        [TestMethod]
        public void DeleteProductGroup()
        {
            string productId = "5956a9b92f02d30b88dfad7d";
            string groupId = "5870184f9898000087020001";
            int count_v1 = Client.GetProductGroups(productId).Count;
            Client.DeleteProductGroup(productId, groupId);
            int count_v2 = Client.GetProductGroups(productId).Count;
            Assert.AreEqual(count_v1 - 1, count_v2);
        }


        #endregion Product TestCases




        #region Subscription TestCases

        [TestMethod]
        public void CreateSubscription()
        {
            int c1 = Client.GetSubscriptions().Count;
            string userId = "user_bef163ba98af433c917914dd4c208115";
            string productId = "5870184f9898000087060002";
            string name = "server subscriptions";
            DateTime now = DateTime.Now;
            SubscriptionDateSettings dateSettings = new SubscriptionDateSettings(now.AddDays(1), now.AddMonths(2));
            Subscription subscription = Subscription.Create(userId, productId, name, dateSettings, SubscriptionState.active);
            Subscription entity = Client.CreateSubscription(subscription);
            Assert.IsNotNull(entity);
            Assert.IsNotNull(entity.Id);
            int c2 = Client.GetSubscriptions().Count;
            Assert.AreEqual(c1 + 1, c2);
        }

        [TestMethod]
        public void DeleteSubscription()
        {
            int c1 = Client.GetSubscriptions().Count;
            string subscriptionId = "subscription_c0ddc8fd75934e1eb2325ff507908140";
            Client.DeleteSubscription(subscriptionId);
            int c2 = Client.GetSubscriptions().Count;
            Assert.AreEqual(c1 - 1, c2);
        }

        [TestMethod]
        public void UpdateSubscription()
        {
            string subscriptionId  = "subscription_72208da5700b45e8a016605ccdc78aa1";
            Subscription entity_v1 = Client.GetSubscription(subscriptionId);

            UpdateSubscription updateSubscription = new UpdateSubscription(subscriptionId);
            DateTime now = DateTime.Now;
            updateSubscription.ExpirationDate = now.AddMonths(23);
            Print(JsonConvert.SerializeObject(updateSubscription));
            Client.UpdateSubscription(subscriptionId, updateSubscription);
            Subscription entity_v2 = Client.GetSubscription(subscriptionId);
            
            Assert.AreNotEqual(entity_v1.ExpirationDate, entity_v2.ExpirationDate);
        }

        [TestMethod]
        public void GetSubscriptionCollection()
        {
            EntityCollection<Subscription> collection = Client.GetSubscriptions();
            Assert.IsNotNull(collection);
            Print(collection);
        }

        [TestMethod]
        public void GetSubscription()
        {
            string subscriptionId = "subscription_801c5e5a9a9d4e9fba47b1561d1e19f6";
            Subscription subscription = Client.GetSubscription(subscriptionId);
            Print(subscription);
            Assert.IsNotNull(subscription);
            Assert.AreEqual(subscriptionId, subscription.Id);
        }


        [TestMethod]
        public void GeneratePrimaryKey()
        {
            string subscriptionId = "5870184f9898000087070001";
            string key_v1 = Client.GetSubscription(subscriptionId).PrimaryKey;
            Client.GeneratePrimaryKey(subscriptionId);
            string key_v2 = Client.GetSubscription(subscriptionId).PrimaryKey;
            Assert.AreNotEqual(key_v1, key_v2);
        }

        [TestMethod]
        public void GenerateSecondaryKey()
        {
            string subscriptionId = "5870184f9898000087070001";
            string key_v1 = Client.GetSubscription(subscriptionId).SecondaryKey;
            Client.GenerateSecondaryKey(subscriptionId);
            string key_v2 = Client.GetSubscription(subscriptionId).SecondaryKey;
            Assert.AreNotEqual(key_v1, key_v2);

        }

        #endregion Subscription TestCases



        #region GroupTestCases

        [TestMethod]
        public void CreateGroup()
        {
            int count_v1 = Client.GetGroups().Count;
            string name = "server group 3";
            string description = "this group is created from server";
            Group group = Group.Create(name, description, GroupType.system);
            Group entity = Client.CreateGroup(group);
            Assert.IsNotNull(entity);
            Assert.IsNotNull(entity.Id);
            int count_v2 = Client.GetGroups().Count;
            Assert.AreEqual(count_v1 + 1, count_v2);
        }

        [TestMethod]
        public void GetGroup()
        {
            string groupId = "group_7ac29362a8c743aaa798b331cc87919e";
            Group group = Client.GetGroup(groupId);
            Assert.IsNotNull(group);
            Assert.AreEqual(groupId, group.Id);
        }

        [TestMethod]
        public void DeleteGroup()
        {
            int count_v1 = Client.GetGroups().Count;
            string groupId = "5963e39d2f02d312f01a7dcf";
            Client.DeleteGroup(groupId);
            int count_v2 = Client.GetGroups().Count;
            Assert.AreEqual(count_v1 - 1, count_v2);
        }

        [TestMethod]
        public void GroupCollection()
        {
            EntityCollection<Group> collection = Client.GetGroups();
            Assert.IsNotNull(collection);
            Print(collection);
        }

        [TestMethod]
        public void GetGroupUsers()
        {
            string groupId = "5870184f9898000087020002";
            EntityCollection<User> collection = Client.GetUsersInGroup(groupId);
            Assert.IsNotNull(collection);
            Print(collection);
        }


        [TestMethod]
        public void AddUserToGroup()
        {
            string userId = "user_bef163ba98af433c917914dd4c208115";
            string groupId = "group_7ac29362a8c743aaa798b331cc87919e";
            int count_v1 = Client.GetUsersInGroup(groupId).Count;
            Client.AddUserToGroup(groupId, userId);
            int count_v2 = Client.GetUsersInGroup(groupId).Count;
            Assert.AreEqual(count_v1 + 1, count_v2);
        }



        [TestMethod]
        public void RemoveUserFromGroup()
        {
            string userId = "user_bef163ba98af433c917914dd4c208115";
            string groupId = "group_7ac29362a8c743aaa798b331cc87919e";
            int count_v1 = Client.GetUsersInGroup(groupId).Count;
            Client.RemoveUserFromGroup(groupId, userId);
            int count_v2 = Client.GetUsersInGroup(groupId).Count;
            Assert.AreEqual(count_v1 - 1, count_v2);
        }


        #endregion GroupTestCases



        #region Logger TestCases

        [TestMethod]
        public void CreateLogger()
        {
            string loggerType = "azureEventHub";
            string name = "ServerLogger_v1";
            string description = "This logger is created in server";
            object credentials = null;
            Logger logger = Logger.Create(loggerType, name, description, credentials);
            Logger entity = Client.CreateLogger(logger);
        }

        [TestMethod]
        public void GetLoggers()
        {
            EntityCollection<Logger> loggers = Client.GetLoggers();
        }

#endregion

    }
}

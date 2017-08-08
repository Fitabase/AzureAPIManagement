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
using Fitabase.Azure.ApiManagement.Model.Exceptions;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

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
            Client = new ManagementClient(@"C:\Repositories\AzureAPIManagement\Azure.ApiManagement.Test\APIMKeys.json");
        }

 
        /*********************************************************/
        /************************  USER  *************************/
        /*********************************************************/

        #region User TestCases

        [TestMethod]
        public void CreateUser()
        {
            int count_v1 = Client.GetUsers().Count;
            string firstName = "Derek";
            string lastName = "Nguyen";
            string email = String.Format("{0}{1}@test.com", firstName, DateTimeOffset.Now.ToUnixTimeMilliseconds());
            string password = "P@ssWo3d";
            User newUser = User.Create(firstName, lastName, email, password);
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
            Assert.IsNotNull(userCollection);
            Assert.AreEqual(userCollection.Count, userCollection.Values.Count);
        }

        [TestMethod]
        public void GetUser()
        {
            string userId = "user_bef163ba98af433c917914dd4c208115";
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
        }

        [TestMethod]
        public void GetUserGroup()
        {
            string userId = "user_bef163ba98af433c917914dd4c208115";
            EntityCollection<Group> collection = Client.GetUserGroups(userId);
            Assert.IsNotNull(collection);
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
            User user = new User()
            {
                Id = userId,
                FirstName = "serverName"
            };
            Client.UpdateUser(user);
            User entity = Client.GetUser(userId);
            Assert.AreEqual(entity.FirstName, user.FirstName);

        }


        #endregion User TestCases



        /*********************************************************/
        /*************************  API  *************************/
        /*********************************************************/

        #region API TestCases



        [TestMethod]
        public void CreateApi()
        {
            int count_v1 = Client.GetAPIs().Count;
            string name = "abcd";
            string description = "An example to create apis from service";
            string serviceUrl = "abc.com";
            string path = "/v4";
            string[] protocols = new string[] { "http", "https" };

            API newAPI = API.Create(name, serviceUrl, path, protocols, description);
            API api = Client.CreateAPI(newAPI);
            int count_v2 = Client.GetAPIs().Count;
            Assert.IsNotNull(api.Id);
            Assert.AreEqual(count_v1 + 1, count_v2);
        }


        [TestMethod]
        public void GetAPI()
        {
            string apiId = "api_b8aad5c90425479c9e50c2513bfbc804";
            API api = Client.GetAPI(apiId);
            Assert.IsNotNull(api);
            Assert.AreEqual(api.Id, apiId);
        }


        [TestMethod]
        public void ApiCollection()
        {
            EntityCollection<API> apis = Client.GetAPIs();
            Assert.IsNotNull(apis);
            Assert.IsTrue(apis.Count > 0);
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


        [TestMethod]
        public void UpdateAPI()
        {
            string apiId = "api_b8aad5c90425479c9e50c2513bfbc804";
            API entity = Client.GetAPI(apiId);
            API api = new API()
            {
                Id = apiId,
                Name = "newName-"
            };
            Client.UpdateAPI(api);

            entity = Client.GetAPI(apiId);
            Assert.AreEqual(entity.Id, api.Id);
            Assert.AreEqual(entity.Name, api.Name);
        }

        #endregion APITestCases



        /*********************************************************/
        /******************   API OPERATIONS  ********************/
        /*********************************************************/


        #region API Operations TestCases

        
        [TestMethod]
        public void APIOperationLifeCycle()
        {
            long c1, c2;
            string apiId = "api_b8aad5c90425479c9e50c2513bfbc804";

            string name = "Operation_v11";
            RequestMethod method = RequestMethod.GET;
            string urlTemplate = "/sum";
            string description = "an operation created in the operation";

            APIOperation operation = APIOperation.Create(name, method, urlTemplate, Parameters(), Request(), Responses(), description);


            #region CREATE

            c1 = Client.GetOperationsByAPI(apiId).Count;
            APIOperation entity = Client.CreateAPIOperation(apiId, operation);
            c2 = Client.GetOperationsByAPI(apiId).Count;
            Assert.AreEqual(c1 + 1, c2);

            #endregion

            #region RETRIEVE

            APIOperation other = Client.GetAPIOperation(apiId, entity.Id);
            Assert.AreEqual(entity.Name, other.Name);
            Assert.AreEqual(entity.UrlTemplate, other.UrlTemplate);

            #endregion

            #region Update INFO
            entity.Name = entity.Name + "---------";
            Client.UpdateAPIOperation(apiId, entity.Id, entity);
            other = Client.GetAPIOperation(apiId, entity.Id);
            Assert.AreEqual(entity.Name, other.Name);
            #endregion


            #region UPDATE REPSPONSE
            other = Client.GetAPIOperation(apiId, entity.Id);
            Assert.AreEqual(entity.Responses.Count(), other.Responses.Count());
            List<ResponseContract> responses = entity.Responses.ToList();
            responses.Add(ResponseContract.Create(400, "so bad", new RepresentationContract[] {
                RepresentationContract.Create("application/json", null, null, "sample code", null)
            }));
            entity.Responses = responses.ToArray();
            Assert.AreEqual(entity.Responses.Count() - 1, other.Responses.Count());
            Client.UpdateAPIOperation(apiId, entity.Id, entity);
            other = Client.GetAPIOperation(apiId, entity.Id);
            Assert.AreEqual(entity.Responses.Count(), other.Responses.Count());
            #endregion



            #region UPDATE REQUEST
            other = Client.GetAPIOperation(apiId, entity.Id);
            Assert.AreEqual(entity.Request.Description, other.Request.Description);
            RequestContract request = RequestContract.Create(entity.Description + " -----new description");
            entity.Request = request;
            Assert.AreNotEqual(entity.Request.Description, other.Request.Description);
            Client.UpdateAPIOperation(apiId, entity.Id, entity);
            other = Client.GetAPIOperation(apiId, entity.Id);
            Assert.AreEqual(entity.Request.Description, other.Request.Description);
            #endregion


            #region UPATE PARAMETERS
            other = Client.GetAPIOperation(apiId, entity.Id);
            Assert.AreEqual(entity.UrlTemplate, other.UrlTemplate);
            Assert.AreEqual(entity.TemplateParameters.Count(), other.TemplateParameters.Count());
            APIOperationHelper helper = new APIOperationHelper(entity);

            List<ParameterContract> parameters = new List<ParameterContract>();
            parameters.Add(ParameterContract.Create("account", "uuid"));
            parameters.Add(ParameterContract.Create("other", "number"));
            parameters.Add(ParameterContract.Create("start", "date-time"));
            parameters.Add(ParameterContract.Create("end", "date-time"));
            parameters.Add(ParameterContract.Create("description", "string"));


            entity.UrlTemplate = APIOperationHelper.BuildURL(helper.GetOriginalURL(), parameters);
            entity.TemplateParameters = parameters.ToArray();

            Assert.AreNotEqual(entity.TemplateParameters.Count(), other.TemplateParameters.Count());
            Assert.AreNotEqual(entity.UrlTemplate, other.UrlTemplate);
            Client.UpdateAPIOperation(apiId, entity.Id, entity);

            other = Client.GetAPIOperation(apiId, entity.Id);
            Assert.AreEqual(entity.UrlTemplate, other.UrlTemplate);
            Assert.AreEqual(entity.TemplateParameters.Count(), other.TemplateParameters.Count());

            #endregion



            #region DELETE Operation
            c1 = Client.GetOperationsByAPI(apiId).Count;
            Client.DeleteOperation(apiId, entity.Id);
            c2 = Client.GetOperationsByAPI(apiId).Count;
            Assert.AreEqual(c1 - 1, c2);
            #endregion
        }


        private ResponseContract[] Responses()
        {
            ResponseContract[] responses = {
                   ResponseContract.Create(200, "OK!", new RepresentationContract[]{
                       RepresentationContract.Create("application/json", null, "typeName", null, null),
                       RepresentationContract.Create("text/json", null, "typeName", "sample data", Parameters()),
                   }),
                   ResponseContract.Create(201, "Created", null),
            };
            return responses;

        }

        private RequestContract Request()
        {
            RequestContract request = new RequestContract();
            request.Headers = new ParameterContract[] {
                                            ParameterContract.Create("Ocp-Apim-Subscription-Key", ParameterType.STRING.ToString())
                                        };
            request.QueryParameters = new ParameterContract[] {
                                            ParameterContract.Create("filter", ParameterType.STRING.ToString())
                                        };
            return request;
        }

        private ParameterContract[] Parameters()
        {

            ParameterContract[] parameters =
            {
                ParameterContract.Create("a", ParameterType.NUMBER.ToString()),
                ParameterContract.Create("b", ParameterType.NUMBER.ToString())
            };
            return parameters;
        }


        [TestMethod]
        public void UpdateAPIOperation()
        {
            string apiId = "597687442f02d30494230f8c";
            string operationId = "597687442f02d31290052fed";
            APIOperation entity_v1 = Client.GetAPIOperation(apiId, operationId);

            APIOperation operation = new APIOperation()
            {
                Name = "New Operation Name",
                Method = "POST"
            };

            Client.UpdateAPIOperation(apiId, operationId, operation);
            APIOperation entity_v2 = Client.GetAPIOperation(apiId, operationId);

            Assert.AreNotEqual(entity_v1.Name, entity_v2.Name);
            Assert.AreNotEqual(entity_v1.Method, entity_v2.Method);
        }

        


        [TestMethod]
        public void UpdateOperationParameter()
        {
            string apiId = "api_b8aad5c90425479c9e50c2513bfbc804";
            string operationId = "operation_ab7e97314cb840eca6cead919d7c003b";

            APIOperation entity = Client.GetAPIOperation(apiId, operationId);
            APIOperationHelper helper = new APIOperationHelper(entity);




            List<ParameterContract> parameters = new List<ParameterContract>();
            parameters.Add(ParameterContract.Create("account", "uuid"));

            entity.UrlTemplate = APIOperationHelper.BuildURL(helper.GetOriginalURL(), parameters);
            entity.TemplateParameters = parameters.ToArray();

            Client.UpdateAPIOperation(apiId, operationId, entity);

        }

        [TestMethod]
        public void UpdateOperationResponse()
        {
            string apiId = "api_b8aad5c90425479c9e50c2513bfbc804";
            string operationId = "operation_ab7e97314cb840eca6cead919d7c003b";
            APIOperation entity_v1 = Client.GetAPIOperation(apiId, operationId);

            ResponseContract response = ResponseContract.Create(400, "Ok", null);
            List<ResponseContract> responses = entity_v1.Responses.ToList();

            responses.Add(response);

            APIOperation operation = new APIOperation()
            {
                Responses = responses.ToArray()
            };

            Client.UpdateAPIOperation(apiId, operationId, operation);
            APIOperation entity_v2 = Client.GetAPIOperation(apiId, operationId);
            Assert.AreEqual(entity_v2.Responses.Count(), operation.Responses.Count());
        }


        [TestMethod]
        public void GetAPIOperation()
        {
            try
            {
                string apiId = "api_2ee0f0a800334301b857367980c332c4";
                string operationId = "operation_9a4eea768ecc48a5be9fcb8f33781189";
                APIOperation operation = Client.GetAPIOperation(apiId, operationId);
                Assert.IsNotNull(operation);
            }
            catch (HttpResponseException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(ex.StatusCode);
            }
        }

        [TestMethod]
        public void GetOperationsByAPI()
        {
            string apiId = "597687442f02d30494230f8c";
            EntityCollection<APIOperation> collection = Client.GetOperationsByAPI(apiId);
            Assert.IsNotNull(collection);
        }


        [TestMethod]

        public void DelteAPIOperation()
        {
            string apiId = "65d17612d5074d8bbfde4026357a24da";
            string operationId = "d6be400efb924ea18c615cdcc486d278";
            int count_v1 = Client.GetOperationsByAPI(apiId).Count;

            Client.DeleteOperation(apiId, operationId);
            int count_v2 = Client.GetOperationsByAPI(apiId).Count;
            Assert.AreEqual(count_v1 - 1, count_v2);
        }


        [TestMethod]
        public void DeleteOperationResponse()
        {
            string apiId = "597687442f02d30494230f8c";
            string operationId = "597687442f02d31290052fec";
            int statusCode = 200;

            APIOperation entity = Client.GetAPIOperation(apiId, operationId);
            List<ResponseContract> responses = entity.Responses.ToList();
            foreach (ResponseContract response in responses)
            {
                if (response.StatusCode == statusCode)
                {
                    responses.Remove(response);
                    break;
                }
            }
            APIOperation operation = new APIOperation()
            {
                Responses = responses.ToArray()
            };
            Client.UpdateAPIOperation(apiId, operationId, operation);
            entity = Client.GetAPIOperation(apiId, operationId);
            Assert.AreEqual(entity.Responses.Count(), operation.Responses.Count());
        }

        #endregion API Operations TestCases



        /*********************************************************/
        /**********************  PRODUCT  ************************/
        /*********************************************************/

        #region Product TestCases

        [TestMethod]
        public void CreateProduct()
        {
            int count_v1 = Client.GetProducts().Count;
            Product product = Product.Create("new Server product", "This product is created from the server", ProductState.published);
            Product entity = Client.CreateProduct(product);
            Assert.IsNotNull(entity);
            Assert.IsNotNull(entity.Id);
            int count_v2 = Client.GetProducts().Count;
            Assert.AreEqual(count_v1 + 1, count_v2);
        }

        [TestMethod]
        public void GetProduct()
        {
            string productId = "product_5cdf0c46784b4e98b326f426bb6c2c81";
            Product product = Client.GetProduct(productId);
            Assert.IsNotNull(product);
            Assert.AreEqual(productId, product.Id);
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
            string productId = "29f79d2acfab453eac057ddf3656a31b";
            Product product = new Product()
            {
                Id = productId,
                Name = "AbcProduct"
            };
            Client.UpdateProduct(product);
            Product entity = Client.GetProduct(productId);
            Assert.AreEqual(product.Name, entity.Name);
        }


        [TestMethod]
        public void DeletProduct()
        {
            string productId = "product_5cdf0c46784b4e98b326f426bb6c2c81";
            int count_v1 = Client.GetProducts().Count;
            Client.DeleteProduct(productId);
            int count_v2 = Client.GetProducts().Count;

            Assert.AreEqual(count_v1 - 1, count_v2);
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






        /*********************************************************/
        /**********************  SUBSCRIPTION  *******************/
        /*********************************************************/

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
            string subscriptionId = "5870184f9898000087070001";
            Subscription subscription = new Subscription()
            {
                Id = subscriptionId,
                Name = "newServerName",
                ExpirationDate = DateTime.Now
            };
            Client.UpdateSubscription(subscription);
            Subscription entity = Client.GetSubscription(subscriptionId);

            Assert.AreEqual(subscription.Name, entity.Name);
        }

        [TestMethod]
        public void GetSubscriptionCollection()
        {
            EntityCollection<Subscription> collection = Client.GetSubscriptions();
            Assert.IsNotNull(collection);
        }

        [TestMethod]
        public void GetSubscription()
        {
            string subscriptionId = "subscription_72208da5700b45e8a016605ccdc78aa1";
            Subscription subscription = Client.GetSubscription(subscriptionId);
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



        /*********************************************************/
        /**********************  GROUP  **************************/
        /*********************************************************/

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
        }

        [TestMethod]
        public void GetGroupUsers()
        {
            string groupId = "5870184f9898000087020002";
            EntityCollection<User> collection = Client.GetUsersInGroup(groupId);
            Assert.IsNotNull(collection);
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




        /*********************************************************/
        /**********************  LOGGERs  ************************/
        /*********************************************************/

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
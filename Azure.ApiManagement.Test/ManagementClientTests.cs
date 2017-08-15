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
            int count_v1 = Client.GetUsersAsync().Result.Count;
            string firstName = "Derek";
            string lastName = "Nguyen";
            string email = String.Format("{0}{1}@test.com", firstName, DateTimeOffset.Now.ToUnixTimeMilliseconds());
            string password = "P@ssWo3d";
            User newUser = User.Create(firstName, lastName, email, password);
            User entity = Client.CreateUserAsync(newUser).Result;
            Assert.IsNotNull(entity);
            Assert.IsNotNull(entity.Id);
            int count_v2 = Client.GetUsersAsync().Result.Count;
            Assert.AreEqual(count_v1 + 1, count_v2);
        }

        [TestMethod]
        public void GetUserCollection()
        {
            EntityCollection<User> userCollection = Client.GetUsersAsync().Result;
            Assert.IsNotNull(userCollection);
            Assert.AreEqual(userCollection.Count, userCollection.Values.Count);
        }

        [TestMethod]
        public void GetUser()
        {
            string userId = "user_bef163ba98af433c917914dd4c208115";
            User user = Client.GetUserAsync(userId).Result;
            Assert.IsNotNull(user);
            Assert.AreEqual(userId, user.Id);
        }

        [TestMethod]
        public void GetUserSubscriptions()
        {
            string userId = "66da331f7a1c49d98ac8a4ad136c7c64";
            EntityCollection<Subscription> collection = Client.GetUserSubscriptionAsync(userId).Result;
            Assert.IsNotNull(collection);
        }

        [TestMethod]
        public void GetUserGroup()
        {
            string userId = "user_bef163ba98af433c917914dd4c208115";
            EntityCollection<Group> collection = Client.GetUserGroupsAsync(userId).Result;
            Assert.IsNotNull(collection);
        }

        [TestMethod]
        public void DeleteUser()
        {
            int count_v1 = Client.GetUsersAsync().Result.Count;
            string userId = "user__1c83a712efdb41fe8b9ef0687d3e7b17";
            var task = Client.DeleteUserAsync(userId);
            task.Wait();
            int count_v2 = Client.GetUsersAsync().Result.Count;
            Assert.AreEqual(count_v1 - 1, count_v2);
        }

        [TestMethod]
        public void DeleteUserWithSubscription()
        {
            int count_v1 = Client.GetUsersAsync().Result.Count;
            string userId = "";
            var task = Client.DeleteUserAsync(userId);
            task.Wait();
            int count_v2 = Client.GetUsersAsync().Result.Count;
            Assert.AreEqual(count_v1 - 1, count_v2);
        }

        [TestMethod]
        public void GetUserSsoURL()
        {
            string userId = "66da331f7a1c49d98ac8a4ad136c7c64";
            SsoUrl user = Client.GenerateSsoURLAsync(userId).Result;
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
            var task = Client.UpdateUserAsync(user);
            task.Wait();
            User entity = Client.GetUserAsync(userId).Result;
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
            int count_v1 = Client.GetAPIsAsync().Result.Count;
            string name = "test";
            string description = "An example to create apis from service";
            string serviceUrl = "test.com";
            string path = "/v4";
            string[] protocols = new string[] { "http", "https" };

            API newAPI = API.Create(name, serviceUrl, path, protocols, description);
            API api = Client.CreateAPIAsync(newAPI).Result;
            int count_v2 = Client.GetAPIsAsync().Result.Count;
            Assert.IsNotNull(api.Id);
            Assert.AreEqual(count_v1 + 1, count_v2);
        }


        [TestMethod]
        public void GetAPI()
        {
            string apiId = "598e06832f02d3110cf5100b";
            API api = Client.GetAPIAsync(apiId).Result;
            string json = JsonConvert.SerializeObject(api);
            Assert.IsNotNull(api);
            Assert.AreEqual(api.Id, apiId);
        }


        [TestMethod]
        public void ApiCollection()
        {
            EntityCollection<API> apis = Client.GetAPIsAsync().Result;
            Assert.IsNotNull(apis);
            Assert.IsTrue(apis.Count > 0);
        }

        [TestMethod]
        public void DeleteAPI()
        {
            int count_v1 = Client.GetAPIsAsync().Result.Count;
            string apiId = "api_05247cffcf9d4817adc81663625c18a1";
            var task = Client.DeleteAPIAsync(apiId);
            task.Wait();
            int count_v2 = Client.GetAPIsAsync().Result.Count;
            Assert.AreEqual(count_v1 - 1, count_v2);
        }


        [TestMethod]
        public void UpdateAPI()
        {
            string apiId = "api_b8aad5c90425479c9e50c2513bfbc804";
            API entity = Client.GetAPIAsync(apiId).Result;
            API api = new API()
            {
                Id = apiId,
                Name = "newName-"
            };
            var task = Client.UpdateAPIAsync(api);
            task.Wait();

            entity = Client.GetAPIAsync(apiId).Result;
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
            string apiId = "api_577edd5ee62543d297bd5d568af78a82";

            string name = "Operation_3";
            RequestMethod method = RequestMethod.DELETE;
            string urlTemplate = "/pet";
            string description = "post it";

            APIOperation operation = APIOperation.Create(name, method, urlTemplate, Parameters(), null, null, description);

            #region CREATE

            c1 = Client.GetOperationsByAPIAsync(apiId).Result.Count;
            APIOperation entity = Client.CreateAPIOperationAsync(apiId, operation).Result;
            c2 = Client.GetOperationsByAPIAsync(apiId).Result.Count;
            Assert.AreEqual(c1 + 1, c2);

            #endregion

            /*
            #region RETRIEVE

            APIOperation other = Client.GetAPIOperationAsync(apiId, entity.Id).Result;
            Assert.AreEqual(entity.Name, other.Name);
            Assert.AreEqual(entity.UrlTemplate, other.UrlTemplate);

            #endregion

            #region Update INFO
            entity.Name = entity.Name + "-new";
            var task = Client.UpdateAPIOperationAsync(apiId, entity.Id, entity);
            task.Wait();
            other = Client.GetAPIOperationAsync(apiId, entity.Id).Result;
            Assert.AreEqual(entity.Name, other.Name);
            #endregion


            #region UPDATE REPSPONSE
            other = Client.GetAPIOperationAsync(apiId, entity.Id).Result;
            Assert.AreEqual(entity.Responses.Count(), other.Responses.Count());
            List<ResponseContract> responses = entity.Responses.ToList();
            responses.Add(ResponseContract.Create(400, "so bad", new RepresentationContract[] {
                RepresentationContract.Create("application/json", null, null, "sample code", null)
            }));
            entity.Responses = responses.ToArray();
            Assert.AreEqual(entity.Responses.Count() - 1, other.Responses.Count());
            task = Client.UpdateAPIOperationAsync(apiId, entity.Id, entity);
            task.Wait();
            other = Client.GetAPIOperationAsync(apiId, entity.Id).Result;
            Assert.AreEqual(entity.Responses.Count(), other.Responses.Count());
            #endregion



            #region UPDATE REQUEST
            other = Client.GetAPIOperationAsync(apiId, entity.Id).Result;
            Assert.AreEqual(entity.Request.Description, other.Request.Description);
            RequestContract request = RequestContract.Create(entity.Description + " -----new description");
            entity.Request = request;
            Assert.AreNotEqual(entity.Request.Description, other.Request.Description);
            task = Client.UpdateAPIOperationAsync(apiId, entity.Id, entity);
            task.Wait();
            other = Client.GetAPIOperationAsync(apiId, entity.Id).Result;
            Assert.AreEqual(entity.Request.Description, other.Request.Description);
            #endregion


            #region UPATE PARAMETERS
            other = Client.GetAPIOperationAsync(apiId, entity.Id).Result;
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
            task = Client.UpdateAPIOperationAsync(apiId, entity.Id, entity);
            task.Wait();

            other = Client.GetAPIOperationAsync(apiId, entity.Id).Result;
            Assert.AreEqual(entity.UrlTemplate, other.UrlTemplate);
            Assert.AreEqual(entity.TemplateParameters.Count(), other.TemplateParameters.Count());

            #endregion



            #region DELETE Operation
            c1 = Client.GetOperationsByAPIAsync(apiId).Result.Count;
            task = Client.DeleteOperationAsync(apiId, entity.Id);
            task.Wait();
            c2 = Client.GetOperationsByAPIAsync(apiId).Result.Count;
            Assert.AreEqual(c1 - 1, c2);
            #endregion
            */
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
                ParameterContract.Create("petId", ParameterType.NUMBER.ToString())
            };
            return parameters;
        }


        [TestMethod]
        public void UpdateAPIOperation()
        {
            string apiId = "598e06832f02d3110cf5100b";
            string operationId = "598e06832f02d30700f1c8f1";
            APIOperation entity_v1 = Client.GetAPIOperationAsync(apiId, operationId).Result;

            APIOperation operation = new APIOperation()
            {
                Name = "New Operation Name",
                Method = "POST"
            };

            var task = Client.UpdateAPIOperationAsync(apiId, operationId, operation);
            task.Wait();
            APIOperation entity_v2 = Client.GetAPIOperationAsync(apiId, operationId).Result;

            Assert.AreNotEqual(entity_v1.Name, entity_v2.Name);
            Assert.AreNotEqual(entity_v1.Method, entity_v2.Method);
        }

        


        [TestMethod]
        public void UpdateOperationParameter()
        {
            string apiId = "api_b8aad5c90425479c9e50c2513bfbc804";
            string operationId = "operation_be5ecb981a0d43678ae492502c925047";

            APIOperation entity = Client.GetAPIOperationAsync(apiId, operationId).Result;
            APIOperationHelper helper = new APIOperationHelper(entity);

            
            List<ParameterContract> parameters = new List<ParameterContract>();
            parameters.Add(ParameterContract.Create("account", "uuid"));
            parameters.Add(ParameterContract.Create("subscription", "uuid"));

            entity.UrlTemplate = APIOperationHelper.BuildURL("/get", parameters);
            entity.TemplateParameters = parameters.ToArray();

            var task = Client.UpdateAPIOperationAsync(apiId, operationId, entity);
            task.Wait();

        }

        [TestMethod]
        public void UpdateOperationResponse()
        {
            string apiId = "api_b8aad5c90425479c9e50c2513bfbc804";
            string operationId = "operation_ab7e97314cb840eca6cead919d7c003b";
            APIOperation entity_v1 = Client.GetAPIOperationAsync(apiId, operationId).Result;

            ResponseContract response = ResponseContract.Create(400, "Ok", null);
            List<ResponseContract> responses = entity_v1.Responses.ToList();

            responses.Add(response);

            APIOperation operation = new APIOperation()
            {
                Responses = responses.ToArray()
            };

            var task = Client.UpdateAPIOperationAsync(apiId, operationId, operation);
            task.Wait();
            APIOperation entity_v2 = Client.GetAPIOperationAsync(apiId, operationId).Result;
            Assert.AreEqual(entity_v2.Responses.Count(), operation.Responses.Count());
        }


        [TestMethod]
        public void GetAPIOperation()
        {
            string apiId = "5991f3b22f02d30bacf57719";
            string operationId = "5991f3b22f02d30700f1c92d";
            APIOperation operation = Client.GetAPIOperationAsync(apiId, operationId).Result;
            Assert.IsNotNull(operation);
        }

        [TestMethod]
        public void GetOperationsByAPI()
        {
            string apiId = "5991f3b22f02d30bacf57719";
            EntityCollection<APIOperation> collection = Client.GetOperationsByAPIAsync(apiId).Result;
            List<string> operationIds = new List<string>();
            foreach(APIOperation operation in collection.Values)
            {
                //operationIds.Add(operation.Name);
            }
            string json = JsonConvert.SerializeObject(operationIds);
            Assert.IsNotNull(collection);
        }


        [TestMethod]

        public void DelteAPIOperation()
        {
            string apiId = "65d17612d5074d8bbfde4026357a24da";
            string operationId = "d6be400efb924ea18c615cdcc486d278";
            int count_v1 = Client.GetOperationsByAPIAsync(apiId).Result.Count;

            var task = Client.DeleteOperationAsync(apiId, operationId);
            task.Wait();
            int count_v2 = Client.GetOperationsByAPIAsync(apiId).Result.Count;
            Assert.AreEqual(count_v1 - 1, count_v2);
        }


        [TestMethod]
        public void DeleteOperationResponse()
        {
            string apiId = "597687442f02d30494230f8c";
            string operationId = "597687442f02d31290052fec";
            int statusCode = 200;

            APIOperation entity = Client.GetAPIOperationAsync(apiId, operationId).Result;
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
            var task = Client.UpdateAPIOperationAsync(apiId, operationId, operation);
            task.Wait();
            entity = Client.GetAPIOperationAsync(apiId, operationId).Result;
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
            int count_v1 = Client.GetProductsAsync().Result.Count;
            Product product = Product.Create("new Server product", "This product is created from the server", ProductState.published);
            Product entity = Client.CreateProductAsync(product).Result;
            Assert.IsNotNull(entity);
            Assert.IsNotNull(entity.Id);
            int count_v2 = Client.GetProductsAsync().Result.Count;
            Assert.AreEqual(count_v1 + 1, count_v2);
        }

        [TestMethod]
        public void GetProduct()
        {
            string productId = "product_5cdf0c46784b4e98b326f426bb6c2c81";
            Product product = Client.GetProductAsync(productId).Result;
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
            var task = Client.UpdateProductAsync(product);
            task.Wait();
            Product entity = Client.GetProductAsync(productId).Result;
            Assert.AreEqual(product.Name, entity.Name);
        }


        [TestMethod]
        public void DeletProduct()
        {
            string productId = "product_5cdf0c46784b4e98b326f426bb6c2c81";
            int count_v1 = Client.GetProductsAsync().Result.Count;
            var task = Client.DeleteProductAsync(productId);
            task.Wait();
            int count_v2 = Client.GetProductsAsync().Result.Count;

            Assert.AreEqual(count_v1 - 1, count_v2);
        }

        [TestMethod]
        public void ProductCollection()
        {
            EntityCollection<Product> products = Client.GetProductsAsync().Result;
            Assert.IsNotNull(products);
        }


        [TestMethod]
        public void GetProductSubscriptions()
        {
            string productId = "5956a9b92f02d30b88dfad7d";
            EntityCollection<Subscription> collection = Client.GetProductSubscriptionsAsync(productId).Result;
            Assert.IsNotNull(collection);
        }


        [TestMethod]
        public void GetProductAPIs()
        {
            string productId = "5956a9b92f02d30b88dfad7d";
            EntityCollection<API> collection = Client.GetProductAPIsAsync(productId).Result;
            Assert.IsNotNull(collection);
        }
        [TestMethod]
        public void AddProductApi()
        {
            string productId = "5956a9b92f02d30b88dfad7d";
            string apiId = "5956a87a2f02d30b88dfad7b";
            int count_v1 = Client.GetProductAPIsAsync(productId).Result.Count;
            var task = Client.AddProductAPIAsync(productId, apiId);
            task.Wait();
            int count_v2 = Client.GetProductAPIsAsync(productId).Result.Count;

            Assert.AreEqual(count_v1 + 1, count_v2);
        }

        [TestMethod]
        public void DeleteProductApi()
        {
            string productId = "5956a9b92f02d30b88dfad7d";
            string apiId = "5956a87a2f02d30b88dfad7b";
            int count_v1 = Client.GetProductAPIsAsync(productId).Result.Count;
            var task= Client.DeleteProductAPIAsync(productId, apiId);
            task.Wait();
            int count_v2 = Client.GetProductAPIsAsync(productId).Result.Count;

            Assert.AreEqual(count_v1 - 1, count_v2);
        }


        [TestMethod]
        public void GetProductGroups()
        {
            string productId = "5956a9b92f02d30b88dfad7d";
            EntityCollection<Group> collection = Client.GetProductGroupsAsync(productId).Result;
            Assert.IsNotNull(collection);
        }
        [TestMethod]
        public void AddProductGroup()
        {
            string productId = "5956a9b92f02d30b88dfad7d";
            string groupId = "5870184f9898000087020001";
            int count_v1 = Client.GetProductGroupsAsync(productId).Result.Count;
            var task = Client.AddProductGroupAsync(productId, groupId);
            task.Wait();
            int count_v2 = Client.GetProductGroupsAsync(productId).Result.Count;
            Assert.AreEqual(count_v1 + 1, count_v2);
        }
        [TestMethod]
        public void DeleteProductGroup()
        {
            string productId = "5956a9b92f02d30b88dfad7d";
            string groupId = "5870184f9898000087020001";
            int count_v1 = Client.GetProductGroupsAsync(productId).Result.Count;
            var task = Client.DeleteProductGroupAsync(productId, groupId);
            task.Wait();
            int count_v2 = Client.GetProductGroupsAsync(productId).Result.Count;
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
            int c1 = Client.GetSubscriptionsAsync().Result.Count;
            string userId = "user_bef163ba98af433c917914dd4c208115";
            string productId = "5870184f9898000087060002";
            string name = "server subscriptions";
            DateTime now = DateTime.Now;
            SubscriptionDateSettings dateSettings = new SubscriptionDateSettings(now.AddDays(1), now.AddMonths(2));
            Subscription subscription = Subscription.Create(userId, productId, name, dateSettings, SubscriptionState.active);
            Subscription entity = Client.CreateSubscriptionAsync(subscription).Result;
            Assert.IsNotNull(entity);
            Assert.IsNotNull(entity.Id);
            int c2 = Client.GetSubscriptionsAsync().Result.Count;
            Assert.AreEqual(c1 + 1, c2);
        }

        [TestMethod]
        public void DeleteSubscription()
        {
            int c1 = Client.GetSubscriptionsAsync().Result.Count;
            string subscriptionId = "subscription_c0ddc8fd75934e1eb2325ff507908140";
            var task = Client.DeleteSubscriptionAsync(subscriptionId);
            task.Wait();
            int c2 = Client.GetSubscriptionsAsync().Result.Count;
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
            var task = Client.UpdateSubscriptionAsync(subscription);
            task.Wait();
            Subscription entity = Client.GetSubscriptionAsync(subscriptionId).Result;

            Assert.AreEqual(subscription.Name, entity.Name);
        }

        [TestMethod]
        public void GetSubscriptionCollection()
        {
            EntityCollection<Subscription> collection = Client.GetSubscriptionsAsync().Result;
            Assert.IsNotNull(collection);
        }

        [TestMethod]
        public void GetSubscription()
        {
            string subscriptionId = "subscription_72208da5700b45e8a016605ccdc78aa1";
            Subscription subscription = Client.GetSubscriptionAsync(subscriptionId).Result;
            Assert.IsNotNull(subscription);
            Assert.AreEqual(subscriptionId, subscription.Id);
        }


        [TestMethod]
        public void GeneratePrimaryKey()
        {
            string subscriptionId = "5870184f9898000087070001";
            string key_v1 = Client.GetSubscriptionAsync(subscriptionId).Result.PrimaryKey;
            var task = Client.GeneratePrimaryKeyAsync(subscriptionId);
            task.Wait();
            string key_v2 = Client.GetSubscriptionAsync(subscriptionId).Result.PrimaryKey;
            Assert.AreNotEqual(key_v1, key_v2);
        }

        [TestMethod]
        public void GenerateSecondaryKey()
        {
            string subscriptionId = "5870184f9898000087070001";
            string key_v1 = Client.GetSubscriptionAsync(subscriptionId).Result.SecondaryKey;
            var task = Client.GenerateSecondaryKeyAsync(subscriptionId);
            task.Wait();
            string key_v2 = Client.GetSubscriptionAsync(subscriptionId).Result.SecondaryKey;
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
            int count_v1 = Client.GetGroupsAsync().Result.Count;
            string name = "server group 3";
            string description = "this group is created from server";
            Group group = Group.Create(name, description, GroupType.system);
            Group entity = Client.CreateGroupAsync(group).Result;
            Assert.IsNotNull(entity);
            Assert.IsNotNull(entity.Id);
            int count_v2 = Client.GetGroupsAsync().Result.Count;
            Assert.AreEqual(count_v1 + 1, count_v2);
        }

        [TestMethod]
        public void GetGroup()
        {
            string groupId = "group_7ac29362a8c743aaa798b331cc87919e";
            Group group = Client.GetGroupAsync(groupId).Result;
            Assert.IsNotNull(group);
            Assert.AreEqual(groupId, group.Id);
        }

        [TestMethod]
        public void DeleteGroup()
        {
            int count_v1 = Client.GetGroupsAsync().Result.Count;
            string groupId = "5963e39d2f02d312f01a7dcf";
            var task = Client.DeleteGroupAsync(groupId);
            task.Wait();
            int count_v2 = Client.GetGroupsAsync().Result.Count;
            Assert.AreEqual(count_v1 - 1, count_v2);
        }

        [TestMethod]
        public void GroupCollection()
        {
            EntityCollection<Group> collection = Client.GetGroupsAsync().Result;
            Assert.IsNotNull(collection);
        }

        [TestMethod]
        public void GetGroupUsers()
        {
            string groupId = "5870184f9898000087020002";
            EntityCollection<User> collection = Client.GetUsersInGroupAsync(groupId).Result;
            Assert.IsNotNull(collection);
        }


        [TestMethod]
        public void AddUserToGroup()
        {
            string userId = "user_bef163ba98af433c917914dd4c208115";
            string groupId = "group_7ac29362a8c743aaa798b331cc87919e";
            int count_v1 = Client.GetUsersInGroupAsync(groupId).Result.Count;
            var task = Client.AddUserToGroupAsync(groupId, userId);
            task.Wait();
            int count_v2 = Client.GetUsersInGroupAsync(groupId).Result.Count;
            Assert.AreEqual(count_v1 + 1, count_v2);
        }



        [TestMethod]
        public void RemoveUserFromGroup()
        {
            string userId = "user_bef163ba98af433c917914dd4c208115";
            string groupId = "group_7ac29362a8c743aaa798b331cc87919e";
            int count_v1 = Client.GetUsersInGroupAsync(groupId).Result.Count;
            var task = Client.RemoveUserFromGroupAsync(groupId, userId);
            task.Wait();
            int count_v2 = Client.GetUsersInGroupAsync(groupId).Result.Count;
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
            Logger entity = Client.CreateLoggerAsync(logger).Result;
        }

        [TestMethod]
        public void GetLoggers()
        {
            EntityCollection<Logger> loggers = Client.GetLoggersAsync().Result;
        }

        #endregion

    }
}
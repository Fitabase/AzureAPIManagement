using Microsoft.VisualStudio.TestTools.UnitTesting;
using Fitabase.Azure.ApiManagement;
using Fitabase.Azure.ApiManagement.Model;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Generic;
using Fitabase.Azure.ApiManagement.DataModel.Properties;
using Fitabase.Azure.ApiManagement.Filters;
using Fitabase.Azure.ApiManagement.DataModel.Filters;
using Newtonsoft.Json;

namespace Azure.ApiManagement.Test
{
    [TestClass]
    public class ManagementClientTests
    {
        protected ManagementClient Client { get; set; }
		private QueryFilterExpression filter;

        private string _apiId = "api_05247cffcf9d4817adc81663625c18a1";
        private string _userId = "user_bef163ba98af433c917914dd4c208115";
        private string _operationId = "operation_be5ecb981a0d43678ae492502c925047";
        private string _productId = "product_5cdf0c46784b4e98b326f426bb6c2c81";
        private string _groupId = "group_7ac29362a8c743aaa798b331cc87919e";
        private string _subscriptionId = "subscription_c0ddc8fd75934e1eb2325ff507908140";

        [TestInitialize]
        public void SetUp()
        {
            Client = new ManagementClient(@"APIMKeys.json");

			filter = new QueryFilterExpression()
			{
				Skip = 1,
			};
        }

 
        /*********************************************************/
        /************************  USER  *************************/
        /*********************************************************/

        #region User TestCases

        [TestMethod]
        [TestCategory("Create")]
        public void CreateUser()
        {
            int count_v1 = Client.GetUsersAsync().Result.Count;
            string firstName = "Derek";
            string lastName = "Nguyen";
            string email = String.Format("{0}{1}@test.com", firstName, DateTimeOffset.Now.ToUnixTimeMilliseconds());
            string password = "P@ssWo3d";
            User newUser = User.Create(firstName, lastName, email, password, userId:_userId);
            User entity = Client.CreateUserAsync(newUser).Result;
            Assert.IsNotNull(entity);
            Assert.IsNotNull(entity.Id);
            int count_v2 = Client.GetUsersAsync().Result.Count;
            Assert.AreEqual(count_v1 + 1, count_v2);
        }

        [TestMethod]
        [TestCategory("Read")]
        public void GetUserCollection()
        {
            EntityCollection<User> userCollection = Client.GetUsersAsync().Result;
            Assert.IsNotNull(userCollection);
            Assert.AreEqual(userCollection.Count, userCollection.Values.Count);

			// Test with filter
			userCollection = Client.GetUsersAsync(filter).Result;
			Assert.IsNotNull(userCollection);
			Assert.IsTrue(userCollection.Count > userCollection.Values.Count);
		}

	
        [TestMethod]
        [TestCategory("Read")]
        public void GetUser()
        {
            string userId = _userId;
            User user = Client.GetUserAsync(userId).Result;
            Assert.IsNotNull(user);
            Assert.AreEqual(userId, user.Id);
        }

        [TestMethod]
        [TestCategory("Read")]
        public void GetUserSubscriptions()
        {
            string userId = _userId;
            EntityCollection<Subscription> collection = Client.GetUserSubscriptionAsync(userId).Result;
            Assert.IsNotNull(collection);
			Assert.AreEqual(collection.Count, collection.Values.Count);

			// Test With filter
			collection = Client.GetUserSubscriptionAsync(userId, filter).Result;
			Assert.IsNotNull(collection);
			Assert.IsTrue(collection.Count > collection.Values.Count);
		}




		[TestMethod]
        [TestCategory("Read")]
        public void GetUserGroup()
		{
			string userId = _userId;
			EntityCollection<Group> collection = Client.GetUserGroupsAsync(userId).Result;
			Assert.IsNotNull(collection);
			Assert.AreEqual(collection.Count, collection.Values.Count);

			// With Filter
			collection = Client.GetUserGroupsAsync(userId, filter).Result;
			Assert.IsNotNull(collection);
			Assert.IsTrue(collection.Count > collection.Values.Count);
		}

		[TestMethod]
        [TestCategory("Delete")]
        public void DeleteUser()
        {
            int count_v1 = Client.GetUsersAsync().Result.Count;
            string userId = _userId;
            var task = Client.DeleteUserAsync(userId);
            task.Wait();
            int count_v2 = Client.GetUsersAsync().Result.Count;
            Assert.AreEqual(count_v1 - 1, count_v2);
        }

        //[TestMethod]
        //[TestCategory("Delete")]
        //public void DeleteUserWithSubscription()
        //{
        //    int count_v1 = Client.GetUsersAsync().Result.Count;
        //    string userId = "";
        //    var task = Client.DeleteUserAsync(userId);
        //    task.Wait();
        //    int count_v2 = Client.GetUsersAsync().Result.Count;
        //    Assert.AreEqual(count_v1 - 1, count_v2);
        //}

        [TestMethod]
        [TestCategory("Read")]
        public void GetUserSsoURL()
        {
            string userId = _userId;
            SsoUrl user = Client.GenerateSsoURLAsync(userId).Result;
            Assert.IsNotNull(user);
            Assert.IsNotNull(user.Url);
        }


        [TestMethod]
        [TestCategory("Update")]
        public void UpdateUser()
        {
            string userId = _userId;
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
        [TestCategory("Create")]
        public void CreateApi()
        {
            int count_v1 = Client.GetAPIsAsync().Result.Count;
            string name = "Test FitabaseAPI staging";
            string description = "An example to create apis from service";
            string serviceUrl = "fitabase-staging.net";
            string path = "/v1";
            string[] protocols = new string[] { "http", "https" };

            API newAPI = API.Create(name, serviceUrl, path, protocols, description, apiId:_apiId);
            API api = Client.CreateAPIAsync(newAPI).Result;
            Assert.IsNotNull(api.Id);
            int count_v2 = Client.GetAPIsAsync().Result.Count;
            Assert.AreEqual(count_v1 + 1, count_v2);
        }


  //      [TestMethod]
  //      public void GetAPI()
  //      {
  //          //string apiId = "api_7d1d97fd4cce41c09b5d0c703be89d15;rev=2";
		//	string apiId = "api_7d1d97fd4cce41c09b5d0c703be89d15";
		//	string revision = "2";
		//	API api = Client.GetAPIAsync(apiId, revision).Result;
		//	Assert.IsNotNull(api);
		//	Assert.AreEqual(api.ApiRevision, revision);
		//	Assert.AreEqual(api.Id, apiId);
		//	string json = JsonConvert.SerializeObject(api);

		//}
		

		[TestMethod]
        [TestCategory("Update")]
        public void UpdateAPIName()
		{
			string apiId = _apiId;
			string revision = "1";
			API api = Client.GetAPIAsync(apiId, revision).Result;
			//api.ApiRevision = "2";
			//api.IsCurrent = false;

			api.Name = "other name";
			api.IsCurrent = false;
			api.ApiRevision = "2";

			var task = Client.UpdateAPIAsync(api);
			api = Client.GetAPIAsync(apiId).Result;
			string json = JsonConvert.SerializeObject(api);
		}


        [TestMethod]
        [TestCategory("Read")]
        public void ApiCollection()
        {
            EntityCollection<API> collection = Client.GetAPIsAsync().Result;
            Assert.IsNotNull(collection);
            Assert.IsTrue(collection.Count > 0);
			string json = JsonConvert.SerializeObject(collection);

		}


		[TestMethod]
        [TestCategory("Read")]
        public void TestApiCollection_FilterByApiName()
		{
			QueryFilterExpression filter = new QueryFilterExpression()
			{
				Filter = new OperationFilterExpression(OperationOption.GT, new QueryKeyValuePair(QueryableConstants.Api.Id, "api_08e0084f4fe243aeb6ae04452b3fa4c3"))
			};

			EntityCollection<API> collection = Client.GetAPIsAsync(filter).Result;
			Assert.IsNotNull(collection);
		}


		[TestMethod]
        [TestCategory("Read")]
        public void TestApiCollection_FilterByApiPath()
		{
			QueryFilterExpression filter = new QueryFilterExpression()
			{
				Filter = new FunctionFilterExpression(FunctionOption.CONTAINS, new QueryKeyValuePair(QueryableConstants.Api.Name, "(Staging)"))
			};

			EntityCollection<API> collection = Client.GetAPIsAsync(filter).Result;
			Assert.IsNotNull(collection);

			string json = JsonConvert.SerializeObject(collection);
		}



		[TestMethod]
        [TestCategory("Read")]
        public void TestApiCollection_FilterByApiServiceUrl()
		{
			QueryFilterExpression filter = new QueryFilterExpression()
			{
				Filter = new FunctionFilterExpression(FunctionOption.CONTAINS, new QueryKeyValuePair("path", "v1")),
				Skip = 1
			};

			EntityCollection<API> collection = Client.GetAPIsAsync(filter).Result;
			Assert.IsNotNull(collection);
		}

		[TestMethod]
        [TestCategory("Delete")]
        public void DeleteAPI()
        {
            int count_v1 = Client.GetAPIsAsync().Result.Count;
            string apiId = _apiId;
            var task = Client.DeleteAPIAsync(apiId);
            task.Wait();
            int count_v2 = Client.GetAPIsAsync().Result.Count;
            Assert.AreEqual(count_v1 - 1, count_v2);
        }


        [TestMethod]
        [TestCategory("Update")]
        public void UpdateAPI()
        {
            string apiId = _apiId;
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
        [TestCategory("Create")]
        public void CreateAPIOperation()
        {
            long c1, c2;
            string apiId = _apiId;

            string name = "Operation_3";
            RequestMethod method = RequestMethod.DELETE;
            string urlTemplate = "/pet/{petId}";
            string description = "post it";

            APIOperation operation = APIOperation.Create(name, method, urlTemplate, Parameters(), Request(), Responses(), description, _operationId);

            c1 = Client.GetOperationsByAPIAsync(apiId).Result.Count;
            APIOperation entity = Client.CreateAPIOperationAsync(apiId, operation).Result;
            c2 = Client.GetOperationsByAPIAsync(apiId).Result.Count;
            Assert.AreEqual(c1 + 1, c2);         
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
        [TestCategory("Update")]
        public void UpdateAPIOperation()
        {
            string apiId = _apiId;
            string operationId = _operationId;
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
        [TestCategory("Update")]
        public void UpdateOperationParameter()
        {
            string apiId = _apiId;
            string operationId = _operationId;

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
        [TestCategory("Update")]
        public void UpdateOperationResponse()
        {
            string apiId = _apiId;
            string operationId = _operationId;
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
        [TestCategory("Read")]
        public void GetAPIOperation()
        {
            string apiId = _apiId;
            string operationId = _operationId;
            APIOperation operation = Client.GetAPIOperationAsync(apiId, operationId).Result;
            Assert.IsNotNull(operation);
        }

		[TestMethod]
        [TestCategory("Read")]
        public void GetOperationsByAPI()
		{
			string apiId = _apiId;
			EntityCollection<APIOperation> collection = Client.GetOperationsByAPIAsync(apiId).Result;
			Assert.IsNotNull(collection);
			Assert.AreEqual(collection.Count, collection.Values.Count);
			
		}



		[TestMethod]
        [TestCategory("Read")]
        public void TestApiOperationCollectionWithFilter()
		{
			QueryFilterExpression filter = new QueryFilterExpression()
			{
				Filter = new OperationFilterExpression(OperationOption.EQ, new QueryKeyValuePair(QueryableConstants.Operation.Method, "post")),
				Skip = 1
			};

			string apiId = _apiId;
			EntityCollection<APIOperation> collection = Client.GetOperationsByAPIAsync(apiId, filter).Result;
		}

		[TestMethod]
        [TestCategory("Delete")]
        public void DelteAPIOperation()
        {
            string apiId = _apiId;
            string operationId = _operationId;
            int count_v1 = Client.GetOperationsByAPIAsync(apiId).Result.Count;

            var task = Client.DeleteOperationAsync(apiId, operationId);
            task.Wait();
            int count_v2 = Client.GetOperationsByAPIAsync(apiId).Result.Count;
            Assert.AreEqual(count_v1 - 1, count_v2);
        }


        [TestMethod]
        [TestCategory("Update")]
        public void DeleteOperationResponse()
        {
            string apiId = _apiId;
            string operationId = _operationId;
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
        [TestCategory("Create")]
        public void CreateProduct()
        {
            int count_v1 = Client.GetProductsAsync().Result.Count;
            Product product = Product.Create("new Server product", "This product is created from the server", ProductState.published, productId:_productId);
            Product entity = Client.CreateProductAsync(product).Result;
            Assert.IsNotNull(entity);
            Assert.IsNotNull(entity.Id);
            int count_v2 = Client.GetProductsAsync().Result.Count;
            Assert.AreEqual(count_v1 + 1, count_v2);
        }
        

        [TestMethod]
        [TestCategory("Read")]
        public void GetProduct()
        {
            string productId = _productId;
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
        [TestCategory("Update")]
        public void UpdateProduct()
        {
            string productId = _productId;
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
        [TestCategory("Delete")]
        public void DeleteProduct()
        {
            string productId = _productId;
            int count_v1 = Client.GetProductsAsync().Result.Count;
            var task = Client.DeleteProductAsync(productId);
            task.Wait();
            int count_v2 = Client.GetProductsAsync().Result.Count;

            Assert.AreEqual(count_v1 - 1, count_v2);
        }

        [TestMethod]
        [TestCategory("Read")]
        public void ProductCollection()
        {
            EntityCollection<Product> collection = Client.GetProductsAsync().Result;
            Assert.IsNotNull(collection);
			Assert.AreEqual(collection.Count, collection.Values.Count);

			// With filter
			collection = Client.GetProductsAsync(filter).Result;
			Assert.IsTrue(collection.Count > collection.Values.Count);

		}


        [TestMethod]
        [TestCategory("Read")]
        public void GetProductSubscriptions()
        {
            string productId = _productId;
            EntityCollection<Subscription> collection = Client.GetProductSubscriptionsAsync(productId).Result;
            Assert.IsNotNull(collection);
			Assert.AreEqual(collection.Count, collection.Values.Count);

			// With filter
			collection = Client.GetProductSubscriptionsAsync(productId, filter).Result;
			Assert.IsTrue(collection.Count > collection.Values.Count);
		}


        [TestMethod]
        [TestCategory("Read")]
        public void GetProductAPIs()
        {
            string productId = _productId;
            EntityCollection<API> collection = Client.GetProductAPIsAsync(productId).Result;
            Assert.IsNotNull(collection);
			Assert.AreEqual(collection.Count, collection.Values.Count);

			// With filter
			collection = Client.GetProductAPIsAsync(productId, filter).Result;
			Assert.IsTrue(collection.Count > collection.Values.Count);
		}
        [TestMethod]
        [TestCategory("Init")]
        public void AddProductApi()
        {
            string productId = _productId;
            string apiId = _apiId;
            int count_v1 = Client.GetProductAPIsAsync(productId).Result.Count;
            var task = Client.AddProductAPIAsync(productId, apiId);
            task.Wait();
            int count_v2 = Client.GetProductAPIsAsync(productId).Result.Count;

            Assert.AreEqual(count_v1 + 1, count_v2);
        }

        [TestMethod]
        [TestCategory("Delete")]
        public void DeleteProductApi()
        {
            string productId = _productId;
            string apiId = _apiId;
            int count_v1 = Client.GetProductAPIsAsync(productId).Result.Count;
            var task= Client.DeleteProductAPIAsync(productId, apiId);
            task.Wait();
            int count_v2 = Client.GetProductAPIsAsync(productId).Result.Count;

            Assert.AreEqual(count_v1 - 1, count_v2);
        }


        [TestMethod]
        [TestCategory("Read")]
        public void GetProductGroups()
        {
            string productId = _productId;
            EntityCollection<Group> collection = Client.GetProductGroupsAsync(productId).Result;
            Assert.IsNotNull(collection);
			Assert.AreEqual(collection.Count, collection.Values.Count);

			// With Filter
			collection = Client.GetProductGroupsAsync(productId, filter).Result;
			Assert.IsTrue(collection.Count > collection.Values.Count);
		}
        [TestMethod]
        [TestCategory("Init")]
        public void AddProductGroup()
        {
            string productId = _productId;
            string groupId = _groupId;
            int count_v1 = Client.GetProductGroupsAsync(productId).Result.Count;
            var task = Client.AddProductGroupAsync(productId, groupId);
            task.Wait();
            int count_v2 = Client.GetProductGroupsAsync(productId).Result.Count;
            Assert.AreEqual(count_v1 + 1, count_v2);
        }
        [TestMethod]
        [TestCategory("Delete")]
        public void DeleteProductGroup()
        {
            string productId = _productId;
            string groupId = _groupId;
            int count_v1 = Client.GetProductGroupsAsync(productId).Result.Count;
            var task = Client.DeleteProductGroupAsync(productId, groupId);
            task.Wait();
            int count_v2 = Client.GetProductGroupsAsync(productId).Result.Count;
            Assert.AreEqual(count_v1 - 1, count_v2);
        }

        [TestMethod]
        [TestCategory("Read")]
        public void GetProductPolicy()
        {
            string productId = _productId;
            var o = Client.GetProductPolicyAsync(productId);
        }
        
        [TestMethod]
        [TestCategory("Read")]
        public void CheckProductPolicy()
        {
            string productId = _productId;
            var a = Client.CheckProductPolicyAsync(productId);
        }

        [TestMethod]
        [TestCategory("Init")]
        public void SetProductPolicy()
        {
            string productId = _productId;
            string policy = "";
            var b = Client.SetProductPolicyAsync(productId, policy);
        }

        [TestMethod]
        [TestCategory("Delete")]
        public void DeleteProductPolicy()
        {
            string productId = _productId;
            var b = Client.DeleteProductPolicyAsync(productId);
        }


        #endregion Product TestCases





        



        /*********************************************************/
        /**********************  SUBSCRIPTION  *******************/
        /*********************************************************/

        #region Subscription TestCases

        [TestMethod]
        [TestCategory("Create")]
        public void CreateSubscription()
        {
            int c1 = Client.GetSubscriptionsAsync().Result.Count;
            string userId = _userId;
            string productId = _productId;
            string name = "server subscriptions";
            DateTime now = DateTime.Now;
            SubscriptionDateSettings dateSettings = new SubscriptionDateSettings(now.AddDays(1), now.AddMonths(2));
            Subscription subscription = Subscription.Create(userId, productId, name, dateSettings, SubscriptionState.active, subscriptionId:_subscriptionId);
            Subscription entity = Client.CreateSubscriptionAsync(subscription).Result;
            Assert.IsNotNull(entity);
            Assert.IsNotNull(entity.Id);
            int c2 = Client.GetSubscriptionsAsync().Result.Count;
            Assert.AreEqual(c1 + 1, c2);
        }

        [TestMethod]
        [TestCategory("Delete")]
        public void DeleteSubscription()
        {
            int c1 = Client.GetSubscriptionsAsync().Result.Count;
            string subscriptionId = _subscriptionId;
            var task = Client.DeleteSubscriptionAsync(subscriptionId);
            task.Wait();
            int c2 = Client.GetSubscriptionsAsync().Result.Count;
            Assert.AreEqual(c1 - 1, c2);
        }


        [TestMethod]
        [TestCategory("Update")]
        public void UpdateSubscription()
        {
            string subscriptionId = _subscriptionId;
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
        [TestCategory("Read")]
        public void GetSubscriptionCollection()
        {
            EntityCollection<Subscription> collection = Client.GetSubscriptionsAsync().Result;
            Assert.IsNotNull(collection);
			Assert.AreEqual(collection.Count, collection.Values.Count);

			// With Filter
			collection = Client.GetSubscriptionsAsync(filter).Result;
			Assert.IsTrue(collection.Count > collection.Values.Count);
		}

        [TestMethod]
        [TestCategory("Read")]
        public void GetSubscription()
        {
            string subscriptionId = _subscriptionId;
            Subscription subscription = Client.GetSubscriptionAsync(subscriptionId).Result;
            Assert.IsNotNull(subscription);
            Assert.AreEqual(subscriptionId, subscription.Id);
        }


        [TestMethod]
        [TestCategory("Update")]
        public void GeneratePrimaryKey()
        {
            string subscriptionId = _subscriptionId;
            string key_v1 = Client.GetSubscriptionAsync(subscriptionId).Result.PrimaryKey;
            var task = Client.GeneratePrimaryKeyAsync(subscriptionId);
            task.Wait();
            string key_v2 = Client.GetSubscriptionAsync(subscriptionId).Result.PrimaryKey;
            Assert.AreNotEqual(key_v1, key_v2);
        }

        [TestMethod]
        [TestCategory("Update")]
        public void GenerateSecondaryKey()
        {
            string subscriptionId = _subscriptionId;
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
        [TestCategory("Create")]
        public void CreateGroup()
        {
            int count_v1 = Client.GetGroupsAsync().Result.Count;
            string name = "server group 3";
            string description = "this group is created from server";
            Group group = Group.Create(name, description, GroupType.custom, groupId:_groupId);
            Group entity = Client.CreateGroupAsync(group).Result;
            Assert.IsNotNull(entity);
            Assert.IsNotNull(entity.Id);
            int count_v2 = Client.GetGroupsAsync().Result.Count;
            Assert.AreEqual(count_v1 + 1, count_v2);
        }

        [TestMethod]
        [TestCategory("Read")]
        public void GetGroup()
        {
            string groupId = _groupId;
            Group group = Client.GetGroupAsync(groupId).Result;
            Assert.IsNotNull(group);
            Assert.AreEqual(groupId, group.Id);
        }

        [TestMethod]
        [TestCategory("Delete")]
        public void DeleteGroup()
        {
            int count_v1 = Client.GetGroupsAsync().Result.Count;
            string groupId = _groupId;
            var task = Client.DeleteGroupAsync(groupId);
            task.Wait();
            int count_v2 = Client.GetGroupsAsync().Result.Count;
            Assert.AreEqual(count_v1 - 1, count_v2);
        }

        [TestMethod]
        [TestCategory("Read")]
        public void GroupCollection()
        {
            EntityCollection<Group> collection = Client.GetGroupsAsync().Result;
            Assert.IsNotNull(collection);
			Assert.AreEqual(collection.Count, collection.Values.Count);

			// With filter
			collection = Client.GetGroupsAsync(filter).Result;
			Assert.IsTrue(collection.Count > collection.Values.Count);
		}

        [TestMethod]
        [TestCategory("Read")]
        public void GetGroupUsers()
        {
            string groupId = _groupId;
            EntityCollection<User> collection = Client.GetUsersInGroupAsync(groupId).Result;
            Assert.IsNotNull(collection);
			Assert.AreEqual(collection.Count, collection.Values.Count);

			// With filter
			collection = Client.GetUsersInGroupAsync(groupId, filter).Result;
			Assert.IsTrue(collection.Count > collection.Values.Count);
		}


        [TestMethod]
        [TestCategory("Init")]
        public void AddUserToGroup()
        {
            string userId = _userId;
            string groupId = _groupId;
            int count_v1 = Client.GetUsersInGroupAsync(groupId).Result.Count;
            var task = Client.AddUserToGroupAsync(groupId, userId);
            task.Wait();
            int count_v2 = Client.GetUsersInGroupAsync(groupId).Result.Count;
            Assert.AreEqual(count_v1 + 1, count_v2);
        }



        [TestMethod]
        [TestCategory("Delete")]
        public void RemoveUserFromGroup()
        {
            string userId = _userId;
            string groupId = _groupId;
            int count_v1 = Client.GetUsersInGroupAsync(groupId).Result.Count;
            var task = Client.RemoveUserFromGroupAsync(groupId, userId);
            task.Wait();
            int count_v2 = Client.GetUsersInGroupAsync(groupId).Result.Count;
            Assert.AreEqual(count_v1 - 1, count_v2);
        }


        #endregion GroupTestCases



        

    }
}
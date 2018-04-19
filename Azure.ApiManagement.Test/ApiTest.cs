using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Fitabase.Azure.ApiManagement.Model;
using Fitabase.Azure.ApiManagement.DataModel.Properties;
using Newtonsoft.Json;

namespace Azure.ApiManagement.Test
{
	[TestClass]
	public class ApiTest : ManagementClientTests
	{
        private string _apiId1 = "api_9a47ab93edc2407bba0d792c4c93e392";
        private string _operationId1 = "operation_fbe4209ab5d2413da5feaea6447499fa";

        [TestMethod]
        [TestCategory("Create")]
		public void TestCreateAPI()
		{
			string name = "Other Revision";
			string serviceUrl = "a-afitabase-staging.net";
			string path = "v11z";
			string[] protocols = { "https", "http" };
			API api = API.Create(name, serviceUrl, path, protocols, apiId:_apiId1);

			API entity = Client.CreateAPIAsync(api).Result;
        }


		[TestMethod]
        [TestCategory("Read")]
        public void GetAPI()
		{
			string apiId = _apiId1;
			API api = Client.GetAPIAsync(apiId).Result;
			Assert.IsNotNull(api);
			Assert.AreEqual(api.Id, apiId);
			string json = JsonConvert.SerializeObject(api);

		}

		[TestMethod]
        [TestCategory("Read")]
        public void GetAPIRevisions()
		{
			string apiId = _apiId1;

			EntityCollection<APIRevision> revisions = Client.GetApiRevisions(apiId).Result;
		}




		[TestMethod]
        [TestCategory("Update")]
        public void UpdateApiWithRevision()
		{
			string apiId = _apiId1;
			string revision = null;
			API api = Client.GetAPIAsync(apiId, revision).Result;
			api.Name = "Test 2 revisions";
			var task = Client.UpdateAPIAsync(api);
			api = Client.GetAPIAsync(apiId, revision).Result;
		}



		[TestMethod]
        [TestCategory("Read")]
        public void GetApiOperations()
		{
			string apiId = _apiId1;

			EntityCollection<APIRevision> revisions = Client.GetApiRevisions(apiId).Result;
			foreach (APIRevision revision in revisions.Values)
			{

				EntityCollection<APIOperation> collections = Client.GetOperationsByAPIAsync(revision.RevisionId).Result;
				string json = JsonConvert.SerializeObject(collections);
			}
		}




		[TestMethod]
        [TestCategory("Create")]
        public void TestCreateAPIOperation()
		{
			long c1, c2;
			string apiId = _apiId1;

			RequestMethod method = RequestMethod.DELETE;
			string name = method.ToString() + " Calc operation";
			string urlTemplate = "/calc/" + method.ToString();
			string description = "";

			APIOperation operation = APIOperation.Create(name, method, urlTemplate, null, null, null, description, _operationId1);

			c1 = Client.GetOperationsByAPIAsync(apiId).Result.Count;
			APIOperation entity = Client.CreateAPIOperationAsync(apiId, operation).Result;
			c2 = Client.GetOperationsByAPIAsync(apiId).Result.Count;
			Assert.AreEqual(c1 + 1, c2);
        }



		[TestMethod]
        [TestCategory("Read")]
        public void GetOperations()
		{
			string apiId = _apiId1;
			EntityCollection<APIOperation> collections = Client.GetOperationsByAPIAsync(apiId).Result;
		}



		[TestMethod]
        [TestCategory("Read")]
        public void TestGetOperation()
		{
			string apiId = _apiId1;
			string operationId = _operationId1;
			APIOperation operation = Client.GetAPIOperationAsync(apiId, operationId).Result;
			string json = JsonConvert.SerializeObject(operation);
		}


		[TestMethod]
        [TestCategory("Update")]
		public void TestUpdateOperation()
		{
			string apiId_rev1 = _apiId1;
			string operationId = _operationId1;
			APIOperation operation = Client.GetAPIOperationAsync(apiId_rev1, operationId).Result;
			operation.Name = "zzzz";
			var task = Client.UpdateAPIOperationAsync(apiId_rev1, operationId, operation);
			task.Wait();
			APIOperation o1 = Client.GetAPIOperationAsync(apiId_rev1, operationId).Result;

			Assert.AreEqual(o1.Name, operation.Name);

			string json = JsonConvert.SerializeObject(o1);
		}


		[TestMethod]
        [TestCategory("Delete")]
        public void TestDeleteOperation()
		{
			string apiId_rev1 = _apiId1;
			string operationId = _operationId1;
			APIOperation operation = Client.GetAPIOperationAsync(apiId_rev1, operationId).Result;

            var listBefore = Client.GetOperationsByAPIAsync(apiId_rev1).Result;
            var task = Client.DeleteOperationAsync(apiId_rev1, operationId);
			task.Wait();
			var listAfter = Client.GetOperationsByAPIAsync(apiId_rev1).Result;

			Assert.AreEqual(listBefore.Count - 1, listAfter.Count);
        }


        [TestMethod]
        [TestCategory("Delete")]
        public async Task TestDeleteAPI()
        {
            int count_v1 = Client.GetAPIsAsync().Result.Count;

            await Client.DeleteAPIAsync(_apiId1);

            int count_v2 = Client.GetAPIsAsync().Result.Count;
            Assert.AreEqual(count_v1 - 1, count_v2);
        }
    }
}

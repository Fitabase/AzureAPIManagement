using Microsoft.VisualStudio.TestTools.UnitTesting;
using Fitabase.Azure.ApiManagement;
using Fitabase.Azure.ApiManagement.Model;
using System;
using System.Linq;
using System.Collections.Generic;
using Fitabase.Azure.ApiManagement.DataModel.Properties;
using Fitabase.Azure.ApiManagement.Filters;
using Fitabase.Azure.ApiManagement.DataModel.Filters;
using Newtonsoft.Json;
using Fitabase.Azure.ApiManagement.DataModel.Model;

namespace Azure.ApiManagement.Test
{
	[TestClass]
	public class ApiTest : ManagementClientTests
	{
		[TestMethod]
		public void CreateAPI()
		{
			string name = "Other Revision";
			string serviceUrl = "a-afitabase-staging.net";
			string path = "v11z";
			string[] protocols = { "https", "http" };
			API api = API.Create(name, serviceUrl, path, protocols);

			API entity = Client.CreateAPIAsync(api).Result;
		}


		[TestMethod]
		public void GetAPI()
		{
			//string apiId = "api_7d1d97fd4cce41c09b5d0c703be89d15;rev=2";
			string apiId = "api_9a47ab93edc2407bba0d792c4c93e392";
			API api = Client.GetAPIAsync(apiId).Result;
			Assert.IsNotNull(api);
			Assert.AreEqual(api.Id, apiId);
			string json = JsonConvert.SerializeObject(api);

		}

		[TestMethod]
		public void GetAPIRevisions()
		{
			string apiId = "api_7d1d97fd4cce41c09b5d0c703be89d15";

			EntityCollection<APIRevision> revisions = Client.GetApiRevisions(apiId).Result;
		}




		[TestMethod]
		public void UpdateApiWithRevision()
		{
			string apiId = "api_7d1d97fd4cce41c09b5d0c703be89d15";
			string revision = null;
			API api = Client.GetAPIAsync(apiId, revision).Result;
			api.Name = "Test 2 revisions";
			var task = Client.UpdateAPIAsync(api);
			api = Client.GetAPIAsync(apiId, revision).Result;
		}



		[TestMethod]
		public void GetApiOperations()
		{
			string apiId = "api_7d1d97fd4cce41c09b5d0c703be89d15";

			EntityCollection<APIRevision> revisions = Client.GetApiRevisions(apiId).Result;
			foreach (APIRevision revision in revisions.Values)
			{

				EntityCollection<APIOperation> collections = Client.GetOperationsByAPIAsync(revision.RevisionId).Result;
				string json = JsonConvert.SerializeObject(collections);
			}
		}




		[TestMethod]
		public void TestAPIOperationLifeCycle()
		{
			long c1, c2;
			string apiId = "api_1de6f9ae1521400594ee3e234ffd630c;rev=3";

			RequestMethod method = RequestMethod.DELETE;
			string name = method.ToString() + " Calc operation";
			string urlTemplate = "/calc/" + method.ToString();
			string description = "";

			APIOperation operation = APIOperation.Create(name, method, urlTemplate, null, null, null, description);


			#region CREATE

			c1 = Client.GetOperationsByAPIAsync(apiId).Result.Count;
			APIOperation entity = Client.CreateAPIOperationAsync(apiId, operation).Result;
			c2 = Client.GetOperationsByAPIAsync(apiId).Result.Count;
			Assert.AreEqual(c1 + 1, c2);

			#endregion

		}



		[TestMethod]
		public void GetOperations()
		{
			string apiId = "api_1de6f9ae1521400594ee3e234ffd630c;rev=3";
			EntityCollection<APIOperation> collections = Client.GetOperationsByAPIAsync(apiId).Result;
		}



		[TestMethod]
		public void TestGetOperation()
		{
			string apiId = "api_7d1d97fd4cce41c09b5d0c703be89d15;rev=3";
			string operationId = "operation_fbe4209ab5d2413da5feaea6447499fa";
			APIOperation operation = Client.GetAPIOperationAsync(apiId, operationId).Result;
			string json = JsonConvert.SerializeObject(operation);
		}


		[TestMethod]
		public void TestUpdateOperation()
		{
			string apiId_rev1 = "api_7d1d97fd4cce41c09b5d0c703be89d15;rev=1";
			string apiId_rev3 = "api_7d1d97fd4cce41c09b5d0c703be89d15;rev=3";
			string operationId = "operation_fbe4209ab5d2413da5feaea6447499fa";
			APIOperation operation = Client.GetAPIOperationAsync(apiId_rev1, operationId).Result;
			operation.Name = "zzzz";
			var task = Client.UpdateAPIOperationAsync(apiId_rev1, operationId, operation);
			task.Wait();
			APIOperation o1 = Client.GetAPIOperationAsync(apiId_rev1, operationId).Result;
			APIOperation o3 = Client.GetAPIOperationAsync(apiId_rev3, operationId).Result;

			Assert.AreEqual(o1.Name, o3.Name);

			string json = JsonConvert.SerializeObject(o1);
		}


		[TestMethod]
		public void TestDeleteOperaion()
		{
			string apiId_rev1 = "api_7d1d97fd4cce41c09b5d0c703be89d15;rev=1";
			string apiId_rev3 = "api_7d1d97fd4cce41c09b5d0c703be89d15;rev=3";
			string operationId = "operation_fbe4209ab5d2413da5feaea6447499fa";
			APIOperation operation = Client.GetAPIOperationAsync(apiId_rev1, operationId).Result;
			var task = Client.DeleteOperationAsync(apiId_rev1, operationId);
			task.Wait();
			var list = Client.GetOperationsByAPIAsync(apiId_rev1).Result;
			var list3 = Client.GetOperationsByAPIAsync(apiId_rev3).Result;


			Assert.AreEqual(list.Count + 1, list3.Count);
		}


		[TestMethod]
		public void TestUpdate()
		{
			string apiId_rev1 = "api_7d1d97fd4cce41c09b5d0c703be89d15;rev=1";
			string apiId_rev3 = "api_7d1d97fd4cce41c09b5d0c703be89d15;rev=3";
			string operationId = "operation_fbe4209ab5d2413da5feaea6447499fa";
			APIOperation operation = Client.GetAPIOperationAsync(apiId_rev3, operationId).Result;
			operation.Id = operationId;
			var task = Client.CreateAPIOperationAsync(apiId_rev1, operation);
			task.Wait();

			var list = Client.GetOperationsByAPIAsync(apiId_rev1).Result;
			var list3 = Client.GetOperationsByAPIAsync(apiId_rev3).Result;

			Assert.AreEqual(list.Count, list3.Count);

		}
	}
}

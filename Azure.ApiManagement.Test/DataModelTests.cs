using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Fitabase.Azure.ApiManagement;
using Fitabase.Azure.ApiManagement.Model;

namespace Azure.ApiManagement.Test
{
    [TestClass]
    public class DataModelTests
    {
        [TestMethod]
        public void DeserilizeProducts()
        {
            var content = @"{
                              ""value"": [
                                {
                                  ""id"": ""/products/53e10f187e888000b4060001"",
                                  ""name"": ""Starter"",
                                  ""description"": ""Subscribers will be able to run 5 calls/minute up to a maximum of 100 calls/week."",
                                  ""terms"": """",
                                  ""subscriptionPeriod"": {
                                                ""value"": 15,
                                    ""interval"": ""day""
                                  },
                                  ""notificationPeriod"": {
                                                ""value"": 12,
                                    ""interval"": ""day""
                                  },
                                  ""subscriptionRequired"": true,
                                  ""approvalRequired"": false,
                                  ""subscriptionsLimit"": null,
                                  ""state"": ""published""
                                },
                                {
                                  ""id"": ""/products/53e10f187e888000b4060002"",
                                  ""name"": ""Unlimited"",
                                  ""description"": ""Subscribers have completely unlimited access to the API. Administrator approval is required."",
                                  ""subscriptionRequired"": true,
                                  ""approvalRequired"": true,
                                  ""subscriptionsLimit"": null,
                                  ""state"": ""published""
                                }
                              ],
                              ""count"": 2,
                              ""nextLink"": null
                            }";

            var result = Utility.DeserializeToJson<EntityCollection<Product>>(content);

            Assert.AreEqual("53e10f187e888000b4060001", result.Values[0].Id);
            Assert.AreEqual("53e10f187e888000b4060002", result.Values[1].Id);
            Assert.AreEqual("Starter", result.Values[0].Name);
            Assert.AreEqual(null, result.Values[0].Groups);

        }

        [TestMethod]
        public void DeserilizeProductsWithGroups()
        {
            var content = @"{
                              ""value"": [
                                {
                                  ""id"": ""/products/544d9a6d0fe876031c060001"",
                                  ""name"": ""Starter"",
                                  ""description"": ""Subscribers will be able to run 5 calls/minute up to a maximum of 100 calls/week."",
                                  ""terms"": """",
                                  ""subscriptionRequired"": true,
                                  ""approvalRequired"": false,
                                  ""subscriptionsLimit"": null,
                                  ""state"": ""published"",
                                  ""groups"": [
                                    {
                                      ""id"": ""/groups/544d9a6d0fe876031c020001"",
                                      ""name"": ""Administrators"",
                                      ""description"": ""Administrators is a built-in group. Its membership is managed by the system. Microsoft Azure subscription administrators fall into this group."",
                                      ""builtIn"": true
                                    },
                                    {
                                      ""id"": ""/groups/544d9a6d0fe876031c020002"",
                                      ""name"": ""Developers"",
                                      ""description"": ""Developers is a built-in group. Its membership is managed by the system. Signed-in users fall into this group."",
                                      ""builtIn"": true
                                    },
                                    {
                                      ""id"": ""/groups/544d9a6d0fe876031c020003"",
                                      ""name"": ""Guests"",
                                      ""description"": ""Guests is a built-in group. Its membership is managed by the system. Unauthenticated users visiting the developer portal fall into this group."",
                                      ""builtIn"": true
                                    }
                                  ]
                                },
                                {
                                  ""id"": ""/products/544d9a6d0fe876031c060002"",
                                  ""name"": ""Unlimited"",
                                  ""description"": ""Subscribers have completely unlimited access to the API. Administrator approval is required."",
                                  ""terms"": null,
                                  ""subscriptionRequired"": true,
                                  ""approvalRequired"": true,
                                  ""subscriptionsLimit"": null,
                                  ""state"": ""published"",
                                  ""groups"": [
                                    {
                                      ""id"": ""/groups/544d9a6d0fe876031c020001"",
                                      ""name"": ""Administrators"",
                                      ""description"": ""Administrators is a built-in group. Its membership is managed by the system. Microsoft Azure subscription administrators fall into this group."",
                                      ""builtIn"": true
                                    },
                                    {
                                      ""id"": ""/groups/544d9a6d0fe876031c020002"",
                                      ""name"": ""Developers"",
                                      ""description"": ""Developers is a built-in group. Its membership is managed by the system. Signed-in users fall into this group."",
                                      ""builtIn"": true
                                    },
                                    {
                                      ""id"": ""/groups/544d9a6d0fe876031c020003"",
                                      ""name"": ""Guests"",
                                      ""description"": ""Guests is a built-in group. Its membership is managed by the system. Unauthenticated users visiting the developer portal fall into this group."",
                                      ""builtIn"": true
                                    }
                                  ]
                                }
                              ],
                              ""count"": 2,
                              ""nextLink"": null
                            }";

            var result = Utility.DeserializeToJson<EntityCollection<Product>>(content);

            Assert.AreEqual("544d9a6d0fe876031c020001", result.Values[0].Groups[0].Id);
            Assert.AreEqual("544d9a6d0fe876031c020003", result.Values[0].Groups[2].Id);

            Assert.AreEqual("Administrators", result.Values[0].Groups[0].Name);
            Assert.AreEqual(true, result.Values[0].Groups[0].BuiltIn);
        }

        [TestMethod]
        public void DeserializeProduct()
        {
            var content = @"{
                              ""id"": ""/products/53e10f187e888000b4060002"",
                              ""name"": ""Unlimited"",
                              ""description"": ""Subscribers have completely unlimited access to the API. Administrator approval is required."",
                              ""subscriptionRequired"": true,
                              ""approvalRequired"": true,
                              ""subscriptionsLimit"": null,
                              ""state"": ""published""
                            }";

            var result = Utility.DeserializeToJson<Product>(content);

            Assert.AreEqual("53e10f187e888000b4060002", result.Id);
            Assert.AreEqual(ProductState.published, result.State);
            Assert.AreEqual(true, result.ApprovalRequired);
        }

        [TestMethod]
        public void DeserializeAPIs()
        {
            var content = @"{
                              ""value"": [
                                {
                                  ""id"": ""/apis/54ff046ff0be6b0c94ccfb2f"",
                                  ""name"": ""Basic Calculator"",
                                  ""description"": ""Arithmetics is just a call away!"",
                                  ""serviceUrl"": ""http://calcapi.cloudapp.net/api"",
                                  ""path"": ""calc"",
                                  ""protocols"": [
                                    ""https""
                                  ]
                                },
                                {
                                  ""id"": ""/apis/5480c157f0be6b10ac965a76"",
                                  ""name"": ""Contoso API"",
                                  ""description"": null,
                                  ""serviceUrl"": ""http://api.contoso.com"",
                                  ""path"": ""contosoapi"",
                                  ""protocols"": [
                                    ""https""
                                  ]
                                },
                                {
                                  ""id"": ""/apis/53c765632095310385040001"",
                                  ""name"": ""Echo API"",
                                  ""description"": null,
                                  ""serviceUrl"": ""http://echoapi.cloudapp.net/api"",
                                  ""path"": ""echo"",
                                  ""protocols"": [
                                    ""https""
                                  ]
                                },
                                {
                                  ""id"": ""/apis/54d913208ad1c9037c46876a"",
                                  ""name"": ""Salesforce REST API"",
                                  ""description"": ""OAuth demo service."",
                                  ""serviceUrl"": ""https://na17.salesforce.com/services/data/v20.0"",
                                  ""path"": ""salesforce"",
                                  ""protocols"": [
                                    ""https""
                                  ]
                                }
                              ],
                              ""count"": 4,
                              ""nextLink"": null
                            }";

            var result = Utility.DeserializeToJson<EntityCollection<API>>(content);

            Assert.AreEqual("54ff046ff0be6b0c94ccfb2f", result.Values[0].Id);
            Assert.AreEqual("Basic Calculator", result.Values[0].Name);
            Assert.AreEqual("https", result.Values[0].Protocols[0]);

            Assert.AreEqual("53c765632095310385040001", result.Values[2].Id);
        }

        [TestMethod]
        public void DeserializeAPIWihtoutExport()
        {
            var content = @"{
                              ""id"": ""/apis/53c765632095310385040001"",
                              ""name"": ""Echo API"",
                              ""description"": null,
                              ""serviceUrl"": ""http://echoapi.cloudapp.net/api"",
                              ""path"": ""echo"",
                              ""protocols"": [
                                ""https""
                              ],
                              ""authenticationSettings"": {
                                ""oAuth2"": null
                              },
                              ""subscriptionKeyParameterNames"": {
                                ""header"": ""Ocp-Apim-Subscription-Key"",
                                ""query"": ""subscription-key""
                              }
                             }";

            var result = Utility.DeserializeToJson<API>(content);

            Assert.AreEqual("53c765632095310385040001", result.Id);
            Assert.AreEqual("http://echoapi.cloudapp.net/api", result.ServiceUrl);
            Assert.AreEqual("echo", result.Path);

            Assert.AreEqual("https", result.Protocols[0]);
        }

    }
}

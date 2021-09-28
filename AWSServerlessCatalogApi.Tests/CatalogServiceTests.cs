using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Xunit;
using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;
using Amazon.Lambda.APIGatewayEvents;

using Newtonsoft.Json;

using AWSServerlessCatalogApi;
using AWSServerlessCatalogApi.Models;
using Moq;
using AWSServerlessCatalogApi.Controllers;
using Amazon.S3;
using AWSManagement.API.Models;
using AWSServerlessCatalogApi.Repository;
using Microsoft.Extensions.Options;
using Amazon.S3.Model;
using AWSServerlessCatalogApi.Services;
using System.Text;
using Nancy.Json;

namespace AWSServerlessCatalogApi.Tests
{
    public class CatalogServiceTests
    {
        private readonly Mock<IAwsManagementS3Repository> _awsmgmtrepoMock = new Mock<IAwsManagementS3Repository>();
        CatalogService _sut;
        private readonly Catalog obj = new Catalog();
        public CatalogServiceTests()
        {
            _sut = new CatalogService(_awsmgmtrepoMock.Object);
        }
        [Fact]
        public async Task GetByIdAsync_ShouldReturnCatalog_WhenCatalogExists()
        {
            var catalogid = 1;
            GetObjectResponse response = new GetObjectResponse();
            var responseStream = new MemoryStream();
            var jsonBytes = Encoding.UTF8.GetBytes("{\"id\":\"1\",\"name\":\"flight\"}");
            responseStream.Write(jsonBytes, 0, jsonBytes.Length);
            responseStream.Seek(0, SeekOrigin.Begin);
            response.ResponseStream = responseStream;
            Catalog catalogMock = new Catalog
            {
                id = catalogid.ToString(),
                name = "flight"
            };
            _awsmgmtrepoMock.Setup(x => x.GetFile(catalogid)).ReturnsAsync(response);
            var catalog = await _sut.GetFile_service(catalogid);
            Assert.True(catalogMock.Equals(catalog));

        }
        [Fact]
        public async Task GetAsync_ShouldReturnCatalogs_WhenCatalogExists()
        {
            ListObjectsV2Response response = new ListObjectsV2Response();
            S3Object S1 = new S3Object
            {
                Key = "1.json"
            };
            S3Object S2 = new S3Object
            {
                Key = "2.json"
            };
            S3Object S3 = new S3Object
            {
                Key = "3.json"
            };
            response.S3Objects.Add(S1);
            response.S3Objects.Add(S2);
            response.S3Objects.Add(S3);
            _awsmgmtrepoMock.Setup(x => x.GetFiles()).ReturnsAsync(response);
            List<Catalog> catalogList = new List<Catalog>()
                                                      {
                                                          new Catalog
                                                          {
                                                            id = "1",
                                                            name = "Flight"
                                                           },
                                                           new Catalog
                                                           {
                                                             id = "2",
                                                             name = "hotel"
                                                            },
                                                           new Catalog
                                                           {
                                                            id = "3",
                                                            name = "car"
                                                            }
                                                     };
        
            foreach (S3Object entry in response.S3Objects)
            {
                if (entry.Key.Contains(".json"))
                {
                    GetObjectResponse response1 = new GetObjectResponse();
                    var responseStream = new MemoryStream();
                    var key = entry.Key.Split(".")[0];
                    int catalogid = Convert.ToInt32(entry.Key.Split(".")[0]);
                    var obj = catalogList.Where(x => x.id == key).FirstOrDefault();

                    JavaScriptSerializer js = new JavaScriptSerializer();
                    string blogObject = js.Serialize(obj);

                    
                    var jsonBytes = Encoding.UTF8.GetBytes(blogObject);
                    responseStream.Write(jsonBytes, 0, jsonBytes.Length);
                    responseStream.Seek(0, SeekOrigin.Begin);
                    response1.ResponseStream = responseStream;
                    _awsmgmtrepoMock.Setup(x => x.GetFile(catalogid)).ReturnsAsync(response1);
                    //var catalog = await _sut.GetFile_service(catalogid);
                    // lst.Add(catalog);
                }
            }
            _awsmgmtrepoMock.Setup(x => x.GetFiles()).ReturnsAsync(response);
           
            var catalogs = await _sut.GetFiles_Service();
            Assert.Equal(catalogList, catalogs);

        }

        [Fact]
        public async Task DeleteByIdAsync_ShouldReturnCatalog_WhenCatalogExists()
        {
            var catalogid = 3 +".json";
            bool result = false;
            DeleteObjectResponse response = new DeleteObjectResponse();
            response.HttpStatusCode = System.Net.HttpStatusCode.OK;
            _awsmgmtrepoMock.Setup(x => x.DeleteFile(catalogid)).ReturnsAsync(response);
            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                result = true;
            else
                result = false;
            var catalog = await _sut.DeleteFile_service(catalogid);
            Assert.True(result.Equals(catalog));

        }
        [Fact]
        public async Task UploadAsync_ShouldDataUpload()
        {
            var catalogid = 3 + ".json";
            bool result = false;
            DeleteObjectResponse response = new DeleteObjectResponse();
            response.HttpStatusCode = System.Net.HttpStatusCode.OK;
            _awsmgmtrepoMock.Setup(x => x.DeleteFile(catalogid)).ReturnsAsync(response);
            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                result = true;
            else
                result = false;
            var catalog = await _sut.DeleteFile_service(catalogid);
            Assert.True(result.Equals(catalog));

        }
    }
}

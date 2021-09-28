using Amazon.S3;
using AWSManagement.API.Models;
using AWSServerlessCatalogApi.Models;
//using Microsoft.IdentityModel.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3.Model;
using Amazon;
using System.IO;
using Nancy.Json;

namespace AWSServerlessCatalogApi.Repository
{
    public class AWSManagementS3Repository : IAwsManagementS3Repository
    {
        private readonly IAmazonS3 _amazonS3;
        private readonly ServiceConfiguration _settings;
        public AWSManagementS3Repository(IAmazonS3 amazons3, IOptions<ServiceConfiguration> settings)
        {
            this._settings = settings.Value;
            this._amazonS3 = new AmazonS3Client(this._settings.AWSS3.AccessKey, this._settings.AWSS3.SecretKey, RegionEndpoint.USEast1);
        }
        public async Task<DeleteObjectResponse> DeleteFile(string key)
        {
            DeleteObjectResponse response = new DeleteObjectResponse();
            key = key + ".json";
            response = await this._amazonS3.DeleteObjectAsync(this._settings.AWSS3.BucketName, key);
            return response;
        }
        public async Task<ListObjectsV2Response> GetFiles()
        {
            ListObjectsV2Request request = new ListObjectsV2Request
            {
                BucketName = this._settings.AWSS3.BucketName,
                MaxKeys = 10
            };
            ListObjectsV2Response response = await this._amazonS3.ListObjectsV2Async(request);
            return response;
        }
        public async Task<GetObjectResponse> GetFile(int key)
        {
            
            var response = await this._amazonS3.GetObjectAsync(this._settings.AWSS3.BucketName, key + ".json".ToString());
            return response;
        }
        public async Task<bool> UploadFile(Catalog obj)
        {
            PutObjectResponse response = null;
            try
            {
                var path = Path.Combine("Files", obj.id + ".json");
                string fileExtension = Path.GetExtension(path);
                string fileName = string.Empty;
                fileName = $"{obj.id}{fileExtension}";
                JavaScriptSerializer js = new JavaScriptSerializer();
                PutObjectRequest request = new PutObjectRequest()
                {
                    BucketName = this._settings.AWSS3.BucketName,
                    Key = fileName,
                    ContentType = "application/json",
                    ContentBody = js.Serialize(obj)
                };
                response = await this._amazonS3.PutObjectAsync(request);
            }
            catch (Exception ex)
            {

            }
            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                return true;
            else
                return false;
        }
    }
}

using Amazon.S3.Model;
using AWSManagement.API.Models;
using AWSServerlessCatalogApi.Models;
using Nancy.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AWSServerlessCatalogApi.Services
{
    public class CatalogService
    {
        IAwsManagementS3Repository _awsmgmtRepository;
        public CatalogService(IAwsManagementS3Repository awsmgmtRepository) {
            this._awsmgmtRepository = awsmgmtRepository;
        }
        public async Task<Catalog> GetFile_service(int key)
        {
            var response = await this._awsmgmtRepository.GetFile(key);
            string responseBody = string.Empty;
            Catalog blogObject;
            using (Stream responseStream = response.ResponseStream)
            using (StreamReader reader = new StreamReader(responseStream))
            {
                string contentType = response.Headers["Content-Type"];
                responseBody = reader.ReadToEnd();
                JavaScriptSerializer js = new JavaScriptSerializer();
                blogObject = js.Deserialize<Catalog>(responseBody);
            }
            return blogObject;
        }

        public async Task<List<Catalog>> GetFiles_Service()
        {
            var response = await this._awsmgmtRepository.GetFiles();
            List<Catalog> lst = new List<Catalog>();
            foreach (S3Object entry in response.S3Objects)
            {
                if (entry.Key.Contains(".json"))
                {
                    var key = entry.Key.Split(".")[0];
                    var response_Catalog = await GetFile_service(Convert.ToInt32(key));
                    lst.Add(response_Catalog);
                }
            }
            return lst;
        }
        public async Task<bool> DeleteFile_service(string key)
        {
            var response = await this._awsmgmtRepository.DeleteFile(key);
            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                return true;
            else
                return false;

        }
        public async Task<bool> UploadFile_service(Catalog obj)
        {
            var response = await this._awsmgmtRepository.UploadFile(obj);
            //if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                return true;
            //else
                //return false;

        }

    }
}

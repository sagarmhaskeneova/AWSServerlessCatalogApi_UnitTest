using Amazon.S3.Model;
using AWSManagement.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AWSServerlessCatalogApi.Models
{
    public interface IAwsManagementS3Repository
    {
        //ask<List<Catalog>> GetFiles();
        //Task<Catalog> GetFile(int id);
        Task<ListObjectsV2Response> GetFiles();
        Task<GetObjectResponse> GetFile(int id);
        Task<bool> UploadFile(Catalog obj);
        Task<DeleteObjectResponse> DeleteFile(string key);
    }
}

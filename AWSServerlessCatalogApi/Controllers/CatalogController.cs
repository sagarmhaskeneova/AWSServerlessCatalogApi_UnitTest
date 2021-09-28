using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using AWSManagement.API.Models;
using AWSServerlessCatalogApi.Models;
using AWSServerlessCatalogApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nancy.Json;

namespace AWSServerlessCatalogApi.Controllers
{
    [Route("api/[controller]")]
    public class CatalogController : ControllerBase
    {
        //IAmazonS3 S3Client { get; set; }
        IAwsManagementS3Repository AwsRepo;
        CatalogService _awsServices;
        public CatalogController(IAwsManagementS3Repository awsRepo,CatalogService awsServices) 
        {
            this._awsServices = awsServices;
            this.AwsRepo = awsRepo;
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                List<Catalog> catalogList = await this._awsServices.GetFiles_Service();
                return Ok(catalogList);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error in retrieving data from file");
            }

        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                Catalog catalog = await this._awsServices.GetFile_service(id);
                return Ok(catalog);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error in retrieving data from file");
            }
        }
        // POST api/values
        //[HttpPost]
        //public async Task<bool> Post([FromBody] Catalog obj)
        //{
        //    PutObjectResponse response = null;
        //    try
        //    {
        //        var path = Path.Combine("Files", obj.id + ".json");
        //        //var path = @"C:\Users\sagar\source\repos\File\" + obj.id + ".json";
        //        //string json = new JavaScriptSerializer().Serialize(obj);
        //        //write string to file
        //        // System.IO.File.WriteAllText(path, json);
        //        //using (FileStream fsSource = new FileStream(path, FileMode.Open, FileAccess.ReadWrite))
        //        //{
        //        string fileExtension = Path.GetExtension(path);
        //        string fileName = string.Empty;
        //        fileName = $"{obj.id}{fileExtension}";
        //        PutObjectRequest request = new PutObjectRequest()
        //        {
        //            //InputStream = fsSource,
        //            BucketName = "myworkbtk",
        //            Key = fileName,
        //            ContentType = "application/json",
        //            ContentBody = JsonSerializer.Serialize(obj)
        //        };
        //        response = await this.S3Client.PutObjectAsync(request);
        //        //}
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
        //        return true;
        //    else
        //        return false;
        //}
        //[HttpPost]
        //public async Task<bool> Post([FromBody] Catalog obj)
        //{
        //    bool response= await this.AwsRepo.UploadFile(obj);
        //    return response;

        //}
        [HttpPost]
        public async Task<bool> Post([FromBody] Catalog obj)
        {
            bool response = await this._awsServices.UploadFile_service(obj);
            return response;
        }

        [HttpDelete("{key}")]
        public async Task<IActionResult> Delete(string key)
        {
            try
            {
                key = key + ".json";
                bool response = await this._awsServices.DeleteFile_service(key);
                return Ok(true);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error in retrieving data from file"); throw;
            }
        }
    }
}

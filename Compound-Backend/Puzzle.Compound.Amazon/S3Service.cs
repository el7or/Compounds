using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Puzzle.Compound.Amazon
{
    public interface IS3Service
    {
        string UploadFile(string folderName, string fileName, byte[] fileBytes, bool? sameFileName = false);
        string UploadFile(string folderName, string fileName, byte[] fileBytes, out string newFileName, bool? sameFileName = false);
    }
    public class S3Service : IS3Service
    {
        private readonly IConfiguration configuration;

        public S3Service(IConfiguration configuration)
        {
            this.configuration = configuration;

        }
        public string UploadFile(string folderName, string fileName, byte[] fileBytes, out string newFileName, bool? sameFileName = false)
        {
            return UploadS3File(folderName, fileName, fileBytes, out newFileName);
        }

        public string UploadFile(string folderName, string fileName, byte[] fileBytes, bool? sameFileName = false)
        {
            return UploadS3File(folderName, fileName, fileBytes, out string newfileName, sameFileName);
        }

        private string UploadS3File(string folderName, string fileName, byte[] fileBytes, out string newFileName, bool? sameFileName = false)
        {
            try
            {
                string fileExtension = Path.GetExtension(fileName);

                string keyName = sameFileName != true ? $"{Guid.NewGuid()}{fileExtension}" : keyName = fileName;

                string accessKey = configuration.GetSection("AWSS3:accessKey").Value;
                string secretKey = configuration.GetSection("AWSS3:secretKey").Value;
                string bucketName = configuration.GetSection("AWSS3:bucketName").Value;

                var credentials = new BasicAWSCredentials(accessKey, secretKey);
                var config = new AmazonS3Config
                {
                    RegionEndpoint = RegionEndpoint.USEast1
                };

                using var client = new AmazonS3Client(credentials, config);
                TransferUtility fileTransferUtility = new TransferUtility(client);

                var ms = new MemoryStream(fileBytes);

                var uploadRequest = new TransferUtilityUploadRequest
                {
                    Key = $"{folderName}/" + keyName,
                    InputStream = ms,
                    BucketName = bucketName,
                    CannedACL = S3CannedACL.PublicRead
                };

                fileTransferUtility.Upload(uploadRequest);

                string url = configuration.GetSection("AWSS3:mainUrl").Value;
                newFileName = uploadRequest.Key;
                return $"{url}{uploadRequest.Key}";
            }
            catch (AmazonS3Exception ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

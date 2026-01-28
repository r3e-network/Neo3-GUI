using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Neo.Services
{
    /// <summary>
    /// Service for uploading logos to NeoFS via filesend.ngd.network
    /// </summary>
    public class LogoFsService
    {
        private const string UploadEndpoint = "https://filesend.ngd.network/gate/upload/CeeroywT8ppGE4HGjhpzocJkdb2yu3wD5qCGFTjkw1Cc";
        private static readonly HttpClient _httpClient = new HttpClient();

        /// <summary>
        /// Upload response model
        /// </summary>
        public class UploadResponse
        {
            public string ObjectId { get; set; }
            public string ContainerId { get; set; }
        }

        /// <summary>
        /// Upload a logo file to NeoFS and return the object ID
        /// </summary>
        /// <param name="filePath">Path to the logo file</param>
        /// <returns>NeoFS object ID</returns>
        public static async Task<string> UploadLogo(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentException("File path cannot be null or empty", nameof(filePath));
            }

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Logo file not found", filePath);
            }

            var fileInfo = new FileInfo(filePath);
            
            // Check file size (max 50MB)
            if (fileInfo.Length > 50 * 1024 * 1024)
            {
                throw new ArgumentException("File size exceeds 50MB limit");
            }

            using var content = new MultipartFormDataContent();
            using var fileStream = File.OpenRead(filePath);
            using var streamContent = new StreamContent(fileStream);
            
            content.Add(streamContent, "file", fileInfo.Name);

            var response = await _httpClient.PostAsync(UploadEndpoint, content);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<UploadResponse>(responseBody, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
            });

            return result?.ObjectId ?? throw new Exception("Failed to get object ID from response");
        }

        /// <summary>
        /// Upload logo from byte array
        /// </summary>
        /// <param name="fileBytes">Logo file bytes</param>
        /// <param name="fileName">File name</param>
        /// <returns>NeoFS object ID</returns>
        public static async Task<string> UploadLogo(byte[] fileBytes, string fileName)
        {
            if (fileBytes == null || fileBytes.Length == 0)
            {
                throw new ArgumentException("File bytes cannot be null or empty", nameof(fileBytes));
            }

            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentException("File name cannot be null or empty", nameof(fileName));
            }

            // Check file size (max 50MB)
            if (fileBytes.Length > 50 * 1024 * 1024)
            {
                throw new ArgumentException("File size exceeds 50MB limit");
            }

            using var content = new MultipartFormDataContent();
            using var byteContent = new ByteArrayContent(fileBytes);
            
            content.Add(byteContent, "file", fileName);

            var response = await _httpClient.PostAsync(UploadEndpoint, content);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<UploadResponse>(responseBody, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
            });

            return result?.ObjectId ?? throw new Exception("Failed to get object ID from response");
        }
    }
}

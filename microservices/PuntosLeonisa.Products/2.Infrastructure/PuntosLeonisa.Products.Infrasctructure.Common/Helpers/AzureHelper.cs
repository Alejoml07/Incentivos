using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace PuntosLeonisa.Products.Infrasctructure.Common;
public class AzureHelper
{



    private const string ContainerName = "$web";
    private readonly string _connectionString;

    public AzureHelper(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<string> UploadFileToBlobAsync(byte[] bytes, string fileExtension, string contentType)
    {
        if (bytes == null || bytes.Length == 0)
        {
            throw new ArgumentNullException(nameof(bytes), "File missing");
        }
        if (string.IsNullOrWhiteSpace(fileExtension))
        {
            throw new ArgumentException("File name missing", nameof(fileExtension));
        }
        if (string.IsNullOrWhiteSpace(contentType))
        {
            throw new ArgumentException("Content type missing", nameof(contentType));
        }

        string blobName = $"/img/{Path.GetRandomFileName()}{fileExtension}";

        BlobServiceClient blobServiceClient = new BlobServiceClient(_connectionString);
        BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(ContainerName);
        BlobClient blobClient = containerClient.GetBlobClient(blobName);

        BlobUploadOptions uploadOptions = new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders { ContentType = contentType }
        };

        using var stream = new MemoryStream(bytes);
        await blobClient.UploadAsync(stream, uploadOptions);
        stream.Close();

        return blobClient.Uri.ToString();
    }
}





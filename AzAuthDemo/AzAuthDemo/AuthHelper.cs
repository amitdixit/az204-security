
using Azure.Identity;
using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Keys.Cryptography;
using Azure.Security.KeyVault.Secrets;
using Azure.Storage.Blobs;
using System.Text;

namespace AzAuthDemo;
internal static class AuthHelper
{
    static string downloadPath = @"E:\\Downloads\\sample.yml";
    internal static void DownloadBlobWithConnectionString()
    {
        var storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=authdemosaakd;AccountKey=iCdfLCrFA45ruU4/sNukO2ZgburxX2qM+AK6DuidJ662NZo+rY2WRpy2/P0gypTJ/CDW3cIRiH3d+ASty6eQ9Q==;EndpointSuffix=core.windows.net";
        string containerName = "data";
        string blobName = "sample.yml";

        BlobServiceClient serviceClient = new BlobServiceClient(storageConnectionString);

        var container = serviceClient.GetBlobContainerClient(containerName);

        var client = container.GetBlobClient(blobName);

        client.DownloadTo(downloadPath);
    }

    internal static void DownloadBlobWithAzureAdAuth()
    {
        string tenantId = "30996a51-c766-47a0-a51a-e86a78533c3c";
        string clientId = "b47e7ea0-50b5-406d-a439-7faece8aa84e";
        string clientSecrect = "GjE8Q~d4KqFTrFB~MZP4gE1fX.O4rJWCls7wscoa";
        string blobUrl = "https://authdemosaakd.blob.core.windows.net/data/sample.yml";

        var clientCredential = new ClientSecretCredential(tenantId, clientId, clientSecrect);
        var blobUri = new Uri(blobUrl);
        var client = new BlobClient(blobUri, clientCredential);
        client.DownloadTo(downloadPath);
    }

    internal static void GetAzureSecretFromVault()
    {
        //Ensure that the Application Object has access to Secrets Permission on the Azure KeyVaults (Read and List)
        string tenantId = "30996a51-c766-47a0-a51a-e86a78533c3c";
        string clientId = "b47e7ea0-50b5-406d-a439-7faece8aa84e";
        string clientSecrect = "GjE8Q~d4KqFTrFB~MZP4gE1fX.O4rJWCls7wscoa";


        string keyVaultUrl = "https://authdemokeyvaultakd.vault.azure.net/";
        string secretName = "dbpassword";

        var clientCredential = new ClientSecretCredential(tenantId, clientId, clientSecrect);

        var secretClient = new SecretClient(new Uri(keyVaultUrl), clientCredential);

        var secret = secretClient.GetSecret(secretName);

        Console.WriteLine($"The value of the secret is {secret.Value.Value}");
    }


    internal static void GetAzureKeysFromVault()
    {
        //Ensure that the Application Object has access to Keys Permission on the Azure KeyVaults (Read,List,Encrypt,Decrypt)
        string tenantId = "30996a51-c766-47a0-a51a-e86a78533c3c";
        string clientId = "b47e7ea0-50b5-406d-a439-7faece8aa84e";
        string clientSecrect = "GjE8Q~d4KqFTrFB~MZP4gE1fX.O4rJWCls7wscoa";


        string keyVaultUrl = "https://authdemokeyvaultakd.vault.azure.net/";
        string keyName = "mySecrectKey";
        string textToEncrypt = "This text needs to be encrypted";

        var clientCredential = new ClientSecretCredential(tenantId, clientId, clientSecrect);

        var keyClient = new KeyClient(new Uri(keyVaultUrl), clientCredential);

        var key = keyClient.GetKey(keyName);

        // The CryptographyClient class is part of the Azure Key vault package
        // This is used to perform cryptographic operations with Azure Key Vault keys
        var cryptoClient = new CryptographyClient(key.Value.Id, clientCredential);

        // We first need to take the bytes of the string that needs to be converted

        byte[] textToBytes = Encoding.UTF8.GetBytes(textToEncrypt);

        EncryptResult result = cryptoClient.Encrypt(EncryptionAlgorithm.RsaOaep, textToBytes);

        Console.WriteLine("The encrypted text");
        Console.WriteLine(Convert.ToBase64String(result.Ciphertext));

        // Now lets decrypt the text
        // We first need to convert our Base 64 string of the Cipertext to bytes

        byte[] ciperToBytes = result.Ciphertext;

        DecryptResult textDecrypted = cryptoClient.Decrypt(EncryptionAlgorithm.RsaOaep, ciperToBytes);

        Console.WriteLine(Encoding.UTF8.GetString(textDecrypted.Plaintext));
    }
}

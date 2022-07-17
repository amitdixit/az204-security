using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Storage.Blobs;
using System.Reflection.PortableExecutable;

namespace ManagedIdentitiesDemo;
internal static class ManagedIdentityHelper
{
    const string downloadPath = @"C:\\tmp\\sample.yml";
    internal static void DownloadBlobWithConnectionString()
    {
        string blobUrl = "https://managedidentitiesdemoakd.blob.core.windows.net/data/sample.yml";

        TokenCredential clientCredential = new DefaultAzureCredential();
        var blobUri = new Uri(blobUrl);

        var client = new BlobClient(blobUri, clientCredential);

        client.DownloadTo(downloadPath);

        Console.WriteLine("File downloaded");

    }


    internal static void GetAzureSecretFromVault()
    {
        //Ensure that the Application Object has access to Secrets Permission on the Azure KeyVaults (Read and List)

        string keyVaultUrl = "https://authdemokeyvaultakd.vault.azure.net/";
        string secretName = "newdemopassword";

        var clientCredential = new DefaultAzureCredential();

        var secretClient = new SecretClient(new Uri(keyVaultUrl), clientCredential);

        var secret = secretClient.GetSecret(secretName);

        Console.WriteLine($"The value of the secret is {secret.Value.Value}");
    }

    internal static async Task GetAccessToken()
    {
        using (var httpClient = new HttpClient())
        {
            httpClient.DefaultRequestHeaders.Add("Metadata", "true");
            var response = await httpClient.GetAsync("http://169.254.169.254/metadata/identity/oauth2/token?api-version=2018-02-01&resource=https://storage.azure.com/");
            var data = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"{data}");

        }
    }

}

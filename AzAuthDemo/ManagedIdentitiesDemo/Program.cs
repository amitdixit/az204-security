// See https://aka.ms/new-console-template for more information
using ManagedIdentitiesDemo;

/*
//ManagedIdentity for Storage Account
//In the Storage account Give Reader and Storage Blob Data Reader access to the VM

//In the VM =>Identity Turn on the Status for the Sytem assigned
ManagedIdentityHelper.DownloadBlobWithConnectionString();
*/

//In the VM =>Identity Turn on the Status for the Sytem assigned
//Add Policy and Give List and Get Access to the VM
//ManagedIdentityHelper.GetAzureSecretFromVault();


await ManagedIdentityHelper.GetAccessToken();
Console.ReadKey();

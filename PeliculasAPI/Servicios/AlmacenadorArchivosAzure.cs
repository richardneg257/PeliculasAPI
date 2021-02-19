using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PeliculasAPI.Servicios
{
    public class AlmacenadorArchivosAzure : IAlmacenadorArchivos
    {
        private readonly string connectionString;

        public AlmacenadorArchivosAzure(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("AzureStorage");
        }

        public async Task BorrarArchivo(string ruta, string contenedor)
        {
            if (ruta != null)
            {
                BlobContainerClient container = new BlobContainerClient(connectionString, contenedor);
                var nombreBlob = Path.GetFileName(ruta);
                await container.DeleteBlobIfExistsAsync(nombreBlob);
            }
        }

        public async Task<string> EditarArchivo(byte[] contenido, string extension, string contenedor, string ruta, string contentType)
        {
            await BorrarArchivo(ruta, contenedor);
            return await GuardarArchivo(contenido, extension, contenedor, contentType);
        }

        public async Task<string> GuardarArchivo(byte[] contenido, string extension, string contenedor, string contentType)
        {
            BlobContainerClient container = new BlobContainerClient(connectionString, contenedor);

            await container.CreateIfNotExistsAsync(PublicAccessType.Blob);
            BlobClient blob = container.GetBlobClient($"{Guid.NewGuid()}{extension}");
            await blob.UploadAsync(new MemoryStream(contenido));

            return blob.Uri.ToString();
        }
    }
}

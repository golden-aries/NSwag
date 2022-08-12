using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json.Linq;
using NSwag;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Hosting
{
    /// <summary>
    /// Extends IFileProvider operations
    /// </summary>
    public static class FileProviderEx
    {
        public static string ReadAllText(this IFileProvider fileProvider, string fileName)
        {
            var fi = fileProvider.GetFileInfo(fileName);
            using var stream = fi.CreateReadStream();
            using var reader = new StreamReader(stream);
            var json = reader.ReadToEnd();
            return json;
        }

        public static async Task<string> ReadAllTextAsync(this IFileProvider fileProvider, string fileName)
        {
            var fi = fileProvider.GetFileInfo(fileName);
            using var stream = fi.CreateReadStream();
            using var reader = new StreamReader(stream);
            var json = await reader.ReadToEndAsync().ConfigureAwait(false);
            return json;
        }

        public static async Task<OpenApiDocument> ParseOpenApiDocument(
            this IFileProvider fileProvider,
            string fileName,
            CancellationToken cancellationToken = default)
        {
            var fi = fileProvider.GetFileInfo(fileName);
            using var stream = fi.CreateReadStream();
            using var reader = new StreamReader(stream);
            var json = await reader.ReadToEndAsync().ConfigureAwait(false);
            var doc = await OpenApiDocument.FromJsonAsync(json, cancellationToken).ConfigureAwait(false);
            return doc;
        }

        public static async Task<JObject> ParseJObjectAsync(
            this IFileProvider fileProvider, string fileName, CancellationToken cancellationToken = default)
        {
            var fi = fileProvider.GetFileInfo(fileName);
            using var stream = fi.CreateReadStream();
            using var reader = new StreamReader(stream);
            var json = await reader.ReadToEndAsync().ConfigureAwait(false);
            var jo = JObject.Parse(json);
            return jo;
        }
    }
}

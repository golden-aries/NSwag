using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using NSwag.Probe;
using NSwag.Probe.Extensions;

var host = Host.CreateDefaultBuilder()
    .UseConsoleLifetime()
    .Build();
var env = host.Services.GetRequiredService<IHostEnvironment>();
var fp = env.ContentRootFileProvider;


var od = await fp.ParseOpenApiDocument("Content/Random_Sentence_Generator_Bus_openapidoc.jsonc");

#region  Exploring_OpenDocument
Console.WriteLine("# Info");
Console.WriteLine("Info.Title: " + od.Info.Title);
Console.WriteLine("Info.Description: " + od.Info.Description);
Console.WriteLine("Info.Version: " + od.Info.Version);
Console.WriteLine("## Paths");
foreach (var path in od.Paths)
{
    Console.WriteLine($"{path} | Key {path.Key} | Value {path.Value}");
}
Console.WriteLine("## Operations");
foreach (var o in od.Operations)
{
    Console.WriteLine("### Operation");
    Console.WriteLine(o.ToString());

    Console.WriteLine($"Path: {o.Path}");
    Console.WriteLine($"Method: {o.Method}");
    Console.WriteLine($"Operation: {o.Operation}");
    Console.WriteLine("#### Parameters");
    foreach (var p in o.Operation.Parameters)
    {
        Console.WriteLine($"- {p}");
        Console.WriteLine($"Name: {p.Name}");
        Console.WriteLine($"Position: {p.Position}");
        Console.WriteLine($"IsRequired: {p.IsRequired}");
        Console.WriteLine($"Kind: {p.Kind}");
        Console.WriteLine($"Type: {p.Schema.Type}");
        Console.WriteLine($"Format: {p.Schema.Format}");
    }
    foreach(var r in o.Operation.Responses)
    {
        Console.WriteLine($"- {r}");
        Console.WriteLine($"Key: {r.Key}");
        Console.WriteLine($"Value: {r.Value}");
        Console.WriteLine($"Type: {r.Value.Schema.Type}");
        Console.WriteLine($"Format: {r.Value.Schema.Format}");
    }
}
#endregion

var operations = od.PrepareInitialSchemas();

var mbfcSdk = await fp.ParseJObjectAsync("Content/mbfc_sdk.schema.jsonc");
mbfcSdk.ExtendMbfcSchemaWithOperations(operations);

//od = await fp.ParseOpenApiDocument("Content/FusionFS_openapidoc.jsonc");
//operations = od.PrepareInitialSchemas();
//mbfcSdk.ExtendMbfcSchemaWithOperations(operations);

// write modified MBFC SDK Schema json to file
var modifiedSdkJson = mbfcSdk.ToString();
var h = fp as PhysicalFileProvider ?? throw new Exception("Unable to cast to PhysicalFileProvider");
File.WriteAllText(Path.Combine(h.Root, "Content", "modified_sdk.schema.jsonc"), modifiedSdkJson);
Console.WriteLine("Done!");

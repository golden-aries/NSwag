using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using NSwag;
using NSwag.Probe;

var host = Host.CreateDefaultBuilder().UseConsoleLifetime().Build();
var env = host.Services.GetRequiredService<IHostEnvironment>();
var fp = env.ContentRootFileProvider;

var fileName = "Content/Random_Sentence_Generator_Bus.jsonc";
var fi = fp.GetFileInfo(fileName);
using var stream = fi.CreateReadStream();
using var reader = new StreamReader(stream);
var json = await reader.ReadToEndAsync();
var od = await OpenApiDocument.FromJsonAsync(json);
Console.WriteLine("# Info");
Console.WriteLine("Info.Title: " + od.Info.Title);
Console.WriteLine("Info.Description: " + od.Info.Description);
Console.WriteLine("Info.Version: " + od.Info.Version);
Console.WriteLine("## Paths");
foreach ( var path in od.Paths)
{
    Console.WriteLine($"{path} | Key {path.Key} | Value {path.Value}");
}
Console.WriteLine("## Operations");
foreach ( var o in od.Operations)
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
}
var s = new JObject();
s[Kwd.Schema] = "https://schemas.botframework.com/schemas/component/v1.0/component.schema";
s[Kwd.Role] = "implements(Mcirosoft.IDialog)";
s[Kwd.Title] = od.Info.Title;
s[Kwd.Description] = od.Info.Description;
s[Kwd.Type] = JSchemaType.Object.ToString().ToLower();

var result = s.ToString();
Console.WriteLine(result);
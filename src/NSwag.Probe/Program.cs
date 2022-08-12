using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using NSwag;
using NSwag.Probe;

var host = Host.CreateDefaultBuilder()
    .UseConsoleLifetime()
    .Build();
var env = host.Services.GetRequiredService<IHostEnvironment>();
var fp = env.ContentRootFileProvider;

var od = await fp.ParseOpenApiDocument("Content/Random_Sentence_Generator_Bus.jsonc");
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
}

Dictionary<string, JObject> operations = new();

foreach (var o in od.Operations)
{
    var j = new JObject();
    j[Kwd.Schema] = "https://schemas.botframework.com/schemas/component/v1.0/component.schema";
    j[Kwd.Role] = MbfcRole.ImlementsMicrosoftIDialog;
    j[Kwd.Title] = MbfcSdkDefs.OperationId(o.Operation);
    j[Kwd.Description] = od.Info.Title;
    j[Kwd.Type] = JType.Object;
    j[Kwd.AdditionalProperties] = false;
    var props = new JObject();
    j[Kwd.Properties] = props;
    foreach (var p in o.Operation.Parameters)
    {
        var par = new JObject
        {
            [Kwd.Ref] = MbfcSdkDefs.GetDefRef(OpenApiDefs.OpenApiTypeToMbfcDefMap[p.Schema.Type]),
            [Kwd.Title] = p.Name
        };
        props.Add(p.Name, par);
    }
    foreach(var r in o.Operation.Responses)
    {
        switch(r.Key)
        {
            case "200":
                var cnt = r.Value.Content.First();
                switch(cnt.Key)
                {
                    case System.Net.Mime.MediaTypeNames.Application.Json:
                        var res = new JObject
                        {
                            [Kwd.Ref] = MbfcSdkDefs.GetDefRef(OpenApiDefs.OpenApiTypeToMbfcDefMap[cnt.Value.Schema.Type]),
                            [Kwd.Title] = "Result"
                        };
                        props.Add("resultProperty", res);
                        break;
                }
                break;
            default:
                //throw new Exception("Unknown response code!");
                break;
        }
    }

    props.Add(Kwd.Path, new JObject
    {
        [Kwd.Type] = JType.String,
        [Kwd.Default] = o.Path
    });

    operations.Add(MbfcSdkDefs.OperationId(o.Operation), j);
    var result = j.ToString();
    Console.WriteLine(result);
}

var mj = await fp.ParseJObjectAsync("Content/mbfc_sdk.schema.jsonc");

foreach (var o in operations)
{
   
    var oneOf = mj[Kwd.OneOf] as JArray ?? throw new Exception($"Unable to cast mbfc sdk property to JArray! Prop {Kwd.OneOf}!");
    oneOf.Add(new JObject
    {
        [Kwd.Ref] = MbfcSdkDefs.GetDef(o.Key)
    }); ;
    var defs = mj[Kwd.Definitions] as JObject ?? throw new Exception($"Unable to cast mbfc sdk property to JObject! Prop {Kwd.Definitions}!");
    
    // extend IDialog definition
    var iDialog = defs[Kwd.MicrosoftIDialog] as JObject ?? throw new Exception($"Unable to cast mbfc sdk definitions property to JObject! Prop: {Kwd.MicrosoftIDialog}!");
    oneOf = iDialog[Kwd.OneOf] as JArray ?? throw new Exception($"Unable to cast mbfc sdk defintions[Microsoft.IDialog] property to JArray! Prop: {Kwd.Definitions}!");
    oneOf.Add(new JObject
    {
        [Kwd.Ref] = MbfcSdkDefs.GetDef(o.Key)
    });

    // define operation
    defs.Add(o.Key, o.Value);
    o.Value[Kwd.Kind] = new JObject()
    {
        [Kwd.Title] = "Kind of dialog object",
        [Kwd.Description] = "Defines the valid properties for the component you are configuring (from a dialog .schema file)",
        [Kwd.Type] = JType.String,
        [Kwd.Pattern] = "^[a-zA-Z][a-zA-Z0-9.]*$",
        [Kwd.Const] = o.Key
    };

    o.Value[Kwd.Designer] = new JObject()
    {
        [Kwd.Title] = "Designer information",
        [Kwd.Type] = JType.Object,
        [Kwd.Description] = "Extra information for the Bot Framework Composer."
    };
}
var modifiedSdkJson = mj.ToString();
Console.WriteLine("Done!");

using Newtonsoft.Json.Linq;

namespace NSwag.Probe.Extensions
{
    /// <summary>
    /// Extensions method to facilitate extending MBFC SDK schema with
    /// operations from OpenApi Documents created by Nswag
    /// </summary>
    public static class OpenApiMbfcEx
    {
        /// <summary>
        /// Iterates through OpenApi document operations and creates intital schemas
        /// which are suitable source schemas for bf cli
        /// </summary>
        /// <param name="od"></param>
        /// <returns></returns>
        public static Dictionary<string, JObject> PrepareInitialSchemas(this OpenApiDocument od)
        {
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
                foreach (var r in o.Operation.Responses)
                {
                    switch (r.Key)
                    {
                        case "200":
                            var cnt = r.Value.Content.First();
                            switch (cnt.Key)
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
                    [Kwd.Const] = o.Path
                });

                operations.Add(MbfcSdkDefs.OperationId(o.Operation), j);
                //var result = j.ToString();
                //Console.WriteLine(result);
            }
            return operations;
        }


        /// <summary>
        /// Will iterate throug operations and modify mbfcSdkSchema JObject to make them available in 
        /// Microsoft Bot Framework Composer.
        /// Returns mbfcSdkSchema JObject to support chaining.
        /// </summary>
        /// <param name="mbfcSdkSchema"></param>
        /// <param name="operations"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static JObject ExtendMbfcSchemaWithOperations(this JObject mbfcSdkSchema, Dictionary<string, JObject> operations)
        {
            foreach (var o in operations)
            {
                // 1. insert operation ref  in oneOf of schema  
                var oneOf = mbfcSdkSchema[Kwd.OneOf] as JArray ?? throw new Exception($"Unable to cast mbfc sdk property to JArray! Prop {Kwd.OneOf}!");
                oneOf.Add(new JObject
                {
                    [Kwd.Ref] = MbfcSdkDefs.GetDef(o.Key)
                });
                var defs = mbfcSdkSchema[Kwd.Definitions] as JObject ?? throw new Exception($"Unable to cast mbfc sdk property to JObject! Prop {Kwd.Definitions}!");

                // 2. extend IDialog definition, insert operation ref in oneOf of IDialg
                var iDialog = defs[Kwd.MicrosoftIDialog] as JObject ?? throw new Exception($"Unable to cast mbfc sdk definitions property to JObject! Prop: {Kwd.MicrosoftIDialog}!");
                oneOf = iDialog[Kwd.OneOf] as JArray ?? throw new Exception($"Unable to cast mbfc sdk defintions[Microsoft.IDialog] property to JArray! Prop: {Kwd.Definitions}!");

                //  insert ref to operation before pattern definition
                oneOf.Insert(
                    oneOf.FindIndxOfPattern(),
                    new JObject
                    {
                        [Kwd.Ref] = MbfcSdkDefs.GetDef(o.Key)
                    });

                // 3. define operation,  add operation definition in definitions JArray
                defs.Add(o.Key, o.Value);
                o.Value.Remove(Kwd.Schema);
                o.Value[Kwd.Required] = new JArray
                {
                    new JValue(Kwd.Kind)
                };

                o.Value[Kwd.PatternProperties] = new JObject
                {
                    [@"^\$"] = new JObject
                    {
                        [Kwd.Title] = "Tooling property",
                        [Kwd.Description] = "Open ended property for tooling."

                    }
                };

                var props = o.Value[Kwd.Properties] as JObject ?? throw new Exception("Unable to cast properties to JObject");
                props[Kwd.Kind] = new JObject()
                {
                    [Kwd.Title] = "Kind of dialog object",
                    [Kwd.Description] = "Defines the valid properties for the component you are configuring (from a dialog .schema file)",
                    [Kwd.Type] = JType.String,
                    [Kwd.Pattern] = "^[a-zA-Z][a-zA-Z0-9.]*$",
                    [Kwd.Const] = o.Key
                };

                props[Kwd.Designer] = new JObject()
                {
                    [Kwd.Title] = "Designer information",
                    [Kwd.Type] = JType.Object,
                    [Kwd.Description] = "Extra information for the Bot Framework Composer."
                };
            }
            return mbfcSdkSchema;
        }
    }
}

using NJsonSchema;

namespace NSwag.Probe
{
    /// <summary>
    /// Contains OpenApiTypes and map to Microsoft Bot Framework SDK Defintions
    /// </summary>
    public static class OpenApiDefs
    {
        public static string Integer { get; set; } = "integer";

        public static IDictionary<JsonObjectType, string> OpenApiTypeToMbfcDefMap { get; set; } = new Dictionary<JsonObjectType, string>
        {
            { JsonObjectType.Integer,  MbfcSdkDefs.IntegerExpression },
            { JsonObjectType.String, MbfcSdkDefs.StringExpression },
            { JsonObjectType.Boolean, MbfcSdkDefs.BooleanExpression }
        };
    }
}

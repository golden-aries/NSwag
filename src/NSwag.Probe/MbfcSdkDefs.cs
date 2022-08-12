using NJsonSchema;

namespace NSwag.Probe
{
    /// <summary>
    /// Contains Mbfc Sdk Defintions Refs
    /// </summary>
    public static class MbfcSdkDefs
    {
        /// <summary>
        /// IntegerExpression
        /// </summary>
        public static string IntegerExpression { get; set; } = "integerExpression";
        public static string StringExpression { get; set; } = "stringExpression";

        /// <summary>
        /// extends keyword as "schema:#/definitions/" + definition
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public static string GetDefRef(string keyword)
        {
            return "schema:#/definitions/" + keyword;
        }

        public static string GetDefRef(JsonObjectType jsonObjectType)
        {
            return GetDefRef(OpenApiDefs.OpenApiTypeToMbfcDefMap[jsonObjectType]);
        }

        /// <summary>
        /// extends keyword as "#/definitions/" + keyword
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public static string GetDef(string keyword)
        {
            return "#/definitions/" + keyword;
        }

        public static string OperationId(OpenApiOperation op)
        {
            var indx = op.OperationId.LastIndexOf('_');
            return op.OperationId.Substring(indx+1);
        }
    }
}

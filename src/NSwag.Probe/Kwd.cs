using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSwag.Probe
{
    /// <summary>
    /// Kwd is short for Keyword
    /// Class contains possible keywords for the JSON schema
    /// </summary>
    public static class Kwd
    {
        public static string Schema { get; set; } = "$schema";
        public static string Role { get; set; } = "$role";

        public static string Title { get; set; } = "title";

        public static string Description { get; set; } = "description";

        public static string Type { get; set; } = "type";

        public static string AdditionalProperties { get; set; } = "additionalProperties";

        public static string Properties { get; set; } = "properties";

        public static string Ref { get; set; } = "$ref";

        public static string OneOf { get; set; } = "oneOf";

        public static string Definitions { get; set; } = "definitions";

        public static string MicrosoftIDialog { get; set; } = "Microsoft.IDialog";

        public static string Kind { get; set; } = "$kind";

        public static string Pattern { get; set; } = "pattern";

        public static string Const { get; set; } = "const";

        public static string Designer { get; set; } = "designer";

        public static string Path { get; set; } = "path";

        public static string Default { get; set; } = "default";

    }
}

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
    }
}

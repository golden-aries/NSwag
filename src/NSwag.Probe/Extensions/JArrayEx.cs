using Newtonsoft.Json.Linq;

namespace NSwag.Probe.Extensions
{
    public static class JArrayEx
    {
        /// <summary>
        /// finds JObject wich has "pattern" property
        /// throws exception if not found
        /// </summary>
        /// <param name="jArray"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static JObject FindJoPattern(this JArray jArray)
        {
            var pttrn = jArray.FirstOrDefault(jt =>
            {
                if (jt is JObject jto)
                {
                    var pt = jto[Kwd.Pattern];
                    if (pt is not null)
                    {
                        return true;
                    }
                }
                return false;
            });

            return pttrn as JObject ?? throw new Exception("Unable to locate ");
        }

        /// <summary>
        /// finds index JObject wich has "pattern" property
        /// throws exception if not found
        /// </summary>
        /// <param name="jArray"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static int FindIndxOfPattern(this JArray jArray)
        {
            var pttrn = jArray.FindJoPattern();
            var indx = jArray.IndexOf(pttrn);
            return indx;
        }
    }
}

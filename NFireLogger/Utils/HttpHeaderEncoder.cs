using System;
using System.Text;
using System.Web.Script.Serialization;

namespace NFireLogger.Utils
{
    internal static class HttpHeaderEncoder
    {
        /// <summary>
        /// Size of one header (see PHP implementation)
        /// </summary>
        private const int HEADER_CHUNK_SIZE = 76; // 


        /// <summary>
        /// Serialize object according to FireLogger spec.
        /// </summary>
        /// <param name="o">object to be serialized</param>
        /// <returns>chunks of serialized object</returns>
        public static string[] EncodeObjectIntoChunks(object o)
        {
            return EncodeObject(o).SplitToArray(HEADER_CHUNK_SIZE);
        }


        /// <summary>
        /// Encode object into FireLogger serialization format (JSON in Base64)
        /// </summary>
        /// <param name="o">object for encoding</param>
        /// <returns>encoded string</returns>
        public static string EncodeObject(object o)
        {
            return ConverToBase64(Jsonify(o));
        }


        /// <summary>
        /// Serialize object to string of JSON format 
        /// </summary>
        /// <param name="o">object to serialization</param>
        /// <returns>JSON string</returns>
        static string Jsonify(object o)
        {
            return new JavaScriptSerializer().Serialize(o);
        }


        /// <summary>
        /// Convert string saftey (utf-8) in to BASE64 coding 
        /// </summary>
        /// <param name="value">string for conversion</param>
        /// <returns>Base64 string</returns>
        static string ConverToBase64(string value)
        {
            // TODO according to documentation, text could be in utf-8 but it's not, where is problem? 
            
            // EncodeNonAsciiCharacters simulate PHP implementation of firelogger server 
            // (in fact php function json_encode do this internally with every string)

            var sanitized = value.EncodeNonAsciiCharacters();

            // ColdFusion implementation throw away every non ascii characters (replace with '?')
            // comment in CF implementation: firebug/logger/json parser seems to choke on high ascii characters
            // (see https://github.com/mpaperno/CF-FireLogger/blob/master/us/wdg/cf/firelogger.cfc#L948 )
            
            var bytes = Encoding.UTF8.GetBytes(sanitized);
            return Convert.ToBase64String(bytes);
        }
    }
}

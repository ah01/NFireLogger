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
        /// 
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
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
            // TODO acordnig to documentation, text could be directly in utf-8 but it's not, where is problem? 
            // (EncodeNonAsciiCharacters simulate PHP implementation of firelogger server)
            var sanitized = value.EncodeNonAsciiCharacters();

            var bytes = Encoding.UTF8.GetBytes(sanitized);

            return Convert.ToBase64String(bytes);
        }
    }
}

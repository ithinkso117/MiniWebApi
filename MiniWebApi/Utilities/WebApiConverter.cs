using System;
using System.Globalization;
using MiniWebApi.Log;
using Newtonsoft.Json;

namespace MiniWebApi.Utilities
{
    static class WebApiConverter
    {
        /// <summary>
        /// Convert an object to Json string
        /// </summary>
        /// <param name="obj">object</param>
        /// <returns></returns>
        public static string ObjectToJson(object obj)
        {
            try
            {
                if (obj != null)
                {
                    return JsonConvert.SerializeObject(obj);
                }
            }
            catch(Exception ex)
            {
                Logger.Write($"Convert object <{obj}> to json error: {ex}");
            }
            return string.Empty;
        }


        /// <summary>
        /// Convert a Json string to object
        /// </summary>
        /// <param name="jsonStr">Json string</param>
        /// <param name="type">object type</param>
        /// <returns></returns>
        public static object JsonToObject(string jsonStr, Type type)
        {
            try
            {
                if (!string.IsNullOrEmpty(jsonStr) && !string.IsNullOrWhiteSpace(jsonStr))
                {
                    return JsonConvert.DeserializeObject(jsonStr, type);
                }
            }
            catch (Exception ex)
            {
                Logger.Write($"Convert json [{jsonStr}] to object <{type}> error: {ex}");
            }

            return type.Default();
        }


        /// <summary>
        /// Convert a string to an object.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object StringToObject(string str, Type type)
        {
            try
            {
                return Convert.ChangeType(str, type, CultureInfo.InvariantCulture);
            }
            catch(Exception ex)
            {
                Logger.Write($"Convert string [{str}] to object <{type}> error: {ex}");
            }
            return type.Default();
        }
    }
}

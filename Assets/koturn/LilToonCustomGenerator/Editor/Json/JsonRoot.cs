using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace Koturn.LilToonCustomGenerator.Json
{
    [Serializable]
    internal class JsonRoot
    {
        /// <summary>
        /// <see cref="List{T}"/> of <see cref="ProxyConfig"/> instance.
        /// </summary>
        public List<TemplateConfig> configList;

        /// <summary>
        /// Create instance from specified json file.
        /// </summary>
        /// <param name="filePath">File path to json file.</param>
        /// <returns>Created <see cref="JsonRoot"/> instance.</returns>
        /// <exception cref="FileNotFoundException">Thrown when specified file is not found.</exception>
        /// <exception cref="ArgumentNullException">Thrown when instance is not created.</exception>
        public static JsonRoot LoadFromJsonFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("File not found: " + filePath);
            }
            // var jsonRoot = JsonConvert.DeserializeObject<JsonRoot>(File.ReadAllText(filePath));
            // if (jsonRoot == null)
            // {
            //     throw new NullReferenceException("jsonRoot");
            // }
            // return jsonRoot;

            return JsonUtility.FromJson<JsonRoot>(File.ReadAllText(filePath));
        }
    }
}

using System;
using System.Collections.Generic;


namespace Koturn.LilToonCustomGenerator.Json
{
    [Serializable]
    internal class TemplateFileConfig
    {
        /// <summary>
        /// Template file GUID.
        /// </summary>
        public string guid;
        /// <summary>
        /// Destination file path.
        /// </summary>
        public string destination;
    }
}

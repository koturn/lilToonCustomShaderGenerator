using System;
using System.Collections.Generic;


namespace Koturn.LilToonCustomGenerator.Json
{
    [Serializable]
    internal class TemplateConfig
    {
        /// <summary>
        /// Name of config.
        /// </summary>
        public string name;
        /// <summary>
        /// Parent config name.
        /// </summary>
        public string basedOn;
        /// <summary>
        /// Name for display.
        /// </summary>
        public List<TemplateFileConfig> templates;
    }
}

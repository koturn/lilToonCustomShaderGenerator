using System;
using Koturn.LilToonCustomGenerator.Attributes;


namespace Koturn.LilToonCustomGenerator
{
    [Serializable]
    public class ShaderPropertyDefinition
    {
        // [field: RenameField("Name")]
        // public string Name { get; set; }

        // [field: RenameField("Description")]
        // public string Description { get; set; }

        // [field: RenameField("PropertyType")]
        // public ShaderPropertyType PropertyType { get; set; }

        // [field: RenameField("DefaultValue")]
        // public string DefaultValue { get; set; }

        // [field: RenameField("UniformType")]
        // public UniformVariableType UniformType { get; set; }

        public string Name;
        public string Description;
        public ShaderPropertyType PropertyType;
        public string DefaultValue;
        public UniformVariableType UniformType;

        public ShaderPropertyDefinition(string name, string description, ShaderPropertyType propertyType, string defaultValue, UniformVariableType uniformType)
        {
            Name = name;
            Description = description;
            PropertyType = propertyType;
            DefaultValue = defaultValue;
            UniformType = uniformType;
        }
    }
}

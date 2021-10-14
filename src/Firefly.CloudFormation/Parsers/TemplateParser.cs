namespace Firefly.CloudFormation.Parsers
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    using Firefly.CloudFormation.Model;
    using Firefly.CloudFormationParser;
    using Firefly.CloudFormationParser.Serialization.Settings;
    using Firefly.CloudFormationParser.TemplateObjects;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using YamlDotNet.RepresentationModel;
    using YamlDotNet.Serialization;

    /// <summary>
    /// Base class for CloudFormation template parsers.
    /// </summary>
    public class TemplateParser : ITemplateParser
    {
        private readonly ITemplate template;

        /// <summary>
        /// The description key name
        /// </summary>
        internal const string DescriptionKeyName = "Description";

        /// <summary>
        /// Amount of padding to add to resource names to include random chars added by CloudFormation
        /// </summary>
        internal const int NestedStackPadWidth = 14;

        /// <summary>
        /// The nested stack type
        /// </summary>
        internal const string NestedStackType = "AWS::CloudFormation::Stack";

        /// <summary>
        /// The parameter key name
        /// </summary>
        internal const string ParameterKeyName = "Parameters";

        /// <summary>
        /// The resource key name
        /// </summary>
        internal const string ResourceKeyName = "Resources";

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateParser"/> class.
        /// </summary>
        /// <param name="template">The template.</param>
        protected TemplateParser(ITemplate template)
        {
            this.template = template;
        }

        /// <summary>
        /// Creates a parser subclass of the appropriate type for the input content.
        /// </summary>
        /// <param name="templateBody">The template body.</param>
        /// <returns>A new <see cref="TemplateParser"/></returns>
        /// <exception cref="InvalidDataException">Template body is empty</exception>
        public static ITemplateParser Create(string templateBody)
        {
            using (var settings = new StringDeserializerSettings(templateBody))
            {
                return new TemplateParser(Template.Deserialize(settings).Result);
            }
        }

        /// <summary>
        /// Serializes an object graph to JSON or YAML string
        /// </summary>
        /// <param name="objectGraph">The object graph.</param>
        /// <param name="format">The required serialization format.</param>
        /// <returns>Object graph serialized to string in requested format.</returns>
        // ReSharper disable once UnusedMember.Global - Provided as an API method for clients of this module.
        public static string SerializeObjectGraphToString(object objectGraph, SerializationFormat format)
        {
            switch (format)
            {
                case SerializationFormat.Json:

                    return JsonConvert.SerializeObject(objectGraph, Formatting.Indented);

                case SerializationFormat.Yaml:

                    return new SerializerBuilder().Build().Serialize(objectGraph);

                default:

                    throw new System.InvalidOperationException($"Unsupported format: {format}");
            }
        }

        /// <summary>
        /// Serializes the object graph to representation model.
        /// </summary>
        /// <param name="objectGraph">The object graph.</param>
        /// <param name="format">The format.</param>
        /// <returns>Either a <see cref="YamlNode"/> or a <see cref="JObject"/> depending on requested format.</returns>
        /// <exception cref="System.InvalidOperationException">Unsupported format: {format}</exception>
        public static object SerializeObjectGraphToRepresentationModel(object objectGraph, SerializationFormat format)
        {
            switch (format)
            {
                case SerializationFormat.Json:

                    if (objectGraph == null || objectGraph is string)
                    {
                        return new JValue(objectGraph);
                    }

                    return JObject.Parse(JsonConvert.SerializeObject(objectGraph, Formatting.Indented));

                case SerializationFormat.Yaml:

                    var yaml = new YamlStream();

                    using (var sr = new StringReader(new SerializerBuilder().Build().Serialize(objectGraph)))
                    {
                        yaml.Load(sr);
                    }

                    return yaml.Documents[0].RootNode;

                default:

                    throw new System.InvalidOperationException($"Unsupported format: {format}");
            }
        }

        /// <inheritdoc />
        public IEnumerable<string> GetLogicalResourceNames(string stackName)
        {
            return this.template.GetLogicalResourceNames(stackName);
        }

        /// <inheritdoc />
        public IEnumerable<string> GetNestedStackNames()
        {
            return this.GetNestedStackNames(string.Empty);
        }

        /// <inheritdoc />
        public IEnumerable<string> GetNestedStackNames(string baseStackName)
        {
            return this.template.GetNestedStackNames(baseStackName);
        }

        /// <inheritdoc />
        public IEnumerable<IParameter> GetParameters()
        {
            return this.template.Parameters;
        }

        /// <inheritdoc />
        public IEnumerable<IResource> GetResources()
        {
            return this.template.Resources;
        }

        /// <inheritdoc />
        public string GetTemplateDescription()
        {
            return this.template.GetTemplateDescription();
        }

        /// <summary>
        /// Saves the template to the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        public void Save(string path)
        {
            File.WriteAllText(path, this.GetTemplate(), new UTF8Encoding(false));
        }

        /// <summary>
        /// Gets the template by re-serializing the current state of the representation model.
        /// </summary>
        /// <returns>Template body as string</returns>
        public string GetTemplate()
        {
            return Template.Serialize(this.template);
        }
    }
}
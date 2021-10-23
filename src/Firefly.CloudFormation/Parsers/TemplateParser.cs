namespace Firefly.CloudFormation.Parsers
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    using Firefly.CloudFormationParser;
    using Firefly.CloudFormationParser.Serialization.Settings;
    using Firefly.CloudFormationParser.TemplateObjects;

    /// <summary>
    /// Base class for CloudFormation template parsers.
    /// </summary>
    public class TemplateParser : ITemplateParser
    {
        /// <summary>
        /// The template
        /// </summary>
        private readonly ITemplate template;

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
            using (var settings = new DeserializerSettingsBuilder().WithTemplateString(templateBody).Build())
            {
                return new TemplateParser(Template.Deserialize(settings).Result);
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
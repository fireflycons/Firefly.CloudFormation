namespace Firefly.CloudFormation.Tests.Unit.CloudFormation.Parsers
{
    // ReSharper disable StyleCop.SA1600
    #pragma warning disable 649

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Firefly.CloudFormation.Parsers;
    using Firefly.EmbeddedResourceLoader;

    public class TemplateParserFixture : AutoResourceLoader, IDisposable
    {
        [EmbeddedResource("test-stack.json")]
        private string testStackJsonString;

        [EmbeddedResource("test-stack.yaml")]
        private string testStackYamlString;

        [EmbeddedResource("test-nested-stack.json")]
        private string testNestedStackJsonString;

        [EmbeddedResource("test-nested-stack.yaml")]
        private string testNestedStackYamlString;

        public TemplateParserFixture()
        {
            var jsonParser = Firefly.CloudFormation.Parsers.TemplateParser.Create(this.testStackJsonString);
            var yamlParser = Firefly.CloudFormation.Parsers.TemplateParser.Create(this.testStackYamlString);

            this.JsonParameters = jsonParser.GetParameters().ToList();
            this.JsonTemplateDescription = jsonParser.GetTemplateDescription();

            this.JsonResources = jsonParser.GetResources();
            this.YamlResources = yamlParser.GetResources();

            this.YamlParameters = yamlParser.GetParameters().ToList();
            this.YamlTemplateDescription = yamlParser.GetTemplateDescription();

            jsonParser = Firefly.CloudFormation.Parsers.TemplateParser.Create(this.testNestedStackJsonString);
            yamlParser = Firefly.CloudFormation.Parsers.TemplateParser.Create(this.testNestedStackYamlString);

            this.JsonNestedStacks = jsonParser.GetNestedStackNames();
            this.YamlNestedStacks = yamlParser.GetNestedStackNames();
        }

        internal List<TemplateFileParameter> JsonParameters { get; }

        internal List<TemplateFileParameter> YamlParameters { get; }

        internal string JsonTemplateDescription { get; }

        internal string YamlTemplateDescription { get; }

        internal IEnumerable<string> JsonNestedStacks { get; }

        internal IEnumerable<string> YamlNestedStacks { get; }

        internal IEnumerable<ITemplateResource> JsonResources { get; }

        internal IEnumerable<ITemplateResource> YamlResources { get; }

        public void Dispose()
        {
            // Nothing to dispose
        }
    }
}
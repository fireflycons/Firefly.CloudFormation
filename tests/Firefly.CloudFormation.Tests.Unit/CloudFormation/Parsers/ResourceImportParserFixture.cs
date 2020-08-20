namespace Firefly.CloudFormation.Tests.Unit.CloudFormation.Parsers
{
    // ReSharper disable StyleCop.SA1600
    #pragma warning disable 649

    using System;
    using System.Collections.Generic;

    using Amazon.CloudFormation.Model;

    using Firefly.EmbeddedResourceLoader;

    public class ResourceImportParserFixture : AutoResourceLoader, IDisposable
    {
        [EmbeddedResource("test-imports.json")]
        private string testImportsJson;

        [EmbeddedResource("test-imports.yaml")]
        private string testImportsYaml;

        public ResourceImportParserFixture()
        {
            var jsonParser =
                Firefly.CloudFormation.Parsers.ResourceImportParser.Create(this.testImportsJson);
            var yamlParser =
                Firefly.CloudFormation.Parsers.ResourceImportParser.Create(this.testImportsYaml);

            this.JsonResources = jsonParser.GetResourcesToImport();
            this.YamlResources = yamlParser.GetResourcesToImport();
        }

        internal List<ResourceToImport> JsonResources { get; }

        internal List<ResourceToImport> YamlResources { get; }

        public void Dispose()
        {
            // Nothing to dispose
        }
    }
}
namespace Firefly.CloudFormation.Tests.Unit.CloudFormation.Parsers
{
    // ReSharper disable StyleCop.SA1600
    #pragma warning disable 649

    using System;
    using System.Collections.Generic;

    using Firefly.EmbeddedResourceLoader;

    public class ParameterFileParserFixture : AutoResourceLoader, IDisposable
    {
        [EmbeddedResource("parameter-file.json")]
        private string jsonParameterFile;

        [EmbeddedResource("parameter-file.yaml")]
        private string yamlParameterFile;

        public ParameterFileParserFixture()
        {
            var jsonParser = Firefly.CloudFormation.Parsers.ParameterFileParser.CreateParser(this.jsonParameterFile);
            var yamlParser = Firefly.CloudFormation.Parsers.ParameterFileParser.CreateParser(this.yamlParameterFile);

            this.JsonParameters = jsonParser.ParseParameterFile();
            this.YamlParameters = yamlParser.ParseParameterFile();
        }

        public IDictionary<string, string> JsonParameters { get; set; }

        public IDictionary<string, string> YamlParameters { get; set; }

        public void Dispose()
        {
        }
    }
}
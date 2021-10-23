namespace Firefly.CloudFormation.Tests.Unit.CloudFormation.Parsers
{
    // ReSharper disable StyleCop.SA1600
    #pragma warning disable 649

    using System.Linq;

    using Firefly.EmbeddedResourceLoader;

    using FluentAssertions;

    using Xunit;

    /// <summary>
    /// Tests updating of resource properties within a template.
    /// You would need to do this to implement the behaviour of <c>aws cloudformation package</c>
    /// </summary>
    public class ResourcePropertyUpdate : AutoResourceLoader
    {
        //[EmbeddedResource("test-resource-update.json")]
        //private static string testResourceUpdateJson;

        [EmbeddedResource("test-resource-update.yaml")]
        private static string testResourceUpdateYaml;

        //[Fact]
        //public void UpdateJsonResource()
        //{
        //    const string S3Location = "s3://bucket/job.etl";

        //    var parser = Firefly.CloudFormation.Parsers.TemplateParser.Create(testResourceUpdateJson);
        //    var resources = parser.GetResources();

        //    var resource = resources.First(r => r.Name == "MyJob");

        //    // resource.UpdateResourceProperty("Code", new { S3Bucket = "bucket-name", S3Key = "code/lambda.zip" });
        //    resource.UpdateResourceProperty("Command.ScriptLocation", S3Location);
        //    var modifiedTemplate = parser.GetTemplate();

        //    modifiedTemplate.Should().Contain($"ScriptLocation: {S3Location}");
        //    resource.GetResourcePropertyValue("Command.ScriptLocation").Should().Be(S3Location);
        //}

        [Fact]
        public void UpdateYamlResource()
        {
            const string S3Bucket = "bucket-name";
            const string S3Key = "code/lambda.zip";

            var parser = Firefly.CloudFormation.Parsers.TemplateParser.Create(testResourceUpdateYaml);
            var resources = parser.GetResources();

            var resource = resources.First(r => r.Name == "lambdaFunction");

            resource.UpdateResourceProperty("Code", new { S3Bucket, S3Key });
            var modifiedTemplate = parser.GetTemplate();
            modifiedTemplate.Should().Contain($"S3Bucket: {S3Bucket}").And.Contain($"S3Key: {S3Key}");
            resource.GetResourcePropertyValue("Code.S3Bucket").Should().Be(S3Bucket);
            resource.GetResourcePropertyValue("Code.S3Key").Should().Be(S3Key);
        }
    }
}
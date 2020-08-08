namespace Firefly.CloudFormation.Tests.Unit.Utils
{
    using System;

    using Amazon;

    using Firefly.CloudFormation.Model;

    using Moq;

    public static class TestHelpers
    {
        public const string AccountId = "123456789012";

        public static readonly RegionEndpoint Region = RegionEndpoint.EUWest1;

        public static readonly string RegionName = Region.SystemName;

        public static readonly string CloudFormationBucketName =
            $"cf-templates-pscloudformation-{RegionName}-{AccountId}";

        internal static Mock<IAwsClientFactory> GetClientFactoryMock()
        {
            var mock = new Mock<IAwsClientFactory>();

            return mock;
        }

        internal static Mock<ICloudFormationContext> GetContextMock(TestLogger logger)
        {
            var mockContext = new Mock<ICloudFormationContext>();

            mockContext.Setup(c => c.Logger).Returns(logger);
            mockContext.Setup(c => c.Region).Returns(Region);

            return mockContext;
        }

        internal static Mock<IS3Util> GetS3UtilMock()
        {
            var mockS3Util = new Mock<IS3Util>();

            mockS3Util
                .Setup(
                    s3 => s3.UploadOversizeArtifactToS3(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<UploadFileType>())).ReturnsAsync(
                    new Uri($"https://{TestHelpers.CloudFormationBucketName}/template-file"));

            return mockS3Util;
        }

        /// <summary>
        /// Mock that also supports GetS3ObjectContent
        /// </summary>
        /// <param name="tempfile">The tempfile.</param>
        /// <returns>The mock</returns>
        internal static Mock<IS3Util> GetS3UtilMock(TempFile tempfile)
        {
            var mockS3Util = GetS3UtilMock();

             mockS3Util.Setup(s3 => s3.GetS3ObjectContent(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(tempfile.GetContent());

            return mockS3Util;
        }
    }
}
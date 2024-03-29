﻿namespace Firefly.CloudFormation.Tests.Unit.CloudFormation
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Amazon.CloudFormation;
    using Amazon.CloudFormation.Model;

    using Firefly.CloudFormation.Model;
    using Firefly.CloudFormation.Tests.Unit.Utils;

    using FluentAssertions;

    using Moq;

    using Xunit;
    using Xunit.Abstractions;

    [Collection("Sequential")]
    public class CreateStack : IClassFixture<TestStackFixture>
    {
        /// <summary>
        /// The stack name
        /// </summary>
        private const string StackName = "test-stack";

        /// <summary>
        /// The stack identifier
        /// </summary>
        private static readonly string StackId =
            $"arn:aws:cloudformation:{TestHelpers.RegionName}:{TestHelpers.AccountId}:stack/test-stack";

        /// <summary>
        /// The output
        /// </summary>
        private readonly ITestOutputHelper output;

        private readonly TestStackFixture fixture;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateStack"/> class.
        /// </summary>
        /// <param name="fixture">Test fixture</param>
        /// <param name="output">The output.</param>
        public CreateStack(TestStackFixture fixture, ITestOutputHelper output)
        {
            this.fixture = fixture;
            this.output = output;
        }

        /// <summary>
        /// Should create stack if stack does not exist.
        /// </summary>
        [Fact]
        public async void ShouldCreateStackIfStackDoesNotExist()
        {
            var logger = new TestLogger(this.output);
            var mockClientFactory = TestHelpers.GetClientFactoryMock();
            var mockContext = TestHelpers.GetContextMock(logger);

            var mockCloudFormation = new Mock<IAmazonCloudFormation>();
            mockCloudFormation.SetupSequence(cf => cf.DescribeStacksAsync(It.IsAny<DescribeStacksRequest>(), default))
                .Throws(new AmazonCloudFormationException($"Stack with id {StackName} does not exist")).ReturnsAsync(
                    new DescribeStacksResponse
                    {
                        Stacks = new List<Stack>
                                         {
                                             new Stack()
                                                 {
                                                     StackName = StackName,
                                                     StackId = StackId,
                                                     StackStatus = StackStatus.CREATE_COMPLETE
                                                 }
                                         }
                    });

            mockCloudFormation.Setup(cf => cf.CreateStackAsync(It.IsAny<CreateStackRequest>(), default))
                .ReturnsAsync(new CreateStackResponse { StackId = StackId });

            mockCloudFormation.Setup(cf => cf.DescribeStackEventsAsync(It.IsAny<DescribeStackEventsRequest>(), default))
                .ReturnsAsync(
                    new DescribeStackEventsResponse
                    {
                        StackEvents = new List<StackEvent>
                                              {
                                                  new StackEvent
                                                      {
                                                          StackName = StackName,
                                                          StackId = StackId,
                                                          ResourceStatus = ResourceStatus.CREATE_COMPLETE,
                                                          Timestamp = DateTime.Now.AddSeconds(1)
                                                      }
                                              }
                    });

            mockCloudFormation.Setup(cf => cf.DescribeStackResourcesAsync(It.IsAny<DescribeStackResourcesRequest>(), default))
                .ReturnsAsync(new DescribeStackResourcesResponse { StackResources = new List<StackResource>() });

            mockClientFactory.Setup(f => f.CreateCloudFormationClient()).Returns(mockCloudFormation.Object);

            var runner = CloudFormationRunner.Builder(mockContext.Object, StackName)
                .WithClientFactory(mockClientFactory.Object)
                .WithFollowOperation()
                .WithTemplateLocation(this.fixture.TestStackJsonTemplate.FullPath)
                .Build();

            (await runner.CreateStackAsync()).StackOperationResult.Should().Be(StackOperationResult.StackCreated);
            logger.StackEvents.Count.Should().BeGreaterThan(0);
        }

        /// <summary>
        /// Should create stack if stack does not exist.
        /// </summary>
        [Fact]
        public async void ShouldUploadTempateToS3ThenCreateStackIfStackDoesNotExistAndForceS3IsSet()
        {
            var logger = new TestLogger(this.output);
            var mockClientFactory = TestHelpers.GetClientFactoryMock();
            var mockContext = TestHelpers.GetContextMock(logger);
            var mockS3Util = TestHelpers.GetS3UtilMock();

            mockContext.Setup(ctx => ctx.S3Util).Returns(mockS3Util.Object);

            var mockCloudFormation = new Mock<IAmazonCloudFormation>();
            mockCloudFormation.SetupSequence(cf => cf.DescribeStacksAsync(It.IsAny<DescribeStacksRequest>(), default))
                .Throws(new AmazonCloudFormationException($"Stack with id {StackName} does not exist")).ReturnsAsync(
                    new DescribeStacksResponse
                    {
                        Stacks = new List<Stack>
                                         {
                                             new Stack()
                                                 {
                                                     StackName = StackName,
                                                     StackId = StackId,
                                                     StackStatus = StackStatus.CREATE_COMPLETE
                                                 }
                                         }
                    });

            mockCloudFormation.Setup(cf => cf.CreateStackAsync(It.IsAny<CreateStackRequest>(), default))
                .ReturnsAsync(new CreateStackResponse { StackId = StackId });

            mockCloudFormation.Setup(cf => cf.DescribeStackEventsAsync(It.IsAny<DescribeStackEventsRequest>(), default))
                .ReturnsAsync(
                    new DescribeStackEventsResponse
                    {
                        StackEvents = new List<StackEvent>
                                              {
                                                  new StackEvent
                                                      {
                                                          StackName = StackName,
                                                          StackId = StackId,
                                                          ResourceStatus = ResourceStatus.CREATE_COMPLETE,
                                                          Timestamp = DateTime.Now.AddSeconds(1)
                                                      }
                                              }
                    });

            mockCloudFormation.Setup(cf => cf.DescribeStackResourcesAsync(It.IsAny<DescribeStackResourcesRequest>(), default))
                .ReturnsAsync(new DescribeStackResourcesResponse { StackResources = new List<StackResource>() });

            mockClientFactory.Setup(f => f.CreateCloudFormationClient()).Returns(mockCloudFormation.Object);

            var runner = CloudFormationRunner.Builder(mockContext.Object, StackName)
                .WithClientFactory(mockClientFactory.Object)
                .WithFollowOperation()
                .WithTemplateLocation(this.fixture.TestStackJsonTemplate.FullPath)
                .WithForceS3()
                .Build();

            (await runner.CreateStackAsync()).StackOperationResult.Should().Be(StackOperationResult.StackCreated);

            mockS3Util.Verify(s3 => s3.UploadOversizeArtifactToS3(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<UploadFileType>()), Times.Exactly(1));

            logger.StackEvents.Count.Should().BeGreaterThan(0);
        }

        /// <summary>
        /// Should fail to create if stack exists.
        /// </summary>
        [Fact]
        public void ShouldFailIfStackExists()
        {
            var logger = new TestLogger(this.output);
            var mockClientFactory = TestHelpers.GetClientFactoryMock();
            var mockContext = TestHelpers.GetContextMock(logger);

            var mockCloudFormation = new Mock<IAmazonCloudFormation>();
            mockCloudFormation.Setup(cf => cf.DescribeStacksAsync(It.IsAny<DescribeStacksRequest>(), default)).ReturnsAsync(
                new DescribeStacksResponse { Stacks = new List<Stack> { new Stack { StackName = StackName } } });

            mockClientFactory.Setup(f => f.CreateCloudFormationClient()).Returns(mockCloudFormation.Object);

            var runner = CloudFormationRunner.Builder(mockContext.Object, StackName)
                .WithClientFactory(mockClientFactory.Object)
                .WithTemplateLocation(this.fixture.TestStackJsonTemplate.FullPath)
                .Build();

            Func<Task<CloudFormationResult>> action = async () => await runner.CreateStackAsync();

            action.Should().Throw<StackOperationException>().And.OperationalState.Should().Be(StackOperationalState.Exists);
        }
    }
}
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
    public class DeleteStack : IClassFixture<TestStackFixture>
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
        /// Initializes a new instance of the <see cref="DeleteStack"/> class.
        /// </summary>
        /// <param name="output">The output.</param>
        public DeleteStack(TestStackFixture fixture, ITestOutputHelper output)
        {
            this.fixture = fixture;
            this.output = output;
        }

        /// <summary>
        /// If the stack exists and is in the correct state, then it should be deleted.
        /// </summary>
        /// <param name="status">The status.</param>
        [SkippableTheory]
        [InlineData("CREATE_COMPLETE")]
        [InlineData("IMPORT_COMPLETE")]
        [InlineData("ROLLBACK_COMPLETE")]
        [InlineData("UPDATE_COMPLETE")]
        [InlineData("UPDATE_ROLLBACK_COMPLETE")]
        public async void ShouldDeleteStackIfStackExistsAndIsInCorrectState(string status)
        {
            var logger = new TestLogger(this.output);
            var mockClientFactory = TestHelpers.GetClientFactoryMock();
            var mockContext = TestHelpers.GetContextMock(logger);

            var mockCloudFormation = new Mock<IAmazonCloudFormation>();

            mockCloudFormation.SetupSequence(cf => cf.DescribeStacksAsync(It.IsAny<DescribeStacksRequest>(), default))
                .ReturnsAsync(
                    new DescribeStacksResponse
                    {
                        Stacks = new List<Stack>
                                         {
                                             new Stack()
                                                 {
                                                     StackName = StackName, StackStatus = StackStatus.FindValue(status)
                                                 }
                                         }
                    }).ReturnsAsync(
                    new DescribeStacksResponse
                    {
                        Stacks = new List<Stack>
                                         {
                                             new Stack()
                                                 {
                                                     StackName = StackName, StackStatus = StackStatus.FindValue(status)
                                                 }
                                         }
                    }).ReturnsAsync(
                    new DescribeStacksResponse
                    {
                        Stacks = new List<Stack>
                                         {
                                             new Stack()
                                                 {
                                                     StackName = StackName, StackStatus = StackStatus.DELETE_IN_PROGRESS
                                                 }
                                         }
                    }).ReturnsAsync(
                    new DescribeStacksResponse
                    {
                        Stacks = new List<Stack>
                                         {
                                             new Stack()
                                                 {
                                                     StackName = StackName, StackStatus = StackStatus.DELETE_COMPLETE
                                                 }
                                         }
                    });

            mockCloudFormation.Setup(cf => cf.DeleteStackAsync(It.IsAny<DeleteStackRequest>(), default))
                .ReturnsAsync(new DeleteStackResponse());

            mockCloudFormation.SetupSequence(cf => cf.DescribeStackEventsAsync(It.IsAny<DescribeStackEventsRequest>(), default))
                .ReturnsAsync(
                    new DescribeStackEventsResponse
                    {
                        StackEvents = new List<StackEvent>
                                              {
                                                  new StackEvent
                                                      {
                                                          StackName = StackName,
                                                          StackId = StackId,
                                                          ResourceStatus = status,
                                                          Timestamp = DateTime.Now.AddDays(-1)
                                                      }
                                              }
                    })
                .ReturnsAsync(
                    new DescribeStackEventsResponse
                    {
                        StackEvents = new List<StackEvent>
                                              {
                                                  new StackEvent
                                                      {
                                                          StackName = StackName,
                                                          StackId = StackId,
                                                          ResourceStatus = ResourceStatus.DELETE_COMPLETE,
                                                          Timestamp = DateTime.Now.AddSeconds(1)
                                                      }
                                              }
                    })
                .ReturnsAsync(
                new DescribeStackEventsResponse
                {
                    StackEvents = new List<StackEvent>()
                });

            mockCloudFormation.Setup(cf => cf.DescribeStackResourcesAsync(It.IsAny<DescribeStackResourcesRequest>(), default))
                .ReturnsAsync(new DescribeStackResourcesResponse { StackResources = new List<StackResource>() });

            mockCloudFormation.Setup(cf => cf.GetTemplateAsync(It.IsAny<GetTemplateRequest>(), default)).ReturnsAsync(
                new GetTemplateResponse
                {
                    TemplateBody = this.fixture.TestStackJsonString
                });

            mockCloudFormation.Setup(cf => cf.GetTemplateSummaryAsync(It.IsAny<GetTemplateSummaryRequest>(), default))
                .ReturnsAsync(new GetTemplateSummaryResponse { Parameters = new List<ParameterDeclaration>() });

            mockClientFactory.Setup(f => f.CreateCloudFormationClient()).Returns(mockCloudFormation.Object);
            var runner = CloudFormationRunner.Builder(mockContext.Object, StackName)
                .WithClientFactory(mockClientFactory.Object)
                .WithFollowOperation()
                .Build();

            (await runner.DeleteStackAsync()).StackOperationResult.Should().Be(StackOperationResult.StackDeleted);
            logger.StackEvents.Count.Should().BeGreaterThan(0);
        }

        /// <summary>
        /// If the stack does not exist, delete should fail
        /// </summary>
        [Fact]
        public void ShouldFailIfStackDoesNotExist()
        {
            var logger = new TestLogger(this.output);
            var mockClientFactory = TestHelpers.GetClientFactoryMock();
            var mockContext = TestHelpers.GetContextMock(logger);

            var mockCloudFormation = new Mock<IAmazonCloudFormation>();
            mockCloudFormation.Setup(cf => cf.DescribeStacksAsync(It.IsAny<DescribeStacksRequest>(), default)).Throws(
                new AmazonCloudFormationException($"Stack with id {StackName} does not exist"));

            mockClientFactory.Setup(f => f.CreateCloudFormationClient()).Returns(mockCloudFormation.Object);

            var runner = CloudFormationRunner.Builder(mockContext.Object, StackName)
                .WithClientFactory(mockClientFactory.Object)
                .WithFollowOperation()
                .Build();

            Func<Task<CloudFormationResult>> action = async () => await runner.DeleteStackAsync();

            action.Should().Throw<StackOperationException>().And.OperationalState.Should().Be(StackOperationalState.NotFound);
        }

        /// <summary>
        /// Stack delete should fail if stack is not in correct state
        /// </summary>
        /// <param name="stackStatus">The stack status.</param>
        /// <param name="expectedOutcome">Expected outcome</param>
        [Theory]
        [InlineData("CREATE_IN_PROGRESS", StackOperationalState.Busy)]
        [InlineData("IMPORT_IN_PROGRESS", StackOperationalState.Busy)]
        [InlineData("IMPORT_ROLLBACK_IN_PROGRESS", StackOperationalState.Busy)]
        [InlineData("REVIEW_IN_PROGRESS", StackOperationalState.Busy)]
        [InlineData("ROLLBACK_IN_PROGRESS", StackOperationalState.Busy)]
        [InlineData("UPDATE_COMPLETE_CLEANUP_IN_PROGRESS", StackOperationalState.Busy)]
        [InlineData("UPDATE_IN_PROGRESS", StackOperationalState.Busy)]
        [InlineData("UPDATE_ROLLBACK_COMPLETE_CLEANUP_IN_PROGRESS", StackOperationalState.Busy)]
        [InlineData("UPDATE_ROLLBACK_IN_PROGRESS", StackOperationalState.Busy)]

        public void ShouldFailIfStackIsBrokenOrBusy(string stackStatus, StackOperationalState expectedOutcome)
        {
            var logger = new TestLogger(this.output);
            var mockClientFactory = TestHelpers.GetClientFactoryMock();
            var mockContext = TestHelpers.GetContextMock(logger);
            var mockCloudFormation = new Mock<IAmazonCloudFormation>();

            mockCloudFormation.SetupSequence(cf => cf.DescribeStacksAsync(It.IsAny<DescribeStacksRequest>(), default))
                .ReturnsAsync(
                    new DescribeStacksResponse
                    {
                        Stacks = new List<Stack>
                                         {
                                                 new Stack()
                                                     {
                                                         StackName = StackName, StackStatus = StackStatus.FindValue(stackStatus)
                                                     }
                                         }
                    }).ReturnsAsync(
                    new DescribeStacksResponse
                    {
                        Stacks = new List<Stack>
                                         {
                                                 new Stack()
                                                     {
                                                         StackName = StackName, StackStatus = StackStatus.FindValue(stackStatus)
                                                     }
                                         }
                    });

            mockClientFactory.Setup(f => f.CreateCloudFormationClient()).Returns(mockCloudFormation.Object);

            var runner = CloudFormationRunner.Builder(mockContext.Object, StackName)
                .WithClientFactory(mockClientFactory.Object)
                .Build();

            Func<Task<CloudFormationResult>> action = async () => await runner.DeleteStackAsync();

            action.Should().Throw<StackOperationException>().And.OperationalState.Should().Be(expectedOutcome);
        }
    }
}
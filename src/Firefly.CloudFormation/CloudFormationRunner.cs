﻿[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("ReedExpo.Cake.AWS.CloudFormation.TestBase")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("ReedExpo.Cake.AWS.CloudFormation.Tests.Unit")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("ReedExpo.Cake.AWS.CloudFormation.Tests.Integration")]

namespace Firefly.CloudFormation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Amazon.CloudFormation;
    using Amazon.CloudFormation.Model;

    using Firefly.CloudFormation.Model;
    using Firefly.CloudFormation.Parsers;
    using Firefly.CloudFormation.Resolvers;

    /// <summary>
    /// <para>
    /// Class to manage all stack operations
    /// </para>
    /// <para>
    /// This is the workhorse of the library, managing interaction with the AWS CloudFormation API.
    /// There are no public constructors as a builder pattern is used to create an instance due to
    /// the very large number of constructor arguments.
    /// </para>
    /// <para>
    /// To construct this object, get a builder instance by calling <see cref="CloudFormationRunner.Builder"/>
    /// </para>
    /// </summary>
    public partial class CloudFormationRunner : ICloudFormationRunner
    {
        #region Fields

        /// <summary>
        /// Messages that can be returned by change set creation if the stack is unchanged.
        /// </summary>
        private static readonly string[] NoChangeMessages =
            {
                "The submitted information didn't contain changes", 
                "No updates are to be performed"
            };

        /// <summary>The stack capabilities</summary>
        private readonly IEnumerable<Capability> capabilities = new List<Capability>();

        /// <summary>if set to <c>true</c> only create change set without updating.</summary>
        private readonly bool changesetOnly;

        /// <summary>The CloudFormation client</summary>
        private readonly IAmazonCloudFormation client;

        /// <summary>
        /// The client factory
        /// </summary>
        private readonly IAwsClientFactory clientFactory;

        /// <summary>
        /// Client token 
        /// </summary>
        private readonly string clientToken;

        /// <summary>
        /// The context
        /// </summary>
        private readonly ICloudFormationContext context;

        /// <summary>Whether to delete a change set that results in no change.</summary>
        private readonly bool deleteNoopChangeSet;

        /// <summary>
        /// The disable rollback
        /// </summary>
        private readonly bool disableRollback;

        /// <summary>
        /// The imports
        /// </summary>
        private readonly string resourcesToImportLocation;

        /// <summary>
        /// SNS notification ARNs
        /// </summary>
        // ReSharper disable once InconsistentNaming
        private readonly List<string> notificationARNs;

        /// <summary>
        /// The on failure
        /// </summary>
        private readonly OnFailure onFailure;

        /// <summary>The stack parameters</summary>
        private readonly IDictionary<string, string> parameters = new Dictionary<string, string>();

        /// <summary>
        /// The resource type
        /// </summary>
        private readonly List<string> resourceType;

        /// <summary>
        /// ARN of ole to assume during CF operations
        /// </summary>
        private readonly string roleArn;

        /// <summary>
        /// The rollback configuration
        /// </summary>
        private readonly RollbackConfiguration rollbackConfiguration;

        /// <summary>The stack name</summary>
        private readonly string stackName;

        /// <summary>
        /// The stack operations
        /// </summary>
        private readonly CloudFormationOperations stackOperations;

        /// <summary>
        /// The stack policy during update location
        /// </summary>
        private readonly string stackPolicyDuringUpdateLocation;

        /// <summary>
        /// The stack policy location
        /// </summary>
        private readonly string stackPolicyLocation;

        /// <summary>The tagging information</summary>
        private readonly List<Tag> tags;

        /// <summary>The template path</summary>
        private readonly string templateLocation;

        /// <summary>
        /// The termination protection
        /// </summary>
        private readonly bool terminationProtection;

        /// <summary>
        /// The timeout in minutes
        /// </summary>
        private readonly int timeoutInMinutes;

        /// <summary>
        /// The use previous template
        /// </summary>
        private readonly bool usePreviousTemplate;

        /// <summary>  How long to wait between polls for events</summary>
        private readonly int waitPollTime = 5000;

        /// <summary>
        /// Whether to follow an operation initiated by another process.
        /// </summary>
        private readonly bool waitForInProgressUpdate;

        /// <summary>
        /// Whether to force use of S3
        /// </summary>
        private readonly bool forceS3;

        /// <summary>
        /// Whether to include nested stacks in changesets
        /// </summary>
        private readonly bool includeNestedStacks;

        /// <summary>
        /// The retain resource
        /// </summary>
        private List<string> retainResource;

        /// <summary>
        /// The template resolver
        /// </summary>
        private IInputFileResolver templateResolver;

        /// <summary>The last event time</summary>
        private DateTime lastEventTime;

        /// <summary>
        /// The template description
        /// </summary>
        private string templateDescription;

        /// <summary>Whether to follow an in-progress operation.</summary>
        private bool followOperation;

        #endregion

        #region Constructor

        /// <summary>Initializes a new instance of the <see cref="CloudFormationRunner"/> class.</summary>
        /// <param name="clientFactory">Factory for creating AWS service clients</param>
        /// <param name="stackName">Name of the stack.</param>
        /// <param name="templateLocation">Template location. Either path or URL.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="capabilities">The capabilities.</param>
        /// <param name="context">The Cake context.</param>
        /// <param name="tags">Stack level tags.</param>
        /// <param name="followOperation">if set to <c>true</c> [follow operation].</param>
        /// <param name="waitForInProgressUpdate">if set to <c>true</c> [wait for in progress update].</param>
        /// <param name="deleteNoopChangeSet">if set to <c>true</c> [delete no-op change set].</param>
        /// <param name="changesetOnly">if set to <c>true</c> only create change set without updating.</param>
        /// <param name="resourcesToImportLocation">Resources to import</param>
        /// <param name="roleArn">Role to assume</param>
        /// <param name="clientToken">Client token</param>
        /// <param name="notificationARNs">SNS notification ARNs</param>
        /// <param name="usePreviousTemplate">Whether to use existing template for update.</param>
        /// <param name="rollbackConfiguration">The rollback configuration</param>
        /// <param name="stackPolicyLocation">Location of structure containing a new stack policy body.</param>
        /// <param name="stackPolicyDuringUpdateLocation">Location of structure containing the temporary overriding stack policy body</param>
        /// <param name="resourceType">The template resource types that you have permissions to work with for this create stack action.</param>
        /// <param name="terminationProtection">Whether to enable termination protection on the specified stack.</param>
        /// <param name="onFailure">Determines what action will be taken if stack creation fails.</param>
        /// <param name="timeoutInMinutes">The amount of time that can pass before the stack status becomes CREATE_FAILED</param>
        /// <param name="disableRollback">Set to <c>true</c> to disable rollback of the stack if stack creation failed.</param>
        /// <param name="retainResource">For stacks in the DELETE_FAILED state, a list of resource logical IDs that are associated with the resources you want to retain.</param>
        /// <param name="forceS3">If <c>true</c> always upload local templates to S3.</param>
        /// <param name="includeNestedStacks">Creates a change set for the all nested stacks specified in the template. The default behavior of this action is set to <c>false</c>. To include nested sets in a change set, specify <c>true</c>.</param>
        /// <remarks>Constructor is private as this class implements the builder pattern. See CloudFormation.Runner.Builder.cs</remarks>
        internal CloudFormationRunner(
            IAwsClientFactory clientFactory,
            string stackName,
            string templateLocation,
            IDictionary<string, string> parameters,
            IEnumerable<Capability> capabilities,
            ICloudFormationContext context,
            List<Tag> tags,
            bool followOperation,
            bool waitForInProgressUpdate,
            bool deleteNoopChangeSet,
            bool changesetOnly,
            string resourcesToImportLocation,
            string roleArn,
            string clientToken,
            // ReSharper disable once InconsistentNaming
            List<string> notificationARNs,
            bool usePreviousTemplate,
            RollbackConfiguration rollbackConfiguration,
            string stackPolicyLocation,
            string stackPolicyDuringUpdateLocation,
            List<string> resourceType,
            bool terminationProtection,
            OnFailure onFailure,
            int timeoutInMinutes,
            bool disableRollback,
            List<string> retainResource,
            bool forceS3,
            bool includeNestedStacks)
        {
            this.includeNestedStacks = includeNestedStacks;
            this.forceS3 = forceS3;
            this.retainResource = retainResource;
            this.disableRollback = disableRollback;
            this.timeoutInMinutes = timeoutInMinutes;
            this.onFailure = onFailure;
            this.terminationProtection = terminationProtection;
            this.resourceType = resourceType;
            this.stackPolicyDuringUpdateLocation = stackPolicyDuringUpdateLocation;
            this.stackPolicyLocation = stackPolicyLocation;
            this.rollbackConfiguration = rollbackConfiguration;
            this.usePreviousTemplate = usePreviousTemplate;
            this.notificationARNs = notificationARNs;
            this.clientToken = clientToken;
            this.roleArn = roleArn;
            this.resourcesToImportLocation = resourcesToImportLocation;
            this.clientFactory = clientFactory;
            this.context = context;
            this.changesetOnly = changesetOnly;
            this.templateLocation = templateLocation;
            this.stackName = stackName ?? throw new ArgumentNullException(nameof(stackName));
            this.followOperation = followOperation;
            this.waitForInProgressUpdate = waitForInProgressUpdate;
            this.deleteNoopChangeSet = deleteNoopChangeSet;

            // Cheeky unit test detection
            if (context.GetType().FullName == "Castle.Proxies.ICloudFormationContextProxy")
            {
                // Don't hang around in the wait loop
                this.waitPollTime = 50;
            }

            if (capabilities != null)
            {
                this.capabilities = capabilities;
            }

            if (parameters != null)
            {
                this.parameters = parameters;
            }

            if (tags != null)
            {
                this.tags = tags;
            }

            this.client = this.clientFactory.CreateCloudFormationClient();
            this.stackOperations = new CloudFormationOperations(this.clientFactory, this.context);

            // Get parameters and description from supplied template if any
            this.templateResolver = new TemplateResolver(this.clientFactory, this.context, this.stackName, this.usePreviousTemplate, this.forceS3);

            this.templateResolver.ResolveArtifactLocationAsync(this.context, this.templateLocation, this.stackName)
                .Wait();

            if (this.templateResolver.Source != InputFileSource.None)
            {
                var parser = TemplateParser.Create(this.templateResolver.FileContent);

                this.templateDescription = parser.GetTemplateDescription();

                // Adds base stack name + 10 chars to each nested stack to estimate logical resource ID of each nested stack
                this.context.Logger.SetStackNameColumnWidth(
                    parser.GetNestedStackNames(this.stackName).Concat(new[] { this.stackName })
                        .Max(s => s.Length));

                this.context.Logger.SetResourceNameColumnWidth(parser.GetLogicalResourceNames(this.stackName).Max(r => r.Length));
            }
        }

        #endregion

        /// <summary>
        /// Creates a builder instance for CloudFormationRunner
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="stackName">Name of the stack.</param>
        /// <returns>
        /// New builder instance
        /// </returns>
        public static CloudFormationBuilder Builder(ICloudFormationContext context, string stackName)
        {
            return new CloudFormationBuilder(context, stackName);
        }

        /// <summary>Creates a new stack.</summary>
        /// <returns><see cref="Task"/> object so we can await task return.</returns>
        public async Task<CloudFormationResult> CreateStackAsync()
        {
            try
            {
                var stack = await this.stackOperations.GetStackAsync(this.stackName);

                if (stack != null)
                {
                    throw new StackOperationException(stack, StackOperationalState.Exists);
                }
            }
            catch (StackOperationException e)
            {
                if (e.OperationalState != StackOperationalState.NotFound)
                {
                    throw;
                }
            }

            var req = new CreateStackRequest
                          {
                              Capabilities = this.capabilities.Select(c => c.Value).ToList(),
                              ClientRequestToken = this.clientToken,
                              DisableRollback = this.disableRollback,
                              EnableTerminationProtection = this.terminationProtection,
                              NotificationARNs = this.notificationARNs,
                              OnFailure = this.onFailure,
                              Parameters =
                                  this.parameters.Select(
                                      p => new Parameter { ParameterKey = p.Key, ParameterValue = p.Value }).ToList(),
                              ResourceTypes =
                                  this.resourceType != null && this.resourceType.Any() ? this.resourceType : null,
                              RollbackConfiguration = this.rollbackConfiguration,
                              RoleARN = this.roleArn,
                              StackName = this.stackName,
                              Tags = this.tags,
                              TemplateBody = this.templateResolver.ArtifactContent,
                              TemplateURL = this.templateResolver.ArtifactUrl
                          };

            if (this.timeoutInMinutes > 0)
            {
                req.TimeoutInMinutes = this.timeoutInMinutes;
            }

            var resolved = await new StackPolicyResolver(this.clientFactory, this.context).ResolveArtifactLocationAsync(
                this.context,
                this.stackPolicyLocation,
                this.stackName);

            req.StackPolicyBody = resolved.ArtifactBody;
            req.StackPolicyURL = resolved.ArtifactUrl;

            this.context.Logger.LogInformation($"Creating {this.GetStackNameWithDescription()}\n");

            var stackId = (await this.client.CreateStackAsync(req)).StackId;

            if (this.followOperation)
            {
                await this.WaitStackOperationAsync(stackId, true);
                return new CloudFormationResult
                           {
                               StackArn = stackId, StackOperationResult = StackOperationResult.StackCreated
                           };
            }

            return new CloudFormationResult
                       {
                           StackArn = stackId, StackOperationResult = StackOperationResult.StackCreateInProgress
                       };
        }

        /// <summary>Deletes a stack.</summary>
        /// <param name="invalidRetentionConfirmationFunc">
        /// Function to confirm delete if user passed resources to retain when the stack is not in a failed state due to resources that could not be deleted.
        /// If this parameter is <c>null</c>, or the function returns true, then all resources are deleted. 
        /// </param>
        /// <param name="queryDeleteStackFunc">
        /// Function to confirm whether to delete the stack at all.
        /// If this parameter is <c>null</c>, or the function returns true, then the delete proceeds. 
        /// </param>
        /// <returns>Operation result.</returns>
        public async Task<CloudFormationResult> DeleteStackAsync(Func<bool> invalidRetentionConfirmationFunc = null, Func<bool> queryDeleteStackFunc = null)
        {
            var stack = await this.stackOperations.GetStackAsync(this.stackName);
            var operationalState = await this.stackOperations.GetStackOperationalStateAsync(stack.StackId);
            var haveRetainResources = this.retainResource != null && this.retainResource.Any();

            // Only permit delete if stack is in Ready state
            if (!new[] { StackOperationalState.Ready, StackOperationalState.DeleteFailed, StackOperationalState.Broken }
                    .Contains(operationalState))
            {
                throw new StackOperationException(stack, operationalState);
            }

            if (queryDeleteStackFunc != null && !queryDeleteStackFunc())
            {
                return new CloudFormationResult
                           {
                               StackArn = stack.StackId, StackOperationResult = StackOperationResult.NoChange
                           };
            }

            if (operationalState != StackOperationalState.DeleteFailed && haveRetainResources && invalidRetentionConfirmationFunc != null)
            {
                if (!invalidRetentionConfirmationFunc())
                {
                    return new CloudFormationResult
                               {
                                   StackArn = stack.StackId,
                                   StackOperationResult = StackOperationResult.NoChange
                    };
                }

                this.retainResource = null;
            }

            if (operationalState == StackOperationalState.Broken)
            {
                this.context.Logger.LogWarning("Stack is in a failed state from previous operation. Delete may fail.");
            }

            // Resolve template from CloudFormation to get description if any
            this.templateResolver = new TemplateResolver(this.clientFactory, this.context, this.stackName, true, this.forceS3);
            await this.templateResolver.ResolveFileAsync(null);
            
            var parser = TemplateParser.Create(this.templateResolver.FileContent);
            this.templateDescription = parser.GetTemplateDescription();

            // Adds base stack name + 10 chars to each nested stack to estimate logical resource ID of each nested stack
            this.context.Logger.SetStackNameColumnWidth(
                parser.GetNestedStackNames(this.stackName).Concat(new[] { this.stackName })
                    .Max(s => s.Length));
            this.context.Logger.SetResourceNameColumnWidth(parser.GetLogicalResourceNames(this.stackName).Max(r => r.Length));

            this.context.Logger.LogInformation($"Deleting {this.GetStackNameWithDescription()}");

            if (this.retainResource != null && this.retainResource.Any())
            {
                this.context.Logger.LogInformation($"Retaining resources: {string.Join(", ", this.retainResource)}");
            }

            this.context.Logger.LogInformation(string.Empty);

            this.lastEventTime = await this.GetMostRecentStackEvent(stack.StackId);

            await this.client.DeleteStackAsync(
                new DeleteStackRequest
                    {
                        StackName = stack.StackId,
                        ClientRequestToken = this.clientToken,
                        RoleARN = this.roleArn,
                        RetainResources = this.retainResource
                    });

            if (this.followOperation)
            {
                await this.WaitStackOperationAsync(stack.StackId, true);
                return new CloudFormationResult
                           {
                               StackArn = stack.StackId, StackOperationResult = StackOperationResult.StackDeleted
                           };
            }

            return new CloudFormationResult
                       {
                           StackArn = stack.StackId, StackOperationResult = StackOperationResult.StackDeleteInProgress
                       };
        }

        /// <summary>
        /// <para>
        /// Resets a stack by deleting and recreating it.
        /// </para>
        /// <para>
        /// This is useful when an attempt to create a new stack fails, leaving an existing stack in <c>ROLLBACK_COMPLETE</c> state,
        /// or if you just want to rebuild a stack from scratch. Internally it calls <see cref="CloudFormationRunner.DeleteStackAsync"/>,
        /// waits for that to complete, then calls <see cref="CloudFormationRunner.CreateStackAsync"/>.
        /// </para>
        /// </summary>
        /// <returns>Operation result.</returns>
        // ReSharper disable once UnusedMember.Global - Public API
        public async Task<CloudFormationResult> ResetStackAsync()
        {
            var previousWaitSetting = this.followOperation;

            // Must wait for delete, irrespective of wait setting
            this.followOperation = true;

            try
            {
                await this.DeleteStackAsync();
                this.followOperation = previousWaitSetting;
                var result = await this.CreateStackAsync();

                if (result.StackOperationResult == StackOperationResult.StackCreated)
                {
                    result.StackOperationResult = StackOperationResult.StackReplaced;
                }

                return result;
            }
            finally
            {
                this.followOperation = previousWaitSetting;
            }
        }

        /// <summary>Updates a stack.</summary>
        /// <param name="confirmationFunc">
        /// A callback that should return <c>true</c> or <c>false</c> as to whether to whether the caller confirms the given change set and therefore to continue with the stack update.
        /// If this parameter is <c>null</c>, then <c>true</c> is assumed.
        /// </param>
        /// <exception cref="StackOperationException">Change set creation failed for reasons other than 'no change'</exception>
        /// <returns>Operation result.</returns>
        public async Task<CloudFormationResult> UpdateStackAsync(Func<DescribeChangeSetResponse, bool> confirmationFunc)
        {
            var stack = await this.stackOperations.GetStackAsync(this.stackName);

            // Check stack state first
            var operationalState = await this.stackOperations.GetStackOperationalStateAsync(stack.StackId);

            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (operationalState)
            {
                case StackOperationalState.Deleting:
                case StackOperationalState.DeleteFailed:

                    throw new StackOperationException(stack, operationalState);

                case StackOperationalState.Broken:

                    this.context.Logger.LogWarning("Stack is in a failed state from previous operation. Update may fail.");
                    break;

                case StackOperationalState.Busy:

                    if (this.waitForInProgressUpdate)
                    {
                        // Track stack until update completes
                        // Wait for previous update to complete
                        // Time from which to start polling events
                        this.lastEventTime = await this.GetMostRecentStackEvent(stack.StackId);

                        this.context.Logger.LogInformation(
                            "Stack {0} is currently being updated by another process",
                            stack.StackName);
                        this.context.Logger.LogInformation("Following its progress while waiting...\n");
                        stack = await this.WaitStackOperationAsync(stack.StackId, false);
                        this.context.Logger.LogInformation(string.Empty);

                        var currentState = await this.stackOperations.GetStackOperationalStateAsync(stack.StackId);

                        if (currentState != StackOperationalState.Ready)
                        {
                            throw new StackOperationException(stack, currentState);
                        }
                    }
                    else
                    {
                        throw new StackOperationException(stack, StackOperationalState.Busy);
                    }

                    break;
            }

            // If we get here, stack is in Ready state
            var changeSetName = CreateChangeSetName();
            var resourcesToImport = await this.GetResourcesToImportAsync();

            var changeSetRequest = new CreateChangeSetRequest
                                       {
                                           ChangeSetName = changeSetName,
                                           ChangeSetType = resourcesToImport != null ? ChangeSetType.IMPORT : ChangeSetType.UPDATE,
                                           Parameters = this.GetStackParametersForUpdate(this.templateResolver, stack),
                                           Capabilities = this.capabilities.Select(c => c.Value).ToList(),
                                           StackName = stack.StackId,
                                           ClientToken = this.clientToken,
                                           RoleARN = this.roleArn,
                                           NotificationARNs = this.notificationARNs,
                                           RollbackConfiguration = this.rollbackConfiguration,
                                           ResourceTypes = this.resourceType,
                                           Tags = this.tags,
                                           ResourcesToImport = resourcesToImport,
                                           TemplateBody = this.templateResolver.ArtifactContent,
                                           TemplateURL =
                                               this.usePreviousTemplate ? null : this.templateResolver.ArtifactUrl,
                                           UsePreviousTemplate = this.usePreviousTemplate,
                                           IncludeNestedStacks = this.includeNestedStacks
                                       };

            this.context.Logger.LogInformation($"Creating changeset {changeSetName} for {this.GetStackNameWithDescription()}");

            if (this.includeNestedStacks)
            {
                this.context.Logger.LogInformation("IncludeNestedChangesets is enabled. This may take some time...");
            }

            var changesetArn = (await this.client.CreateChangeSetAsync(changeSetRequest)).Id;

            var stat = ChangeSetStatus.CREATE_PENDING;
            var describeChangeSetRequest = new DescribeChangeSetRequest { ChangeSetName = changesetArn };

            DescribeChangeSetResponse describeChangeSetResponse = null;

            while (!(stat == ChangeSetStatus.CREATE_COMPLETE || stat == ChangeSetStatus.FAILED))
            {
                Thread.Sleep(this.waitPollTime / 2);
                describeChangeSetResponse = await this.client.DescribeChangeSetAsync(describeChangeSetRequest);
                stat = describeChangeSetResponse.Status;
            }

            if (stat == ChangeSetStatus.FAILED)
            {
                // ReSharper disable once PossibleNullReferenceException - we will go round the above loop at least once
                var reason = describeChangeSetResponse.StatusReason;

                if (reason.Contains("Access Denied") && this.usePreviousTemplate)
                {
                    // Likely that lifecycle policy has removed the previous template.
                    throw new
                        StackOperationException("Unable to create changeset: It is probable that the template has been explicitly deleted or removed by lifecycle policy on your bucket. Please retry specifying the path to the template file");
                }

                if (!NoChangeMessages.Any(msg => reason.StartsWith(msg)))
                {
                    throw new StackOperationException($"Unable to create changeset: {reason}");
                }

                this.context.Logger.LogInformation("No changes to stack were detected.");

                if (this.deleteNoopChangeSet)
                {
                    await this.client.DeleteChangeSetAsync(
                        new DeleteChangeSetRequest { ChangeSetName = changesetArn });

                    this.context.Logger.LogInformation($"Deleted changeset {changeSetName}");
                }

                return new CloudFormationResult
                           {
                               ChangesetResponse = this.deleteNoopChangeSet ? null : describeChangeSetResponse,
                               StackArn = stack.StackId,
                               StackOperationResult = StackOperationResult.NoChange
                           };
            }

            // If we get here, emit details, then apply the changeset.
            // ReSharper disable once PossibleNullReferenceException - we will go round the above loop at least once
            if (this.includeNestedStacks)
            {
                // Base stack
                this.context.Logger.LogInformation($"Root Stack: {this.stackName}");
                this.context.Logger.LogChangeset(describeChangeSetResponse);

                // Walk all nested stacks and emit changes for each
                await EmitNestedStackChangesets(describeChangeSetResponse);
            }
            else
            {
                this.context.Logger.LogChangeset(describeChangeSetResponse);
            }

            if (this.changesetOnly)
            {
                this.context.Logger.LogInformation(
                    // ReSharper disable once PossibleNullReferenceException - 'response' cannot be null. DescribeChangeSetAsync has been called at least once to make it here.
                    $"Changeset {describeChangeSetResponse.ChangeSetName} created for stack {stack.StackName}");
                this.context.Logger.LogInformation("Not updating stack since CreateChangesetOnly = true");
                return new CloudFormationResult
                           {
                               ChangesetResponse = describeChangeSetResponse,
                               StackArn = stack.StackId,
                               StackOperationResult = StackOperationResult.NoChange
                           };
            }

            if (confirmationFunc != null)
            {
                // Confirm the changeset before proceeding
                if (!confirmationFunc(describeChangeSetResponse))
                {
                    return new CloudFormationResult
                               {
                                   ChangesetResponse = describeChangeSetResponse,
                                   StackArn = stack.StackId,
                                   StackOperationResult = StackOperationResult.NoChange
                               };
                }
            }

            // Check nobody else has jumped in before us
            var currentState2 = await this.stackOperations.GetStackOperationalStateAsync(stack.StackId);

            if (currentState2 != StackOperationalState.Ready)
            {
                throw new StackOperationException(stack, currentState2);
            }

            // Time from which to start polling events
            this.lastEventTime = await this.GetMostRecentStackEvent(stack.StackId);

            this.context.Logger.LogInformation($"Updating {this.GetStackNameWithDescription()}\n");

            if (resourcesToImport != null)
            {
                // Have to do this by changeset
                await this.client.ExecuteChangeSetAsync(new ExecuteChangeSetRequest { ChangeSetName = changesetArn });
            }
            else
            {
                await this.client.UpdateStackAsync(await this.GetUpdateRequestWithPolicyFromChangesetRequestAsync(changeSetRequest));
            }

            if (this.followOperation)
            {
                await this.WaitStackOperationAsync(stack.StackId, true);
                return new CloudFormationResult
                           {
                               ChangesetResponse = describeChangeSetResponse,
                               StackArn = stack.StackId,
                               StackOperationResult = StackOperationResult.StackUpdated
                           };
            }

            return new CloudFormationResult
                       {
                           ChangesetResponse = describeChangeSetResponse,
                           StackArn = stack.StackId,
                           StackOperationResult = StackOperationResult.StackUpdateInProgress
                       };

            async Task EmitNestedStackChangesets(DescribeChangeSetResponse parentChangeSetResponse)
            {
                // Recursively discover nested stacks and emit changesets for each
                foreach (var nested in parentChangeSetResponse.Changes.Where(
                    c => c.ResourceChange.ResourceType == "AWS::CloudFormation::Stack"))
                {
                    // Locate nested stack's changeset. It's parent ID will be set to the ID in parentChangeSetResponse
                    var summary = (await this.client.ListChangeSetsAsync(
                                       new ListChangeSetsRequest
                                           {
                                               StackName = nested.ResourceChange.PhysicalResourceId
                                           })).Summaries.First(
                        s => s.ParentChangeSetId == parentChangeSetResponse.ChangeSetId);

                    var nestedResponse = await this.client.DescribeChangeSetAsync(
                                       new DescribeChangeSetRequest
                                           {
                                               ChangeSetName = summary.ChangeSetName,
                                               StackName = nested.ResourceChange.PhysicalResourceId
                                           });

                    this.context.Logger.LogInformation($"Nested Stack: {nested.ResourceChange.LogicalResourceId}");
                    this.context.Logger.LogChangeset(nestedResponse);
                    await EmitNestedStackChangesets(nestedResponse);
                }
            }
        }
    }
}
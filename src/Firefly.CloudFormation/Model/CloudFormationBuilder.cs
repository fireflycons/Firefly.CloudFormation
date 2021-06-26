// ReSharper disable InconsistentNaming
// ReSharper disable StyleCop.SA1309

namespace Firefly.CloudFormation.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Amazon.CloudFormation;
    using Amazon.CloudFormation.Model;

    using Firefly.CloudFormation.Utils;

    /// <summary>
    /// <para>
    /// Fluent builder pattern implementation for <see cref="CloudFormationRunner"/>
    /// </para>
    /// <para>
    /// A builder is constructed by calling the static method <see cref="CloudFormationRunner.Builder"/>
    /// </para>
    /// <example>
    /// <code>
    /// var builder = CloudFormationRunner.Builder(new MyCloudFormationContext(), "my-stack");
    /// var runner = builder
    ///     .WithTemplateLocation("~/repos/my-aws-project/cloudformation.yaml")
    ///     .WithCapabilities(new [] { Capability.CAPABILITY_IAM })
    ///     .WithFollowOperation()
    ///     .Build();
    ///
    /// await runner.CreateStackAsync();
    /// </code>
    /// </example>
    /// </summary>
    public class CloudFormationBuilder
    {
        /// <summary>The CloudFormation context</summary>
        private readonly ICloudFormationContext _cloudFormationContext;

        /// <summary>The stack name</summary>
        private readonly string _stackName;

        /// <summary>The stack capabilities</summary>
        private IEnumerable<Capability> _capabilities = new List<Capability>();

        /// <summary>
        /// Whether to only create change set.
        /// </summary>
        private bool _changesetOnly;

        /// <summary>
        /// The client factory
        /// </summary>
        private IAwsClientFactory _clientFactory;

        /// <summary>
        /// The client token
        /// </summary>
        private string _clientToken;

        /// <summary>Whether to delete a change set that results in no change.</summary>
        private bool _deleteNoopChangeset = true;

        /// <summary>
        /// The disable rollback
        /// </summary>
        private bool _disableRollback;

        /// <summary>Whether to wait for an operation to complete</summary>
        private bool _followOperation;

        /// <summary>
        /// Whether to force upload of local template 
        /// </summary>
        private bool _forceS3;

        /// <summary>
        /// Whether to include nested stacks in changesets
        /// </summary>
        private bool _includeNestedStacks;

        /// <summary>
        /// The notification ARNs
        /// </summary>
        private List<string> _notificationARNs = new List<string>();

        /// <summary>
        /// The on failure action
        /// </summary>
        private OnFailure _onFailure;

        /// <summary>The stack parameters</summary>
        private IDictionary<string, string> _parameters;

        /// <summary>
        /// Resource Imports
        /// </summary>
        private string _resourceImportsLocation;

        /// <summary>
        /// The resources to retain on stack delete.
        /// </summary>
        private List<string> _resourcesToRetain;

        /// <summary>
        /// The template resource types that you have permissions to work with for this action.
        /// </summary>
        private List<string> _resourceTypes = new List<string>();

        /// <summary>
        /// The role ARN
        /// </summary>
        private string _roleARN;

        /// <summary>
        /// The rollback configuration
        /// </summary>
        private RollbackConfiguration _rollbackConfiguration;

        /// <summary>
        /// The stack policy
        /// </summary>
        private string _stackPolicy;

        /// <summary>
        /// The stack policy during update
        /// </summary>
        private string _stackPolicyDuringUpdate;

        /// <summary>The tagging information</summary>
        private List<Tag> _tags = new List<Tag>();

        /// <summary>The template location. Either path o URL</summary>
        private string _templateLocation;

        /// <summary>
        /// The termination protection
        /// </summary>
        private bool _terminationProtection;

        /// <summary>
        /// The timeout in minutes
        /// </summary>
        private int _timeoutInMinutes;

        /// <summary>
        /// <c>true</c> to use previous template for updates.
        /// </summary>
        private bool _usePreviousTemplate;

        /// <summary>
        /// The wait for in progress update
        /// </summary>
        private bool _waitForInProgressUpdate;

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudFormationBuilder"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="stackName">Name of the stack.</param>
        internal CloudFormationBuilder(ICloudFormationContext context, string stackName)
        {
            this._cloudFormationContext = context ?? throw new ArgumentNullException(nameof(context));
            this._stackName = stackName ?? throw new ArgumentNullException(nameof(stackName));

            if (this._cloudFormationContext.Logger == null)
            {
                throw new ArgumentNullException(nameof(context), "context.Logger cannot be null");
            }
        }

        /// <summary>Builds a new <see cref="CloudFormationRunner"/> with the settings applied thus far.</summary>
        /// <returns>A new <see cref="CloudFormationRunner"/></returns>
        public ICloudFormationRunner Build()
        {
            return new CloudFormationRunner(
                this._clientFactory ?? new DefaultClientFactory(this._cloudFormationContext),
                this._stackName,
                this._templateLocation,
                this._parameters,
                this._capabilities,
                this._cloudFormationContext,
                this._tags,
                this._followOperation,
                this._waitForInProgressUpdate,
                this._deleteNoopChangeset,
                this._changesetOnly,
                this._resourceImportsLocation,
                this._roleARN,
                this._clientToken,
                this._notificationARNs,
                this._usePreviousTemplate,
                this._rollbackConfiguration,
                this._stackPolicy,
                this._stackPolicyDuringUpdate,
                this._resourceTypes,
                this._terminationProtection,
                this._onFailure,
                this._timeoutInMinutes,
                this._disableRollback,
                this._resourcesToRetain,
                this._forceS3,
                this._includeNestedStacks);
        }

        /// <summary>In some cases, you must explicitly acknowledge that your stack template contains certain capabilities in order for AWS CloudFormation to create the stack.
        /// See <seealso href="https://docs.aws.amazon.com/AWSCloudFormation/latest/APIReference/API_CreateStack.html"/> for information on when to use capabilities.</summary>
        /// <param name="capabilities">The capabilities.</param>
        /// <returns>The builder</returns>
        public CloudFormationBuilder WithCapabilities(IEnumerable<Capability> capabilities)
        {
            this._capabilities = capabilities == null ? new List<Capability>() : capabilities.ToList();
            return this;
        }

        /// <summary>
        /// Set whether to create change set only without performing update. If <c>true</c> then <see cref="CloudFormationRunner.UpdateStackAsync"/> will return immediately after creation of a change set.
        /// </summary>
        /// <param name="enable">If <c>true</c> (default), execute change set only.</param>
        /// <returns>The builder</returns>
        public CloudFormationBuilder WithChangesetOnly(bool enable = true)
        {
            this._changesetOnly = enable;
            return this;
        }

        /// <summary>
        /// Adds a user supplied client factory.
        /// If a user client factory is not supplied, then a default is created using the credentials, region and endpoint information from the <see cref="ICloudFormationContext"/> passed to the builder's constructor.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <returns>The builder</returns>
        public CloudFormationBuilder WithClientFactory(IAwsClientFactory factory)
        {
            this._clientFactory = factory;
            return this;
        }

        /// <summary>
        /// A unique identifier for this CreateChangeSet request.
        /// Specify this token if you plan to retry requests so that AWS CloudFormation knows that you're not attempting to create another change set with the same name.
        /// You might retry CreateChangeSet requests to ensure that AWS CloudFormation successfully received them.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>The builder.</returns>
        public CloudFormationBuilder WithClientToken(string token)
        {
            this._clientToken = token;
            return this;
        }

        /// <summary>
        /// Set whether to auto-delete change sets that return no change.
        /// The default for this is <c>true</c>, so call this method with <c>false</c> to retain change set for future inspection.
        /// </summary>
        /// <param name="delete">if set to <c>true</c> [delete].</param>
        /// <returns>The builder</returns>
        // ReSharper disable once UnusedMember.Global
        public CloudFormationBuilder WithDeleteNoopChangeset(bool delete = true)
        {
            this._deleteNoopChangeset = delete;
            return this;
        }

        /// <summary>
        /// Disables rollback on create failure, leaving the successfully created resources intact.
        /// </summary>
        /// <returns>The builder</returns>
        /// <param name="enable">If <c>true</c> (default), disable rollback if stack create fails.</param>
        /// <exception cref="ArgumentException">Cannot set DisableRollback when OnFailure has been set</exception>
        public CloudFormationBuilder WithDisableRollback(bool enable)
        {
            this._disableRollback = enable;

            if (this._disableRollback && this._onFailure != null)
            {
                throw new ArgumentException("Cannot set DisableRollback when OnFailure has been set");
            }

            return this;
        }

        /// <summary>
        /// <para>
        /// Sets whether to follow a stack operation.
        /// </para>
        /// <para>
        /// For all stack modifications, if this is set then the method will not return until the stack operation completes,
        /// sending events to the <see cref="ILogger"/> implementation.
        /// </para>
        /// </summary>
        /// <param name="enable">If <c>true</c> (default), wait for stack action to complete.</param>
        /// <returns>The builder</returns>
        public CloudFormationBuilder WithFollowOperation(bool enable = true)
        {
            this._followOperation = enable;
            return this;
        }

        /// <summary>
        /// Set whether to force upload of local template to S3, even if it is less than the maximum size for local templates.
        /// </summary>
        /// <param name="force">if set to <c>true</c>, always upload the template.</param>
        /// <returns>The builder</returns>
        /// <remarks>
        /// At the time of writing there is a possible bug in <c>CreateChangeSetAsync</c> method whereby a socket closed by remote hast exception
        /// may be thrown with a local template. If the template is uploaded to S3 first, then the issue goes away.
        /// </remarks>
        public CloudFormationBuilder WithForceS3(bool force = true)
        {
            this._forceS3 = force;
            return this;
        }

        /// <summary>
        /// Creates a change set for the all nested stacks specified in the template. The default behavior of this action is set to False. To include nested sets in a change set, specify True.
        /// </summary>
        /// <param name="include">if set to <c>true</c> [include].</param>
        /// <returns>The builder</returns>
        public CloudFormationBuilder WithIncludeNestedStacks(bool include = false)
        {
            this._includeNestedStacks = include;
            return this;
        }

        /// <summary>
        /// The Amazon Resource Names (ARNs) of Amazon Simple Notification Service (Amazon SNS) topics that AWS CloudFormation associates with the stack.
        /// To remove all associated notification topics, specify an empty list.
        /// </summary>
        /// <param name="notificationARNs">The notification ARNs.</param>
        /// <returns>The builder</returns>
        public CloudFormationBuilder WithNotificationARNs(IEnumerable<string> notificationARNs)
        {
            this._notificationARNs = notificationARNs == null ? new List<string>() : notificationARNs.ToList();
            return this;
        }

        /// <summary>
        /// Determines what action will be taken if stack creation fails.
        /// </summary>
        /// <param name="onFailure">The action to take on create failure.</param>
        /// <returns>The builder</returns>
        /// <exception cref="ArgumentException">Cannot set OnFailure when DisableRollback is true</exception>
        public CloudFormationBuilder WithOnFailure(OnFailure onFailure)
        {
            if (this._disableRollback && onFailure != null)
            {
                throw new ArgumentException("Cannot set OnFailure when DisableRollback is true");
            }

            this._onFailure = onFailure;
            return this;
        }

        /// <summary>Sets the stack parameters.</summary>
        /// <param name="parameters">Dictionary of <c>ParameterKey</c>, <c>ParameterValue</c> pairs.</param>
        /// <returns>The builder</returns>
        public CloudFormationBuilder WithParameters(IDictionary<string, string> parameters)
        {
            this._parameters = parameters ?? new Dictionary<string, string>();
            return this;
        }

        /// <summary>
        /// Adds resources to import.
        /// See <seealso href="https://docs.aws.amazon.com/AWSCloudFormation/latest/UserGuide/resource-import.html"/> for information on resource import.
        /// </summary>
        /// <param name="resourceImportsLocation">Location of resource imports file (or string content).</param>
        /// <returns>The builder</returns>
        public CloudFormationBuilder WithResourceImports(string resourceImportsLocation)
        {
            this._resourceImportsLocation = resourceImportsLocation;
            return this;
        }

        /// <summary>
        /// Sets the template resource types that you have permissions to work with for this update stack action.
        /// </summary>
        /// <param name="resourceType">Type of the resource.</param>
        /// <returns>The builder</returns>
        public CloudFormationBuilder WithResourceType(IEnumerable<string> resourceType)
        {
            this._resourceTypes = resourceType?.ToList();
            return this;
        }

        /// <summary>
        /// For stacks in the <c>DELETE_FAILED</c> state, a list of resource logical IDs that are associated with the resources you want to retain.
        /// </summary>
        /// <param name="retainResource">The resources to retain.</param>
        /// <returns>The builder</returns>
        public CloudFormationBuilder WithRetainResource(IEnumerable<string> retainResource)
        {
            this._resourcesToRetain = retainResource?.ToList();
            return this;
        }

        /// <summary>
        /// The Amazon Resource Name (ARN) of an AWS Identity and Access Management (IAM) role that AWS CloudFormation assumes when executing the change set.
        /// AWS CloudFormation uses the role's credentials to make calls on your behalf. AWS CloudFormation uses this role for all future operations on the stack.
        /// As long as users have permission to operate on the stack, AWS CloudFormation uses this role even if the users don't have permission to pass it. Ensure that the role grants least privilege.
        /// If you don't specify a value, AWS CloudFormation uses the role that was previously associated with the stack.
        /// If no role is available, AWS CloudFormation uses a temporary session that is generated from your user credentials.
        /// </summary>
        /// <param name="arn">The ARN.</param>
        /// <returns>The builder</returns>
        public CloudFormationBuilder WithRoleArn(string arn)
        {
            this._roleARN = arn;
            return this;
        }

        /// <summary>
        /// Adds a rollback configuration.
        /// See <seealso href="https://docs.aws.amazon.com/AWSCloudFormation/latest/UserGuide/using-cfn-rollback-triggers.html"/>.
        /// </summary>
        /// <param name="rollbackConfiguration">The rollback configuration.</param>
        /// <returns>The builder</returns>
        public CloudFormationBuilder WithRollbackConfiguration(RollbackConfiguration rollbackConfiguration)
        {
            this._rollbackConfiguration = rollbackConfiguration;
            return this;
        }

        /// <summary>
        /// Sets a new stack policy.
        /// See <seealso href="https://docs.aws.amazon.com/AWSCloudFormation/latest/UserGuide/protect-stack-resources.html"/>
        /// </summary>
        /// <param name="stackPolicy">The stack policy as a JSON document, path to a file containing the document or an HTTPS or S3 URL pointing to the document in S3.</param>
        /// <returns>The builder</returns>
        public CloudFormationBuilder WithStackPolicy(string stackPolicy)
        {
            this._stackPolicy = stackPolicy;
            return this;
        }

        /// <summary>
        /// Sets a temporary stack policy for use during an update.
        /// See <seealso href="https://docs.aws.amazon.com/AWSCloudFormation/latest/UserGuide/protect-stack-resources.html"/>
        /// </summary>
        /// <param name="stackPolicy">The stack policy as a JSON document, path to a file containing the document or an HTTPS or S3 URL pointing to the document in S3.</param>
        /// <returns>The builder</returns>
        public CloudFormationBuilder WithStackPolicyDuringUpdate(string stackPolicy)
        {
            this._stackPolicyDuringUpdate = stackPolicy;
            return this;
        }

        /// <summary>Adds stack-level tags which are applied to the stack itself and all resources created.</summary>
        /// <param name="tags">The tags.</param>
        /// <returns>The builder</returns>
        public CloudFormationBuilder WithTags(IEnumerable<Tag> tags)
        {
            this._tags = tags == null ? new List<Tag>() : tags.ToList();
            return this;
        }

        /// <summary>Adds the template location. Either a local path, or an HTTPS or S3 URI pointing to a template in S3.</summary>
        /// <param name="templateLocation">The template location.</param>
        /// <returns>The builder</returns>
        public CloudFormationBuilder WithTemplateLocation(string templateLocation)
        {
            this._templateLocation = templateLocation;
            return this;
        }

        /// <summary>
        /// Enable termination protection (create only)
        /// </summary>
        /// <param name="enable">If <c>true</c> (default), set termination protection on the stack.</param>
        /// <returns>The builder</returns>
        public CloudFormationBuilder WithTerminationProtection(bool enable)
        {
            this._terminationProtection = enable;
            return this;
        }

        /// <summary>
        /// The amount of time that can pass before the stack status becomes CREATE_FAILED (Create only)
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        /// <returns>The builder</returns>
        public CloudFormationBuilder WithTimeoutInMinutes(int timeout)
        {
            this._timeoutInMinutes = timeout;
            return this;
        }

        /// <summary>
        /// Whether to use existing template for stack update.
        /// </summary>
        /// <param name="enable">If <c>true</c> (default), use the previous template stored in CloudFormation to perform an update.</param>
        /// <returns>The builder</returns>
        public CloudFormationBuilder WithUsePreviousTemplate(bool enable = true)
        {
            this._usePreviousTemplate = enable;
            return this;
        }

        /// <summary>
        /// <para>
        /// Sets whether to wait for another operation on this stack which is in progress.
        /// </para>
        /// <para>
        /// For stack updates, should a modification be in progress at the time <see cref="CloudFormationRunner.UpdateStackAsync"/> is called,
        /// then that method will wait for the modification to complete, sending events to the <see cref="ILogger"/> interface prior to creating the update changeset.
        /// </para>
        /// </summary>
        /// <param name="enable">
        /// If <c>true</c> (default), wait for stack action initiated elsewhere to complete,
        /// logging stack events to the given <see cref="ILogger"/> implementation.
        /// </param>
        /// <returns>The builder</returns>
        public CloudFormationBuilder WithWaitForInProgressUpdate(bool enable = true)
        {
            this._waitForInProgressUpdate = enable;
            return this;
        }
    }
}
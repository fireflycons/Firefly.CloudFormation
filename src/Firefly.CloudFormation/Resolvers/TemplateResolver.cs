﻿namespace Firefly.CloudFormation.Resolvers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Amazon.CloudFormation;
    using Amazon.CloudFormation.Model;

    using Firefly.CloudFormation.Model;
    using Firefly.CloudFormation.Utils;

    /// <summary>
    /// Concrete file resolver implementation for CloudFormation template.
    /// </summary>
    /// <seealso cref="AbstractFileResolver" />
    public class TemplateResolver : AbstractFileResolver, ITemplateResolver
    {
        /// <summary>
        /// The stack name
        /// </summary>
        private readonly string stackName;

        /// <summary>
        /// If <c>true</c> then update operations should reuse the existing template that is associated with the stack that you are updating
        /// </summary>
        private readonly bool usePreviousTemplate;

        /// <summary>
        /// The template stage
        /// </summary>
        private readonly TemplateStage templateStage;

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateResolver"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="templateStage">Template stage to retrieve.</param>
        /// <param name="stackName">Name of the stack.</param>
        /// <param name="usePreviousTemplate">if set to <c>true</c> reuse the existing template that is associated with the stack that you are updating.</param>
        /// <param name="forceS3">If set to <c>true</c>, force template upload to S3 even if less than max size.</param>
        public TemplateResolver(ICloudFormationContext context, TemplateStage templateStage, string stackName, bool usePreviousTemplate, bool forceS3)
            : this(new DefaultClientFactory(context), context, templateStage, stackName, usePreviousTemplate, forceS3)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateResolver"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="stackName">Name of the stack.</param>
        /// <param name="usePreviousTemplate">if set to <c>true</c> reuse the existing template that is associated with the stack that you are updating.</param>
        /// <param name="forceS3">If set to <c>true</c>, force template upload to S3 even if less than max size.</param>
        public TemplateResolver(ICloudFormationContext context, string stackName, bool usePreviousTemplate, bool forceS3)
            : this(new DefaultClientFactory(context), context, TemplateStage.Original, stackName, usePreviousTemplate, forceS3)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateResolver"/> class.
        /// </summary>
        /// <param name="clientFactory">The client factory.</param>
        /// <param name="context">The context.</param>
        /// <param name="stackName">Name of the stack.</param>
        /// <param name="usePreviousTemplate">if set to <c>true</c> [use previous template].</param>
        /// <param name="forceS3">If set to <c>true</c>, force template upload to S3 even if less than max size.</param>
        public TemplateResolver(IAwsClientFactory clientFactory, ICloudFormationContext context, string stackName, bool usePreviousTemplate, bool forceS3)
            : this(clientFactory, context, TemplateStage.Original, stackName, usePreviousTemplate, forceS3)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateResolver"/> class.
        /// </summary>
        /// <param name="clientFactory">The client factory.</param>
        /// <param name="context">The context.</param>
        /// <param name="templateStage">Template stage to retrieve.</param>
        /// <param name="stackName">Name of the stack.</param>
        /// <param name="usePreviousTemplate">if set to <c>true</c> [use previous template].</param>
        /// <param name="forceS3">If set to <c>true</c>, force template upload to S3 even if less than max size.</param>
        public TemplateResolver(IAwsClientFactory clientFactory, ICloudFormationContext context, TemplateStage templateStage, string stackName, bool usePreviousTemplate, bool forceS3)
            : base(clientFactory, context)
        {
            this.usePreviousTemplate = usePreviousTemplate;
            this.stackName = stackName;
            this.ForceS3 = forceS3;
            this.templateStage = templateStage;
        }

        /// <inheritdoc cref="ITemplateResolver"/>>
        public List<string> NoEchoParameters { get; private set; } = new List<string>();

        /// <summary>
        /// Gets a value indicating whether to force upload of artifact to S3, even if lass than maximum size.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [force s3]; otherwise, <c>false</c>.
        /// </value>
        protected override bool ForceS3 { get; }

        /// <summary>
        /// Gets the maximum size of the file.
        /// If the file is on local file system and is larger than this number of bytes, it must first be uploaded to S3.
        /// </summary>
        /// <value>
        /// The maximum size of the file.
        /// </value>
        protected override int MaxFileSize => 51200;

        /// <summary>
        /// Resolves and loads the given file from the specified location,
        /// including from CloudFormation itself if called for a UsePreviousTemplate update.
        /// </summary>
        /// <param name="objectLocation">The file location.</param>
        /// <returns>
        /// The file content
        /// </returns>
        public override async Task<string> ResolveFileAsync(string objectLocation)
        {
            if (this.usePreviousTemplate)
            {
                using (var cfn = this.ClientFactory.CreateCloudFormationClient())
                {
                    this.FileContent = (await cfn.GetTemplateAsync(
                                            new GetTemplateRequest
                                                {
                                                    StackName = this.stackName, TemplateStage = this.templateStage
                                                })).TemplateBody;
                    this.Source = InputFileSource.UsePreviousTemplate;

                    this.NoEchoParameters =
                        (await cfn.GetTemplateSummaryAsync(
                             new GetTemplateSummaryRequest { StackName = this.stackName })).Parameters
                        .Where(p => p.NoEcho).Select(p => p.ParameterKey).ToList();
                }

            }
            else
            {
                await base.ResolveFileAsync(objectLocation);
            }

            return this.FileContent;
        }
    }
}
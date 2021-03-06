﻿namespace Firefly.CloudFormation.Resolvers
{
    /// <summary>
    /// Concrete file resolver implementation for stack policy files.
    /// </summary>
    /// <seealso cref="AbstractFileResolver" />
    public class StackPolicyResolver : AbstractFileResolver
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StackPolicyResolver"/> class.
        /// </summary>
        /// <param name="clientFactory">The client factory.</param>
        /// <param name="context">The context.</param>
        public StackPolicyResolver(IAwsClientFactory clientFactory, ICloudFormationContext context)
            : base(clientFactory, context)
        {
        }

        /// <summary>
        /// Gets a value indicating whether to force upload of artifact to S3, even if lass than maximum size.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [force s3]; otherwise, <c>false</c>.
        /// </value>
        protected override bool ForceS3 { get; } = false;

        /// <summary>
        /// Gets the maximum size of the file.
        /// If the file is on local file system and is larger than this number of bytes, it must first be uploaded to S3.
        /// </summary>
        /// <value>
        /// The maximum size of the file.
        /// </value>
        protected override int MaxFileSize { get; } = 16384;
    }
}
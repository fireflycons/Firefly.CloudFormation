namespace Firefly.CloudFormation
{
    using System;

    using Amazon;
    using Amazon.Runtime;

    using Firefly.CloudFormation.S3;
    using Firefly.CloudFormation.Utils;

    /// <summary>
    /// <para>
    /// Sets a context for CloudFormation.
    /// </para>
    /// <para>
    /// The context provides essential information for the operation of this library:
    /// <list type="bullet">
    /// <item>
    /// <description>Valid AWS credentials with rights to run the CloudFormation.</description>
    /// </item>
    /// <item>
    /// <description>Region in which to run the stack.</description>
    /// </item>
    /// <item>
    /// <description>Alternative endpoint URL for CloudFormation service, e.g. if using LocalStack.</description>
    /// </item>
    /// <item>
    /// <description>Logging interface for messages generated within this library.</description>
    /// </item>
    /// <item>
    /// <description>S3 interface to handle the upload of templates or stack policies too large to pass as text.</description>
    /// </item>
    /// </list>
    /// </para>
    /// </summary>
    public interface ICloudFormationContext
    {
        /// <summary>
        /// Gets or sets the AWS credentials.
        /// </summary>
        /// <value>
        /// The credentials.
        /// </value>
        AWSCredentials Credentials { get; set; }

        /// <summary>
        /// Gets or sets the AWS region.
        /// </summary>
        /// <value>
        /// The region.
        /// </value>
        RegionEndpoint Region { get; set; }

        /// <summary>
        /// Gets or sets the custom cloud formation endpoint URL. If unset, then AWS default is used.
        /// </summary>
        /// <value>
        /// The cloud formation endpoint URL.
        /// </value>
        Uri CloudFormationEndpointUrl { get; set; }

        /// <summary>
        /// Gets or sets the logging interface.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        ILogger Logger { get; set; }

        /// <summary>
        /// Gets or sets the S3 utility.
        /// Used for uploading oversize objects to S3
        /// </summary>
        /// <value>
        /// The S3 utility.
        /// </value>
        IS3Util S3Util { get; set; }
    }
}
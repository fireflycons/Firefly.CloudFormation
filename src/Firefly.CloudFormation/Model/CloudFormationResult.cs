﻿namespace Firefly.CloudFormation.Model
{
    using Amazon.CloudFormation.Model;

    /// <summary>
    /// <para>
    /// Result object returned by Create/Update/Delete/Reset methods
    /// </para>
    /// <para>
    /// See also <seealso cref="CloudFormationRunner"/>
    /// </para>
    /// </summary>
    public class CloudFormationResult
    {
        /// <summary>
        /// Gets the final changeset response, where a changeset was created.
        /// </summary>
        /// <value>
        /// The changeset ARN.
        /// </value>
        public DescribeChangeSetResponse ChangesetResponse { get; internal set; }

        /// <summary>
        /// Gets the ARN of the stack being modified.
        /// </summary>
        /// <value>
        /// The stack ARN.
        /// </value>
        public string StackArn { get; internal set; }

        /// <summary>
        /// Gets the stack operation result.
        /// </summary>
        /// <value>
        /// The stack operation result.
        /// </value>
        public StackOperationResult StackOperationResult { get; internal set; }
    }
}
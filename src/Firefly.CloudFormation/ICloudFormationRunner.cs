namespace Firefly.CloudFormation
{
    using System;
    using System.Threading.Tasks;

    using Amazon.CloudFormation.Model;

    using Firefly.CloudFormation.Model;

    /// <summary>
    /// Interface describing <see cref="CloudFormationRunner"/>. Provided for mocking.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public interface ICloudFormationRunner : IDisposable
    {
        /// <summary>Creates a new stack.</summary>
        /// <returns><see cref="Task"/> object so we can await task return.</returns>
        Task<CloudFormationResult> CreateStackAsync();

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
        Task<CloudFormationResult> DeleteStackAsync(Func<bool> invalidRetentionConfirmationFunc = null, Func<bool> queryDeleteStackFunc = null);

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
        Task<CloudFormationResult> ResetStackAsync();

        /// <summary>Updates a stack.</summary>
        /// <param name="confirmationFunc">
        /// A callback that should return <c>true</c> or <c>false</c> as to whether to whether the caller confirms the given change set and therefore to continue with the stack update.
        /// If this parameter is <c>null</c>, then <c>true</c> is assumed.
        /// </param>
        /// <exception cref="StackOperationException">Change set creation failed for reasons other than 'no change'</exception>
        /// <returns>Operation result.</returns>
        Task<CloudFormationResult> UpdateStackAsync(Func<DescribeChangeSetResponse, bool> confirmationFunc);
    }
}
namespace Firefly.CloudFormation.Resolvers
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface defining the template resolver
    /// </summary>
    /// <seealso cref="Firefly.CloudFormation.Resolvers.IInputFileResolver" />
    public interface ITemplateResolver : IInputFileResolver
    {
        /// <summary>
        /// <para>
        /// Gets the <c>NoEcho</c> parameters.
        /// </para>
        /// <para>
        /// For templates read from a deployed stack, this contains the names of any parameters flagged <c>NoEcho</c>.
        /// </para>
        /// </summary>
        /// <value>
        /// The <c>NoEcho</c> parameters.
        /// </value>
        List<string> NoEchoParameters { get; }
    }
}
namespace Firefly.CloudFormation.Parsers
{
    using System.Collections.Generic;

    using Amazon.CloudFormation.Model;

    /// <summary>
    /// Interface describing <see cref="ResourceImportParser"/>
    /// </summary>
    public interface IResourceImportParser
    {
        /// <summary>
        /// Gets the resources to import.
        /// </summary>
        /// <returns>List of resources parsed from import file.</returns>
        List<ResourceToImport> GetResourcesToImport();
    }
}
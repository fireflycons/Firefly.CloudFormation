namespace Firefly.CloudFormation.Parsers
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface describing <see cref="TemplateParser"/>
    /// </summary>
    public interface ITemplateParser
    {
        /// <summary>
        /// Gets the logical resource names.
        /// </summary>
        /// <param name="stackName">Name of the parent stack. Used to prefix nested stack resources</param>
        /// <returns>List of resource names.</returns>
        IEnumerable<string> GetLogicalResourceNames(string stackName);

        /// <summary>
        /// Gets logical resource names of nested stacks declared in the given template
        /// Does not recurse these.
        /// </summary>
        /// <returns>List of nested stack logical resource names, if any.</returns>
        IEnumerable<string> GetNestedStackNames();

        /// <summary>
        /// Gets logical resource names of nested stacks declared in the given template, accounting for how CloudFormation will name them when the template runs.
        /// Does not recurse these.
        /// </summary>
        /// <param name="baseStackName">Name of the base stack</param>
        /// <returns>List of nested stack logical resource names, if any.</returns>
        IEnumerable<string> GetNestedStackNames(string baseStackName);

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <returns>List of <see cref="TemplateFileParameter"/></returns>
        IEnumerable<TemplateFileParameter> GetParameters();

        /// <summary>
        /// Gets the template resources.
        /// </summary>
        /// <returns>Enumerable of resources found in template</returns>
        IEnumerable<ITemplateResource> GetResources();

        /// <summary>
        /// Gets the template description.
        /// </summary>
        /// <returns>Content of description property from template</returns>
        string GetTemplateDescription();

        /// <summary>
        /// Saves the template to the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        void Save(string path);

        /// <summary>
        /// Gets the template by re-serializing the current state of the representation model.
        /// </summary>
        /// <returns>Template body as string</returns>
        string GetTemplate();
    }
}
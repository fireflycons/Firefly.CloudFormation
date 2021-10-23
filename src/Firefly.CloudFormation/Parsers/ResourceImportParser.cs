namespace Firefly.CloudFormation.Parsers
{
    using System.Collections.Generic;
    using System.IO;

    using Amazon.CloudFormation.Model;

    using YamlDotNet.Serialization;

    /// <summary>
    /// <para>
    /// Base class for Resource Import file parsers
    /// </para>
    /// <para>
    /// Resource Import files contain a JSON or YAML list of resource import structures which look like this (JSON)
    /// <code>
    /// [
    ///     {
    ///         "ResourceType":"AWS::DynamoDB::Table",
    ///         "LogicalResourceId":"GamesTable",
    ///         "ResourceIdentifier": {
    ///             "TableName":"Games"
    ///         }
    ///     }
    /// ]
    /// </code>
    /// </para>
    /// <para>
    /// See <seealso href="https://docs.aws.amazon.com/AWSCloudFormation/latest/UserGuide/resource-import-existing-stack.html"/> for information on resource import data structure.
    /// </para>
    /// </summary>
    /// <seealso cref="InputFileParser" />
    public class ResourceImportParser : InputFileParser, IResourceImportParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceImportParser"/> class.
        /// </summary>
        /// <param name="fileContent">Content of the file.</param>
        protected ResourceImportParser(string fileContent)
            : base(fileContent)
        {
        }

        /// <summary>
        /// Creates a parser subclass of the appropriate type for the input content.
        /// </summary>
        /// <param name="fileContent">Content of the file.</param>
        /// <returns>A new <see cref="ResourceImportParser"/>.</returns>
        /// <exception cref="InvalidDataException">Resource import file is empty</exception>
        public static IResourceImportParser Create(string fileContent)
        {
            return new ResourceImportParser(fileContent);
        }

        /// <summary>
        /// Gets the resources to import.
        /// </summary>
        /// <returns>List of resources parsed from import file.</returns>
        public List<ResourceToImport> GetResourcesToImport()
        {
            return new DeserializerBuilder().Build().Deserialize<List<ResourceToImport>>(this.FileContent);
        }
    }
}
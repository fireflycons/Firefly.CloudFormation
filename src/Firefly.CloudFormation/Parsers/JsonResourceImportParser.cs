namespace Firefly.CloudFormation.Parsers
{
    using System.Collections.Generic;

    using Amazon.CloudFormation.Model;

    using Newtonsoft.Json;

    /// <summary>
    /// <para>
    /// Concrete resource import parser for JSON
    /// </para>
    /// <para>JSON resource imports look like this</para>
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
    /// </summary>
    /// <seealso cref="ResourceImportParser" />
    internal class JsonResourceImportParser : ResourceImportParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonResourceImportParser"/> class.
        /// </summary>
        /// <param name="fileContent">Content of the file.</param>
        public JsonResourceImportParser(string fileContent)
            : base(fileContent)
        {
        }

        /// <summary>
        /// Gets the resources to import.
        /// </summary>
        /// <returns>
        /// List of resources parsed from import file.
        /// </returns>
        public override List<ResourceToImport> GetResourcesToImport()
        {
            return JsonConvert.DeserializeObject<List<ResourceToImport>>(this.FileContent);
        }
    }
}
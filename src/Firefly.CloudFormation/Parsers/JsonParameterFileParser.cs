namespace Firefly.CloudFormation.Parsers
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// <para>
    /// Parser for JSON parameter files
    /// </para>
    /// <para>
    /// JSON parameter files look like this
    /// <code>
    /// [
    ///     {
    ///         "ParameterKey": "MyParameterName",
    ///         "ParameterValue": "MyParameterValue"
    ///     }
    /// ]
    /// </code>
    /// </para>
    /// </summary>
    /// <seealso cref="ParameterFileParser" />
    internal class JsonParameterFileParser : ParameterFileParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonParameterFileParser"/> class.
        /// </summary>
        /// <param name="fileContent">Content of the file.</param>
        public JsonParameterFileParser(string fileContent)
            : base(fileContent)
        {
        }

        /// <summary>
        /// Parses a JSON parameter file.
        /// </summary>
        /// <returns>
        /// A dictionary of parameter key-value pairs
        /// </returns>
        public override IDictionary<string, string> ParseParameterFile()
        {
            if (string.IsNullOrEmpty(this.FileContent))
            {
                return new Dictionary<string, string>();
            }

            using (var reader = new StringReader(this.FileContent))
            {
                return ((JArray)JToken.ReadFrom(new JsonTextReader(reader))).Cast<JObject>().ToDictionary(
                    o => o["ParameterKey"].ToString(),
                    o => o["ParameterValue"].ToString());
            }
        }
    }
}
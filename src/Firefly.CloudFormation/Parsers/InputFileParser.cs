namespace Firefly.CloudFormation.Parsers
{
    /// <summary>
    /// Base class for all YAML/JSON parser types
    /// </summary>
    public abstract class InputFileParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InputFileParser"/> class.
        /// </summary>
        /// <param name="fileContent">Content of the file.</param>
        protected InputFileParser(string fileContent)
        {
            this.FileContent = fileContent;
        }

        /// <summary>
        /// Gets the template body.
        /// </summary>
        /// <value>
        /// The template body.
        /// </value>
        protected string FileContent { get; }
    }
}
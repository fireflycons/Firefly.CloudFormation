namespace Firefly.CloudFormation.Parsers
{
    /// <summary>
    /// Parameter data as read from a parameter file
    /// </summary>
    internal class ParameterFileEntry
    {
        /// <summary>
        /// Gets or sets the parameter key.
        /// </summary>
        /// <value>
        /// The parameter key.
        /// </value>
        public string ParameterKey { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the parameter value.
        /// </summary>
        /// <value>
        /// The parameter value.
        /// </value>
        public string ParameterValue { get; set; } = string.Empty;
    }
}
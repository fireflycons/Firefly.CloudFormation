namespace Firefly.CloudFormation.Model
{
    /// <summary>
    /// <para>
    /// Data serialization format for templates, policies etc.
    /// </para>
    /// <para>
    /// See also <seealso cref="Firefly.CloudFormation.Parsers.InputFileParser"/>
    /// </para>
    /// </summary>
    public enum SerializationFormat
    {
        /// <summary>
        /// Format is JSON
        /// </summary>
        Json,

        /// <summary>
        /// Format is YAML
        /// </summary>
        Yaml,

        /// <summary>
        /// Cannot determine format. For input files being parsed, generally indicates empty or whitespace only content.
        /// </summary>
        Unknown
    }
}
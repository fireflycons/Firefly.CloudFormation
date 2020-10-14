namespace Firefly.CloudFormation.Parsers
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface describing <see cref="ParameterFileParser"/>
    /// </summary>
    public interface IParameterFileParser
    {
        /// <summary>
        /// Parses a parameter file.
        /// </summary>
        /// <returns>A dictionary of parameter key-value pairs</returns>
        IDictionary<string, string> ParseParameterFile();
    }
}
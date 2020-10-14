namespace Firefly.CloudFormation.Parsers
{
    using System.Collections.Generic;
    using System.IO;

    using Firefly.CloudFormation.Model;

    /// <summary>
    /// <para>
    /// Base class for parameter file parsers.
    /// </para>
    /// <para>
    /// Parameter files contain a JSON or YAML list of parameter structures with fields <c>ParameterKey</c> and <c>ParameterValue</c>.
    /// This is similar to <c>aws cloudformation create-stack</c>  except the other fields defined for that are currently ignored here.
    /// Parameters not supplied to an update operation are assumed to be <c>UsePreviousValue</c>.
    /// </para>
    /// </summary>
    /// <seealso cref="InputFileParser" />
    public abstract class ParameterFileParser : InputFileParser, IParameterFileParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterFileParser"/> class.
        /// </summary>
        /// <param name="fileContent">Content of the file.</param>
        protected ParameterFileParser(string fileContent)
            : base(fileContent)
        {
        }

        /// <summary>
        /// Creates a parser subclass of the appropriate type for the input content.
        /// </summary>
        /// <param name="templateBody">The template body.</param>
        /// <returns>A new <see cref="ParameterFileParser"/></returns>
        /// <exception cref="InvalidDataException">Parameter file is empty is empty</exception>
        public static IParameterFileParser CreateParser(string templateBody)
        {
            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (InputFileParser.GetInputFileFormat(templateBody))
            {
                case SerializationFormat.Json:

                    return new JsonParameterFileParser(templateBody);

                case SerializationFormat.Yaml:

                    return new YamlParameterFileParser(templateBody);

                default:

                    throw new InvalidDataException("Parameter file is empty");
            }
        }

        /// <summary>
        /// Parses a parameter file.
        /// </summary>
        /// <returns>A dictionary of parameter key-value pairs</returns>
        public abstract IDictionary<string, string> ParseParameterFile();
    }
}
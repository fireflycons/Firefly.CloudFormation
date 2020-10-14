namespace Firefly.CloudFormation.Parsers
{
    using System;

    using Firefly.CloudFormation.Model;

    using Newtonsoft.Json.Linq;

    using YamlDotNet.RepresentationModel;

    /// <summary>
    /// Interface describing <see cref="TemplateResource"/>
    /// </summary>
    public interface ITemplateResource
    {
        /// <summary>
        /// Gets the format of the contained resource.
        /// </summary>
        /// <value>
        /// The resource format.
        /// </value>
        SerializationFormat Format { get; }

        /// <summary>
        /// Gets the logical resource name as read from the template.
        /// </summary>
        /// <value>
        /// Logical resource name.
        /// </value>
        string LogicalName { get; }

        /// <summary>
        /// Gets the type of the resource, e.g. <c>AWS::EC2::Instance</c>.
        /// </summary>
        /// <value>
        /// The type of the resource.
        /// </value>
        string ResourceType { get; }

        /// <summary>
        /// Casts the resource as a <see cref="JObject"/>. The call will fail if the input template was YAML
        /// </summary>
        /// <exception cref="FormatException">Cannot cast YAML resource to JSON</exception>
        /// <returns>JSON resource</returns>
        JProperty AsJson();

        /// <summary>
        /// Casts the resource as a <see cref="YamlMappingNode"/>. The call will fail if the input template was JSON
        /// </summary>
        /// <returns>
        /// YAML object
        /// </returns>
        /// <exception cref="FormatException">Cannot cast JSON resource to YAML</exception>
        YamlMappingNode AsYaml();

        /// <summary>
        /// Gets a resource property value. Currently only leaf nodes (string values) are supported.
        /// </summary>
        /// <param name="propertyPath">The property path.</param>
        /// <returns>The value of the property; else <c>null</c> if the property path did not resolve to a leaf node.</returns>
        string GetResourcePropertyValue(string propertyPath);

        /// <summary>
        /// <para>
        /// Updates a property of a CloudFormation Resource in the loaded template.
        /// </para>
        /// <para>
        /// You would want to do this if you were implementing the functionality of <c>aws cloudformation package</c>
        /// to rewrite local file paths to S3 object locations.
        /// </para>
        /// </summary>
        /// <param name="propertyPath">Path to the property you want to set within this resource's <c>Properties</c> section,
        /// e.g. for a <c>AWS::Glue::Job</c> the path would be <c>Command/ScriptLocation</c>.
        /// </param>
        /// <param name="newValue">The new value.</param>
        /// <exception cref="FormatException">Resource format is unknown (not JSON or YAML)</exception>
        void UpdateResourceProperty(string propertyPath, object newValue);
    }
}
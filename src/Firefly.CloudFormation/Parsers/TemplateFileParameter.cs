namespace Firefly.CloudFormation.Parsers
{
    using System;
    using System.Diagnostics;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Represents a parameter declaration in a CloudFormation template file
    /// </summary>
    [DebuggerDisplay("{Name}: {Type}")]
    public class TemplateFileParameter
    {
        /// <summary>
        /// Gets or sets Allowed Pattern regex for parameter validation
        /// </summary>
        /// <value>
        /// Regular Expression validation pattern.
        /// </value>
        public Regex AllowedPattern { get; set; }

        /// <summary>
        /// Gets or sets Allowed Values for parameter validation
        /// </summary>
        /// <value>
        /// List of values that may be supplied for the parameter.
        /// </value>
        public string[] AllowedValues { get; set; }

        /// <summary>
        /// Gets or sets a string that explains a constraint when the constraint is violated. 
        /// </summary>
        /// <value>
        /// Custom message that CloudFormation displays if a parameter constraint is violated.
        /// </value>
        public string ConstraintDescription { get; set; }

        /// <summary>
        /// Gets or sets parameter default value. When parameter is populated from SSM Parameter Store, this is the parameter path.
        /// </summary>
        /// <value>
        /// Default value substituted if a value is not supplied for the parameter.
        /// </value>
        public string Default { get; set; }

        /// <summary>
        /// Gets or sets the parameter description
        /// </summary>
        /// <value>
        /// Description of the parameter.
        /// </value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has minimum length.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has minimum length; otherwise, <c>false</c>.
        /// </value>
        public bool HasMinLength { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has minimum value.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has minimum value; otherwise, <c>false</c>.
        /// </value>
        public bool HasMinValue { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has maximum length.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has maximum length; otherwise, <c>false</c>.
        /// </value>
        public bool HasMaxLength { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has maximum value.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has maximum value; otherwise, <c>false</c>.
        /// </value>
        public bool HasMaxValue { get; set; }

        /// <summary>
        /// Gets a value indicating whether this parameter is populated from SSM Parameter Store
        /// </summary>
        /// <value>
        /// <c>true</c> if this is an SSM-populated parameter; else <c>false</c>.
        /// </value>
        public bool IsSsmParameter => this.Type?.StartsWith("AWS::SSM::Parameter") ?? false;

        /// <summary>
        /// Gets or sets maximum input length. Only valid when type is String
        /// </summary>
        /// <value>
        /// Max length in characters of the parameter value.
        /// </value>
        public int MaxLength { get; set; } = int.MaxValue;

        /// <summary>
        /// Gets or sets maximum value. Only valid when type is Number
        /// </summary>
        /// <value>
        /// Max numerical value of the parameter.
        /// </value>
        public double MaxValue { get; set; } = double.MaxValue;

        /// <summary>
        /// Gets or sets minimum input length. Only valid when type is String
        /// </summary>
        /// <value>
        /// Min length in characters of the parameter value.
        /// </value>
        public int MinLength { get; set; } = 0;

        /// <summary>
        /// Gets or sets minimum value. Only valid when type is Number
        /// </summary>
        /// <value>
        /// Min numerical value of the parameter.
        /// </value>
        public double MinValue { get; set; } = double.MinValue;

        /// <summary>
        /// Gets or sets the parameter name
        /// </summary>
        /// <value>
        /// The parameter name
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the parameter value should be shown by user interfaces.
        /// </summary>
        /// <value>
        /// If <c>true</c>, user interfaces should redact the parameter value when displaying.
        /// </value>
        public bool NoEcho { get; set; }

        /// <summary>
        /// Gets or sets the parameter type
        /// </summary>
        /// <value>
        /// The AWS Parameter type.
        /// </value>
        public string Type { get; set; }
    }
}
# v2.0 Breaking Changes

v2.0 of this module introduces a completely new template parser I [wrote from scratch](https://github.com/fireflycons/Firefly.CloudFormationParser) and as a result, some of the interfaces to the parser have now changed.

There are no longer separate subclasses for JSON and YAML templates, resulting in some classes being removed.

# Retired Classes and Interfaces

* `ITemplateResource` - Replaced by [IResource](https://fireflycons.github.io/Firefly.CloudFormationParser/api/Firefly.CloudFormationParser.IResource.html).
* `ITemplateParameter` - Replaced by [IParameter](https://fireflycons.github.io/Firefly.CloudFormationParser/api/Firefly.CloudFormationParser.IParameter.html)
* `TemplaterFileParameter` - Removed as its functionality is covered by implementation of [IParameter](https://fireflycons.github.io/Firefly.CloudFormationParser/api/Firefly.CloudFormationParser.IParameter.html) in the [parser module](https://github.com/fireflycons/Firefly.CloudFormationParser)
* `SerializationFormat` - No longer relevant as the parsers are format agnostic.

# Obsolete Methods

* [ParameterFileParser.CreateParser](xref:Firefly.CloudFormation.Parsers.ParameterFileParser.CreateParser(System.String)) - Superseded by [ParameterFileParser.Create](xref:Firefly.CloudFormation.Parsers.ParameterFileParser.Create(System.String)) for consistency with other parser classes. The original implementation still exists, but is marked obsolete.

# Retired Methods

* `InputFileParser.GetInputFileFormat` - Not required as there are no longer separate parsers for YAML and JSON.

# Abstractions and Subclasses Removed

* `TemplateParser` - This is no longer abstract as its subclasses have been removed. [TemplateParser.Create](xref:Firefly.CloudFormation.Parsers.TemplateParser.Create(System.String)) now returns a concrete [TemplateParser](xref:Firefly.CloudFormation.Parsers.TemplateParser) rather than a subclass of it.
* `ParameterFileParser` - This is no longer abstract as its subclasses have been removed. [ParameterFileParser.Create](xref:Firefly.CloudFormation.Parsers.ParameterFileParser.Create(System.String)) now returns a concrete [ParameterFileParser](xref:Firefly.CloudFormation.Parsers.ParameterFileParser) rather than a subclass of it.
* `ResourceImportParser` - This is no longer abstract as its subclasses have been removed. [ResourceImportParser.Create](xref:Firefly.CloudFormation.Parsers.ResourceImportParser.Create(System.String)) now returns a concrete [ResourceImportParser](xref:Firefly.CloudFormation.Parsers.ResourceImportParser) rather than a subclass of it.
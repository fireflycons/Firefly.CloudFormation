# 1.0.8 

* Fix - MaxLength for string params less than min length - introduced by previous change. [Issue link](https://github.com/fireflycons/Firefly.CloudFormation/issues/17)
# 1.0.7

* Fix - Parameter min/max validation is incorrect. [Issue link](https://github.com/fireflycons/Firefly.CloudFormation/issues/16)

# 1.0.6

* Enhancement - Add a couple of new methods for stack examination requied by PSCloudFomation.

# 1.0.5

* Fix - Template parser should handle include transforms without throwing an error. [Issue link](https://github.com/fireflycons/Firefly.CloudFormation/issues/14)

# 1.0.4

* Improvement - S3 Access Denied when using UsePreviousTemplate. [Issue link](https://github.com/fireflycons/Firefly.CloudFormation/issues/13)

# 1.0.3

* Fix - Incorrect handling of DisableRollback parameter. [Issue link](https://github.com/fireflycons/Firefly.CloudFormation/issues/12)

# 1.0.2

* Enhancement - Add additional width to logical ID column when serverless resources are present.

# 1.0.1

* Enhancement - Add [SourceLink](https://github.com/dotnet/sourcelink/blob/main/README.md) support. [Issue link](https://github.com/fireflycons/Firefly.CloudFormation/issues/9)
# 1.0.0

* BREAKING CHANGE- Enhancement - Add new property `ChangesetResponse` to `CloudFormationResult` type returned by methods of `CloudFormationRunner`. This property is populated where a changeset was created and not later deleted by the stack operation. [Issue link](https://github.com/fireflycons/Firefly.CloudFormation/issues/8)

# 0.4.0

* Enhancement - Add support for IncludeNestedChangesets. [Issue link](https://github.com/fireflycons/Firefly.CloudFormation/issues/6)
* Technical - Update AWSSDK.CloudFormation dependency.

# 0.3.2

* Fix - Should not abort operation for "Broken" state. [Issue link](https://github.com/fireflycons/Firefly.CloudFormation/issues/5), for [this PSCloudFormation issue](https://github.com/fireflycons/PSCloudFormation/issues/88)

# 0.3.1

* Fix - Long delays when stack has a long event history. [Issue link](https://github.com/fireflycons/Firefly.CloudFormation/issues/4)

# 0.3.0

* Technical: Build against latest 3.5 AWSSDK

# 0.2.5

* Technical: Extract interfaces from CloudFormationRunner, ParameterFileParser, ResourceImportParser, TempateParser and TemplateResource to improve ability to mock in consuming projects.

# 0.2.4

Fix: Incorrect error message when a template file is not found: `ArgumentException` is thrown with message `Unsupported URI scheme 'file'` when it should be `FileNotFoundException`

# 0.2.3

* Fix - Retrieving template for CREATE_FAILED SAM template stack throws exception. Needed to

Needed for [this PSCloudFormation issue](https://github.com/fireflycons/PSCloudFormation/issues/74)

# 0.2.2

* Add an additional Func parameter to DeleteStackAsync to give clients a way of asking "Do you want to delete the stack" from within the method, such that the correct object is returned.

Needed for [this PSCloudFormation issue](https://github.com/fireflycons/PSCloudFormation/issues/68)

# 0.2.1

* Address potential race condition when monitoring stack events during creation of set of nested stacks. It seems it's possible to retrieve a nested stack resource prior to it being assigned a physical resource ID.

# 0.2.0

Add ForceS3 mechanism

* Possible bug in CloudFormation SDK or service sometimes give a socket closed by remote exception on CreateChangeset even when template < 51,200 bytes. Pushing to S3 first is a suitable workaround.

# 0.1.0

* First release

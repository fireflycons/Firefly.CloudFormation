# Firefly.CloudFormation

[![Build status](https://ci.appveyor.com/api/projects/status/qlsak3rsjx8vypha/branch/master?svg=true)](https://ci.appveyor.com/project/fireflycons/firefly-cloudformation/branch/master)

A library to wrap primarily the main AWSSDK CloudFormation operations [CreateStack](https://docs.aws.amazon.com/sdkfornet/v3/apidocs/items/CloudFormation/MCloudFormationCreateStackCreateStackRequest.html), [CreateChangeset](https://docs.aws.amazon.com/sdkfornet/v3/apidocs/items/CloudFormation/MCloudFormationCreateChangeSetCreateChangeSetRequest.html), [UpdateStack](https://docs.aws.amazon.com/sdkfornet/v3/apidocs/items/CloudFormation/MCloudFormationUpdateStackUpdateStackRequest.html) and [DeleteStack](https://docs.aws.amazon.com/sdkfornet/v3/apidocs/items/CloudFormation/MCloudFormationDeleteStackDeleteStackRequest.html).


These SDK methods all return immediately once the command has been accepted by AWS CloudFormation, and it is up to the caller to manage polling the operation to see when it is complete. Additionally, each of these methods have myriad options that are only valid in certain combinations.

## Features

* Implements a builder pattern to set up the required options for a stack operation in a logical and fluent manner.
* Option to return immediately as per the AWS SDK or to wait (follow) until the operation completes.
* If waiting, then stack is polled for events. The polling includes the events generated by any level of nested stacks and are reported in chronological order across all levels.
* Pass an implemetation of the [ILogger](https://fireflycons.github.io/Firefly-CloudFormation/api/Firefly.CloudFormation.ILogger.html) interface to the builder to receive stack events, changeset data and other information.
* If a modification on a stack is in progess when you make an update call and elect to [follow operations](https://fireflycons.github.io/Firefly-CloudFormation/api/Firefly.CloudFormation.Model.CloudFormationBuilder.html#Firefly_CloudFormation_Model_CloudFormationBuilder_WithFollowOperation_System_Boolean_), then you will receive stack events for the in-progress operation until completion before your change is submitted.
* Pass an implementation of the [IS3Util](https://fireflycons.github.io/Firefly-CloudFormation/api/Firefly.CloudFormation.IS3Util.html) interface to have the library automatically upload oversize (> 51,200 bytes) templates to an S3 location of your choice.
* Pass any of a path to a local file, a string containing a template in JSON or YAML, or the URI of a template in S3 (s3 or https schema) to the [WithTemplateLocation](https://fireflycons.github.io/Firefly-CloudFormation/api/Firefly.CloudFormation.Model.CloudFormationBuilder.html#Firefly_CloudFormation_Model_CloudFormationBuilder_WithTemplateLocation_System_String_) method of the builder, and it will do the right thing!
* Public interface to the template parsers and resolvers, allowing implemesntation of things like `aws cloudformation package`
* Full support for [resource import](https://docs.aws.amazon.com/AWSCloudFormation/latest/UserGuide/resource-import.html).


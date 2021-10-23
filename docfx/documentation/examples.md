# How To Use

1. Create an implementation of [ICloudFormationContext](xref:Firefly.CloudFormation.ICloudFormationContext) which contains data such as AWS credentials, S3 and logging interfaces required for the operation to work.
1. Using this, create a [builder](xref:Firefly.CloudFormation.Model.CloudFormationBuilder) by calling the static method [CloudFormationRunner.Builder](xref:Firefly.CloudFormation.CloudFormationRunner.Builder(Firefly.CloudFormation.ICloudFormationContext,System.String)), passing it your context and the name of a CloudFormation Stack. The builder contains methods for setting all the options required for a stack operation.
1. Call the [Build](xref:Firefly.CloudFormation.Model.CloudFormationBuilder.Build) method which retuns a [CloudFormationRunner](xref:Firefly.CloudFormation.CloudFormationRunner) that exposes the methods for creating, updating and deleting stacks.

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

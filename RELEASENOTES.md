# 0.2.0

Add ForceS3 mechanism

Possible bug in CloudFormation SDK or service sometimes give a socket closed by remote exception on CreateChangeset even when template < 51,200 bytes. Pushing to S3 first is a suitable workaround.

# 0.1.0

First release

namespace Firefly.CloudFormation
{
    using System;
    using System.Threading.Tasks;

    using Firefly.CloudFormation.Model;

    /// <summary>
    /// Interface describing a method that CloudFormation operations can use to upload oversize content
    /// (template or policy document) to S3. If an implementation of this interface is not provided to the <see cref="CloudFormationBuilder"/>,
    /// then an attempt to run with oversize content (e.g. template body > 51,200 bytes), then the operation will fail.
    /// </summary>
    public interface IS3Util
    {
        /// <summary>
        /// <para>
        /// Uploads oversize content (template or policy) to S3.
        /// </para>
        /// <para>
        /// This method will be called by create/update operations to upload oversize content to S3.
        /// It is the responsibility of the implementation to provide the destination bucket.
        /// </para>
        /// <para>
        /// <example>
        /// Example implementation
        /// <code>
        /// private IAmazonS3 s3Client;
        /// private string BucketName;
        /// private ILogger logger;
        ///
        /// public async Task&lt;Uri&gt; UploadOversizeArtifactToS3(
        ///     string stackName,
        ///     string body,
        ///     string originalFilename,
        ///     UploadFileType uploadFileType)
        /// {
        ///     var ms = new MemoryStream(
        ///         new UTF8Encoding().GetBytes(body ?? throw new ArgumentNullException(nameof(body))));
        ///
        ///     var key = uploadFileType == UploadFileType.Template
        ///                   ? $"_{stackName}_template_{originalFilename}"
        ///                   : $"_{stackName}_policy_{originalFilename}";
        ///
        ///     var ub = new UriBuilder($"https://{this.BucketName}.s3.amazonaws.com") { Path = $"/{key}" };
        ///
        ///     this.logger.LogInformation($"Copying oversize {uploadFileType.ToString().ToLower()} to {ub.Uri}");
        ///
        ///     await this.s3Client.PutObjectAsync(
        ///         new PutObjectRequest
        ///             {
        ///                 BucketName = this.BucketName,
        ///                 Key = key,
        ///                 AutoCloseStream = true,
        ///                 InputStream = ms
        ///             });
        ///
        ///     return ub.Uri;
        /// }
        /// </code>
        /// </example>
        /// </para>
        /// </summary>
        /// <param name="stackName">Name of the stack. Use to form part of the S3 key.</param>
        /// <param name="body">String content to be uploaded.</param>
        /// <param name="originalFilename">File name of original input file, or <c>"RawString"</c> if the input was a string rather than a file.</param>
        /// <param name="uploadFileType">Type of file (template or policy). Could be used to form part of the S3 key.</param>
        /// <returns>URI of uploaded template.</returns>
        Task<Uri> UploadOversizeArtifactToS3(
            string stackName,
            string body,
            string originalFilename,
            UploadFileType uploadFileType);

        /// <summary>
        /// Gets the content of an S3 object.
        /// </summary>
        /// <param name="bucketName">Name of the bucket.</param>
        /// <param name="key">The key.</param>
        /// <returns>Object contents</returns>
        Task<string> GetS3ObjectContent(string bucketName, string key);
    }
}
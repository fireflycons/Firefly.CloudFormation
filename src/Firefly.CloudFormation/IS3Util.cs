namespace Firefly.CloudFormation
{
    using System;
    using System.Threading.Tasks;

    using Firefly.CloudFormation.Model;
    using Firefly.CloudFormation.Resolvers;

    /// <summary>
    /// <para>
    /// Interface describing methods that CloudFormation operations can use to upload oversize content
    /// (template or policy document) to S3 and to download template content from S3.
    /// </para>
    /// <para>
    /// If an implementation of this interface is not provided to the <see cref="CloudFormationBuilder"/>,
    /// then an attempt to run with oversize content (e.g. template body > 51,200 bytes) or to refer to a template in S3, then the operation will fail.
    /// </para>
    /// </summary>
    /// <example>
    /// <code>
    /// class S3Util: IS3Util
    /// {
    ///     // Constructor implementation omitted.
    ///     private IAmazonS3 s3Client;
    ///     private string BucketName;
    ///     private ILogger logger;
    ///
    ///     public async Task&lt;Uri&gt; UploadOversizeArtifactToS3(
    ///         string stackName,
    ///         string body,
    ///         string originalFilename,
    ///         UploadFileType uploadFileType)
    ///     {
    ///         var ms = new MemoryStream(
    ///             new UTF8Encoding().GetBytes(
    ///                 body ?? throw new ArgumentNullException(nameof(body))));
    ///
    ///         var key = uploadFileType == UploadFileType.Template
    ///                       ? $"_{stackName}_template_{originalFilename}"
    ///                       : $"_{stackName}_policy_{originalFilename}";
    ///
    ///         var ub = new UriBuilder($"https://{this.BucketName}.s3.amazonaws.com")
    ///                  {
    ///                      Path = $"/{key}"
    ///                  };
    ///
    ///         this.logger.LogInformation(
    ///             $"Copying oversize {uploadFileType.ToString().ToLower()} to {ub.Uri}");
    ///
    ///         await this.s3Client.PutObjectAsync(
    ///             new PutObjectRequest
    ///                 {
    ///                     BucketName = this.BucketName,
    ///                     Key = key,
    ///                     AutoCloseStream = true,
    ///                     InputStream = ms
    ///                 });
    ///
    ///         return ub.Uri;
    ///     }
    ///
    ///     public async Task&lt;string&gt; GetS3ObjectContent(string bucketName, string key)
    ///     {
    ///         using (var response = await this.s3.GetObjectAsync(
    ///             new GetObjectRequest {
    ///                 BucketName = bucketName, Key = key }))
    ///         {
    ///             using (var sr = new StreamReader(response.ResponseStream))
    ///             {
    ///                 return sr.ReadToEnd();
    ///             }
    ///         } 
    ///     }
    /// }
    /// </code>
    /// </example>
    public interface IS3Util
    {
        /// <summary>
        /// <para>
        /// Uploads oversize content (template or policy) to S3.
        /// </para>
        /// <para>
        /// This method is called by <see cref="AbstractFileResolver"/> when <see cref="CloudFormationRunner.CreateStackAsync"/>
        /// or <see cref="CloudFormationRunner.UpdateStackAsync"/> resolve local content and find it to be above the maximum permitted
        /// size, so need to upload it to S3.
        /// </para>
        /// <para>
        /// It is the responsibility of the implementation to provide the destination bucket.
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
        /// <para>
        /// Gets the content of an S3 object.
        /// </para>
        /// <para>
        /// This method is called by <see cref="AbstractFileResolver"/> to read templates and policies for which an S3 location has been specified.
        /// </para>
        /// </summary>
        /// <param name="bucketName">Name of the bucket.</param>
        /// <param name="key">The key.</param>
        /// <returns>Object contents</returns>
        Task<string> GetS3ObjectContent(string bucketName, string key);
    }
}
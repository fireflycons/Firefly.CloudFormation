namespace Firefly.CloudFormation.Tests.Unit.CloudFormation
{
    using System;

    using Firefly.EmbeddedResourceLoader;
    using Firefly.EmbeddedResourceLoader.Materialization;

    /// <summary>
    /// Fixture for <see cref="CloudFormationRunner"/> tests.
    /// Loads embedded resources used by these tests
    /// </summary>
    /// <seealso cref="Firefly.EmbeddedResourceLoader.AutoResourceLoader" />
    /// <seealso cref="System.IDisposable" />
    public class TestStackFixture : AutoResourceLoader,  IDisposable
    {
        /// <summary>
        /// Gets or sets the template body as a string.
        /// </summary>
        [EmbeddedResource("test-stack.json")]
        public string TestStackJsonString { get; set; }

        /// <summary>
        /// Gets or sets the the oversized template body as a string.
        /// </summary>
        [EmbeddedResource("test-oversize.json")]
        public string TestOversizedStackJsonString { get; set; }

        /// <summary>
        /// Gets or sets the the oversized template body as a file.
        /// </summary>
        [EmbeddedResource("test-oversize.json")]
        public TempFile TestOversizedStackJsonTemplate { get; set; }

        /// <summary>
        /// Gets or sets the template body as a file.
        /// </summary>
        [EmbeddedResource("test-stack.json")]
        public TempFile TestStackJsonTemplate { get; set; }

        /// <summary>
        /// Removes the temporary resource files.
        /// </summary>
        public void Dispose()
        {
            this.TestStackJsonTemplate?.Dispose();
            this.TestOversizedStackJsonTemplate?.Dispose();
        }
    }
}
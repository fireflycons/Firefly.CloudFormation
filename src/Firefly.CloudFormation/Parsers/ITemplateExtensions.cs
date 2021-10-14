namespace Firefly.CloudFormation.Parsers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Firefly.CloudFormationParser;

    /// <summary>
    /// Extension methods for <c>ITemplate</c>
    /// </summary>
    public static class ITemplateExtensions
    {
        /// <summary>
        /// Amount of padding to add to resource names to include random chars added by CloudFormation
        /// </summary>
        internal const int NestedStackPadWidth = 14;

        /// <summary>
        /// The nested stack type
        /// </summary>
        internal const string NestedStackType = "AWS::CloudFormation::Stack";

        /// <summary>
        /// Gets the logical resource names.
        /// </summary>
        /// <param name="self">This template</param>
        /// <param name="stackName">Name of the parent stack. Used to prefix nested stack resources</param>
        /// <returns>
        /// List of resource names.
        /// </returns>
        public static IEnumerable<string> GetLogicalResourceNames(this ITemplate self, string stackName)
        {
            if (self == null)
            {
                throw new ArgumentNullException(nameof(self), "Argument cannot be null");
            }

            var resourceNames = new List<string> { stackName };

            foreach (var resource in self.Resources)
            {
                var resourceName = resource.Name;

                if (resource.Type != null && resource.Type == NestedStackType)
                {
                    resourceNames.Add(stackName + "-" + resourceName + string.Empty.PadRight(NestedStackPadWidth));
                }
                else
                {
                    resourceNames.Add(resourceName);
                }
            }

            return resourceNames;
        }

        /// <summary>
        /// Gets the template description.
        /// </summary>
        /// <param name="self">This template</param>
        /// <returns>
        /// Content of description property from template
        /// </returns>
        public static string GetTemplateDescription(this ITemplate self)
        {
            if (self == null)
            {
                throw new ArgumentNullException(nameof(self), "Argument cannot be null");
            }

            return self.Description;
        }

        /// <summary>
        /// Gets logical resource names of nested stacks declared in the given template, accounting for how CloudFormation will name them when the template runs.
        /// Does not recurse these.
        /// </summary>
        /// <param name="self">This template</param>
        /// <param name="baseStackName">Name of the base stack</param>
        /// <returns>
        /// List of nested stack logical resource names, if any.
        /// </returns>
        public static IEnumerable<string> GetNestedStackNames(this ITemplate self, string baseStackName)
        {
            var nestedStacks = new List<string>();

            foreach (var resource in self.Resources.Where(r => r.Type == NestedStackType))
            {
                if (!string.IsNullOrEmpty(baseStackName))
                {
                    nestedStacks.Add(baseStackName + "-" + resource.Name + string.Empty.PadRight(NestedStackPadWidth));
                }
                else
                {
                    nestedStacks.Add(resource.Name);
                }
            }

            return nestedStacks;
        }
    }
}
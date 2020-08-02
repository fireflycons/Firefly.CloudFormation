namespace Firefly.CloudFormation.Model
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    using Amazon.CloudFormation.Model;

    /// <summary>
    /// Thrown when an error with a stack operation is detected.
    /// </summary>
    /// <seealso cref="System.Exception" />
    [Serializable]
    public class StackOperationException : Exception
    {
        /// <summary>
        /// The operational state messages
        /// </summary>
        private static readonly Dictionary<StackOperationalState, string> OperationalStateMessages =
            new Dictionary<StackOperationalState, string>
                {
                    { StackOperationalState.Busy, "Stack is being modified by another process." },
                    { StackOperationalState.Broken, "Stack is broken. Please check in AWS Console and fix." },
                    {
                        StackOperationalState.DeleteFailed,
                        "Stack is in DELETE_FAILED state. Try deleting with Retain Resource."
                    },
                    { StackOperationalState.Deleting, "Stack is being deleted by another process." },
                    { StackOperationalState.NotFound, "Stack does not exist." },
                    { StackOperationalState.Ready, "Stack is ready." },
                    { StackOperationalState.Exists, "A stack with this name already exists." }
                };

        /// <summary>
        /// Initializes a new instance of the <see cref="StackOperationException"/> class.
        /// </summary>
        /// <param name="stack">The stack.</param>
        public StackOperationException(Stack stack)
            : base($"Stack '{stack.StackName}': Operation failed. Status is {stack.StackStatus}")
        {
            this.Stack = stack;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StackOperationException"/> class.
        /// </summary>
        /// <param name="stack">The stack.</param>
        /// <param name="state">The operational state.</param>
        public StackOperationException(Stack stack, StackOperationalState state)
            : base(OperationalStateMessages[state])
        {
            this.Stack = stack;
            this.OperationalState = state;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StackOperationException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public StackOperationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StackOperationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public StackOperationException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StackOperationException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"></see> that contains contextual information about the source or destination.</param>
        protected StackOperationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Gets the stack object if one was passed to the constructor.
        /// </summary>
        /// <value>
        /// The stack.
        /// </value>
        public Stack Stack { get; }

        /// <summary>
        /// Gets the operational state of the stack.
        /// </summary>
        /// <value>
        /// The state of the operational.
        /// </value>
        public StackOperationalState OperationalState { get; }
    }
}
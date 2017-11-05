using System;
using System.Runtime.Serialization;

namespace Doctrina.Math.LinearAlgebra
{
    /// <inheritdoc />
    internal class SingularMatrixException : Exception
    {
        public SingularMatrixException()
        {
        }

        public SingularMatrixException(string message) : base(message)
        {
        }

        public SingularMatrixException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SingularMatrixException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
using System;
using System.Collections.Generic;
using System.Text;

namespace ESRI.ArcGIS.Client.Toolkit.DataSources.Kml.Zip
{
    ///// <summary>
    ///// Base exception type for all custom exceptions in the Zip library. It acts as a marker class.
    ///// </summary>
    //[AttributeUsage(AttributeTargets.Class)]
    //public class ZipExceptionAttribute : Attribute { }



    /// <summary>
    /// Indicates that a read was attempted on a stream, and bad or incomplete data was
    /// received.  
    /// </summary>
    internal class BadReadException : ZipException
    {
        /// <summary>
        /// Default ctor.
        /// </summary>
        public BadReadException() { }

        /// <summary>
        /// Come on, you know how exceptions work. Why are you looking at this documentation?
        /// </summary>
        /// <param name="message">The message in the exception.</param>
        public BadReadException(String message)
            : base(message)
        { }

        /// <summary>
        /// Come on, you know how exceptions work. Why are you looking at this documentation?
        /// </summary>
        /// <param name="message">The message in the exception.</param>
        /// <param name="innerException">The innerException for this exception.</param>
        public BadReadException(String message,
            Exception innerException)
            : base(message, innerException)
        {
		}

#if !SILVERLIGHT
        /// <summary>
        /// Come on, you know how exceptions work. Why are you looking at this documentation?
        /// </summary>
        /// <param name="serializationInfo">The serialization info for the exception.</param>
        /// <param name="streamingContext">The streaming context from which to deserialize.</param>
        protected BadReadException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
          {  }        
#endif

	}

    /// <summary>
    /// Issued when an CRC check fails upon extracting an entry from a zip archive.
    /// </summary>
	internal class BadCrcException : ZipException
    {
        /// <summary>
        /// Default ctor.
        /// </summary>
        public BadCrcException() { }

        /// <summary>
        /// Come on, you know how exceptions work. Why are you looking at this documentation?
        /// </summary>
        /// <param name="message">The message in the exception.</param>
        public BadCrcException(String message)
            : base(message)
        { }

        /// <summary>
        /// Come on, you know how exceptions work. Why are you looking at this documentation?
        /// </summary>
        /// <param name="message">The message in the exception.</param>
        /// <param name="innerException">The innerException for this exception.</param>
        public BadCrcException(String message,
            Exception innerException)
            : base(message, innerException)
        {
		}

#if !SILVERLIGHT
        /// <summary>
        /// Come on, you know how exceptions work. Why are you looking at this documentation?
        /// </summary>
        /// <param name="serializationInfo">The serialization info for the exception.</param>
        /// <param name="streamingContext">The streaming context from which to deserialize.</param>
        protected BadCrcException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
          {  }
#endif

	}

    /// <summary>
    /// Base class for all exceptions defined by and throw by the Zip library.
    /// </summary>
	internal class ZipException : Exception
    {
        /// <summary>
        /// Default ctor.
        /// </summary>
        public ZipException() { }

        /// <summary>
        /// Come on, you know how exceptions work. Why are you looking at this documentation?
        /// </summary>
        /// <param name="message">The message in the exception.</param>
        public ZipException(String message) : base(message) { }

        /// <summary>
        /// Come on, you know how exceptions work. Why are you looking at this documentation?
        /// </summary>
        /// <param name="message">The message in the exception.</param>
        /// <param name="innerException">The innerException for this exception.</param>
        public ZipException(String message,
            Exception innerException)
            : base(message, innerException)
        { }

#if !SILVERLIGHT
        /// <summary>
        /// Come on, you know how exceptions work. Why are you looking at this documentation?
        /// </summary>
        /// <param name="serializationInfo">The serialization info for the exception.</param>
        /// <param name="streamingContext">The streaming context from which to deserialize.</param>
        protected ZipException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        { }
#endif

	}

}

﻿namespace FileSnap.Core.Exceptions;

/// <summary>
/// Represents errors that occur during snapshot operations.
/// </summary>
public class SnapshotException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SnapshotException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public SnapshotException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SnapshotException"/> class with a specified error message and a reference to the inner
    /// exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="inner">The exception that is the cause of the current exception.</param>
    public SnapshotException(string message, Exception inner) : base(message, inner) { }
}


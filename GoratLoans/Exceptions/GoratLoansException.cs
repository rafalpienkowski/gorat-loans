using System.Runtime.Serialization;

namespace GoratLoans.Exceptions;

[Serializable]
public class GoratLoansException : Exception
{
    public GoratLoansException()
    {
    }

    public GoratLoansException(string message) : base(message)
    {
    }

    public GoratLoansException(string message, Exception inner) : base(message, inner)
    {
    }
    
    protected GoratLoansException(SerializationInfo info, StreamingContext context) 
        : base(info, context)
    {
    }
}
using System;

namespace Enigmatic.DynamicStateSystem
{
    public class InvalidAddedStateException : Exception
    {
        public InvalidAddedStateException() : base() { }
        public InvalidAddedStateException(string massage) : base(massage) { }
    }

    public class InvalidSwichedStateException : Exception
    {
        public InvalidSwichedStateException() : base() { }
        public InvalidSwichedStateException(string massage) : base(massage) { }
    }

    public class InvalidAddedTransitionException : Exception
    {
        public InvalidAddedTransitionException() : base() { }
        public InvalidAddedTransitionException(string massage) : base(massage) { }
    }

    public class InvalidBindingException : Exception
    {
        public InvalidBindingException() : base() { }
        public InvalidBindingException(string massage) : base(massage) { }
    }
}
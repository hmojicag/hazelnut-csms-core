namespace Hazelnut.CLIApp.Exceptions {
    using System;

    public class InvalidUserParamsException : Exception {
        public InvalidUserParamsException(string msg) : base (msg) {  }
    }
}
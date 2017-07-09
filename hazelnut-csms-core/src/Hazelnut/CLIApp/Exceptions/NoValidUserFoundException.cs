namespace Hazelnut.CLIApp.Exceptions {
    using System;

    public class NoValidUserFoundException : Exception {
        public NoValidUserFoundException(string msg) : base (msg) {  }
    }
}
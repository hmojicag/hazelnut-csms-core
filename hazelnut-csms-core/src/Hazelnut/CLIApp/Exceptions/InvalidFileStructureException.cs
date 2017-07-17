namespace Hazelnut.CLIApp.Exceptions {
    using System;
    public class InvalidFileStructureException : Exception {
        public InvalidFileStructureException(string msg) : base(msg) { }
    }
}
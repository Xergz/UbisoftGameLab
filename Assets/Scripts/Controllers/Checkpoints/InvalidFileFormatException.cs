using System;


public class InvalidFileFormatException : Exception
{
    public InvalidFileFormatException () : base("Invalid file format")
    {
    }
}



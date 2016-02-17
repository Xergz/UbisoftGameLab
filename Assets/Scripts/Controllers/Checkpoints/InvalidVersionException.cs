using System;

public class InvalidVersionException : Exception
{
    private byte loadedVersion;
    private byte supportedVersion;

    public byte LoadedVersion {
        get { 
            return this.loadedVersion;
        }
    }

    public byte SupportedVersion {
        get {
            return this.supportedVersion;
        }
    }

    public InvalidVersionException (byte loaded, byte supported)
        : base("Unsupported file version") {
        this.supportedVersion = supported;
        this.loadedVersion = loaded;
    }
}

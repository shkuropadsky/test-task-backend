

public class StatQueryTaskProgressEventArgs : EventArgs
{
    public int Percent { get; }
    public string Message { get; }
    public StatQueryTaskProgressEventArgs(int percent, string message)
    {
        Percent = percent;    
        Message = message;
    } 
}
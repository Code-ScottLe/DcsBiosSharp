namespace DcsBiosSharp.Connection
{
    public interface IDcsBiosCommand
    {
        string CommandIdentifier
        {
            get;
        }

        string Arguments
        {
            get;
        }
    }
}

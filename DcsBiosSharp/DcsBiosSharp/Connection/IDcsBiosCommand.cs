namespace DcsBiosSharp.Connection
{
    public interface IDcsBiosCommand
    {
        string Name
        {
            get;
        }

        string Arguments
        {
            get;
        }
    }
}

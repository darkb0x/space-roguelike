namespace Game
{
    public interface IEntryComponent
    {
        public void Initialize();
    }
    public interface IEntryComponent<TArg>
    {
        public void Initialize(TArg arg);
    }
    public interface IEntryComponent<TArg1, TArg2>
    {
        public void Initialize(TArg1 arg, TArg2 arg2);
    }
    public interface IEntryComponent<TArg1, TArg2, TArg3>
    {
        public void Initialize(TArg1 arg, TArg2 arg2, TArg3 arg3);
    }
}

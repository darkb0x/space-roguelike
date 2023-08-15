namespace Game.Input
{
    using ActionsMap;

    public abstract class InputHandler
    {
        protected readonly InputActionsMap _inputActions;

        public InputHandler(InputActionsMap inputActions)
        {
            _inputActions = inputActions;
        }

        public abstract void SetActive(bool active);
        public abstract void Dispose();
    }
}

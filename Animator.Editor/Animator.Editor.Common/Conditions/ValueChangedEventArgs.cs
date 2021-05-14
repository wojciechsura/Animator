namespace Animator.Editor.Common.Conditions
{
    public class ValueChangedEventArgs
    {
        // Public methods ------------------------------------------------------

        public ValueChangedEventArgs(bool newValue)
        {
            Value = newValue;
        }

        // Public properties ---------------------------------------------------

        public bool Value { get; }
    }
}
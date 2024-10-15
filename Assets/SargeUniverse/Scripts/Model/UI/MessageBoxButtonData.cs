using System;

namespace SargeUniverse.Scripts.Model.UI
{
    public class MessageBoxButtonData
    {
        public readonly string labelText;
        public readonly Action callback;

        public MessageBoxButtonData(string text, Action action)
        {
            labelText = text;
            callback = action;
        }
    }
}
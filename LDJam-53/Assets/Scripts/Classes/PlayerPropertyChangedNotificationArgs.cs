namespace DefaultNamespace
{
    using System;


    public class PlayerPropertyChangedNotificationArgs : EventArgs
    {
        public string PropertyName { get; set; }

        public object NewValue { get; set; }
    }
}
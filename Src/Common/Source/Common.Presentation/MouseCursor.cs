using System;
using System.Windows.Input;

namespace NuPattern.Presentation
{
    /// <summary>
    /// Defines methods to manage the cursor.
    /// </summary>
    public sealed class MouseCursor : IDisposable
    {
        private Cursor previousCursor;

        /// <summary>
        /// Initializes a new instance of the <see cref="MouseCursor"/> class.
        /// </summary>
        /// <param name="cursor">The cursor.</param>
        public MouseCursor(Cursor cursor)
        {
            this.previousCursor = InputManager.Current.PrimaryMouseDevice.OverrideCursor;
            InputManager.Current.PrimaryMouseDevice.OverrideCursor = cursor;
        }

        /// <summary>
        /// Gets the current cursor.
        /// </summary>
        public static Cursor CurrentCursor
        {
            get { return InputManager.Current.PrimaryMouseDevice.OverrideCursor; }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            InputManager.Current.PrimaryMouseDevice.OverrideCursor = this.previousCursor;
        }
    }
}
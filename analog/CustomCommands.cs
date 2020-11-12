using System.Windows.Input;

namespace Analog
{
    public static class CustomCommands
    {
        public static readonly RoutedUICommand ExecuteQuery = new RoutedUICommand(
            "Execute Query",
            "ExecuteQuery",
            typeof(CustomCommands),
            new InputGestureCollection() { new KeyGesture(Key.F5) }
        );

        public static readonly RoutedUICommand Exit = new RoutedUICommand(
            "Exit",
            "Exit",
            typeof(CustomCommands),
            new InputGestureCollection() { new KeyGesture(Key.F4, ModifierKeys.Alt) }
        );
    }
}

using System.Windows;
using System.Windows.Input;
using form = System.Windows.Forms;
using System.Windows.Media;
using Telerik.Windows.Controls;

namespace ExtraClub.UIControls
{
    /// <remarks>
    /// The FilenameProperty can be at a higher level of the visual tree than
    /// the KeywordProperty, so you don't need to set the filename each time.
    /// </remarks>
    public static class Help
    {
        /// <summary>
        /// Initialize a new instance of <see cref="Help"/>.
        /// </summary>
        static Help()
        {
            // Rather than having to manually associate the Help command, let's take care
            // of this here.
            CommandManager.RegisterClassCommandBinding(typeof(FrameworkElement),
            new CommandBinding(ApplicationCommands.Help,
            new ExecutedRoutedEventHandler(Executed),
            new CanExecuteRoutedEventHandler(CanExecute)));
        }

        #region Filename

        /// <summary>
        /// Filename Attached Dependency Property
        /// </summary>
        public static readonly DependencyProperty FilenameProperty =
        DependencyProperty.RegisterAttached("Filename", typeof(string), typeof(Help));

        /// <summary>
        /// Gets the Filename property.
        /// </summary>
        public static string GetFilename(DependencyObject d)
        {
            return (string)d.GetValue(FilenameProperty);
        }

        /// <summary>
        /// Sets the Filename property.
        /// </summary>
        public static void SetFilename(DependencyObject d, string value)
        {
            d.SetValue(FilenameProperty, value);
        }

        #endregion

        #region Keyword

        /// <summary>
        /// Keyword Attached Dependency Property
        /// </summary>
        public static readonly DependencyProperty KeywordProperty =
        DependencyProperty.RegisterAttached("Keyword", typeof(string), typeof(Help));

        /// <summary>
        /// Gets the Keyword property.
        /// </summary>
        public static string GetKeyword(DependencyObject d)
        {
            return (string)d.GetValue(KeywordProperty);
        }

        /// <summary>
        /// Sets the Keyword property.
        /// </summary>
        public static void SetKeyword(DependencyObject d, string value)
        {
            d.SetValue(KeywordProperty, value);
        }
        #endregion

        #region Helpers
        private static void CanExecute(object sender, CanExecuteRoutedEventArgs args)
        {
            FrameworkElement el = sender as FrameworkElement;
            if (el != null)
            {
                string fileName = FindFilename(el);
                if (!string.IsNullOrEmpty(fileName))
                    args.CanExecute = true;
            }
        }

        private static void Executed(object sender, ExecutedRoutedEventArgs args)
        {
            // Call ShowHelp.
            DependencyObject parent = args.OriginalSource as DependencyObject;
            if (parent is UIElement)
            {
                var tv = ((UIElement)parent).ParentOfType<RadTileView>();
                if (tv != null)
                {
                    if (tv.MaximizedItem != null)
                    {
                        parent = (DependencyObject)tv.MaximizedItem;
                    }
                }
            }
            string keyword = FindKeyword(parent);
            if (!string.IsNullOrEmpty(keyword))
            {
                form.Help.ShowHelp(null, FindFilename(parent), keyword);
            }
            else
            {
                form.Help.ShowHelp(null, FindFilename(parent));
            }
        }

        private static string FindFilename(DependencyObject sender)
        {
            if (sender != null)
            {
                string fileName = GetFilename(sender);
                if (!string.IsNullOrEmpty(fileName))
                    return fileName;
                return FindFilename(VisualTreeHelper.GetParent(sender));
            }
            return null;
        }

        private static string FindKeyword(DependencyObject sender)
        {
            if (sender != null)
            {
                string keyword = GetKeyword(sender);
                if (!string.IsNullOrEmpty(keyword))
                    return keyword;
                return FindKeyword(VisualTreeHelper.GetParent(sender));
            }
            return null;
        }
        #endregion

    }
}
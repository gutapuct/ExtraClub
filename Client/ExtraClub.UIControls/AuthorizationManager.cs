using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Telerik.Windows.Controls;
using ExtraClub.UIControls.Interfaces;

namespace ExtraClub.UIControls
{
    public static class AuthorizationManager
    {
        static IClientContext _context;
        static Window _anchor = new Window();

        public static void Init(IClientContext context)
        {
            _context = context;
        }

        public static void InitAnchor(Window anchor)
        {
            if (_anchor != null && Windows.ContainsKey(_anchor))
            {
                var list = Windows[_anchor];
                Windows.Remove(_anchor);
                Windows.Add(anchor, list);
                anchor.Closed += wnd_Closed;
                anchor.Activated += wnd_Activated;
            }
            _anchor = anchor;
        }

        public static void ApplyPermissions(Window wnd)
        {
            RunAuthorize(wnd ?? _anchor);
        }

        public static readonly DependencyProperty AuthorizationKeyProperty = DependencyProperty.RegisterAttached(
          "AuthorizationKey",
          typeof(String),
          typeof(DependencyObject),
          new FrameworkPropertyMetadata(String.Empty, FrameworkPropertyMetadataOptions.AffectsRender)
        );

        private static Dictionary<UIElement, string> AuthKeys = new Dictionary<UIElement, string>();
        private static Dictionary<Window, List<UIElement>> Windows = new Dictionary<Window, List<UIElement>>();

        public static void SetAuthorizationKey(UIElement element, String value)
        {
            element.SetValue(AuthorizationKeyProperty, value);
            AuthKeys.Add(element, value);
            var wnd = element.ParentOfType<Window>();
            if (wnd == null)
            {
                wnd = _anchor;
            }
            {
                if (!Windows.ContainsKey(wnd))
                {
                    Windows.Add(wnd, new List<UIElement>());
                    wnd.Closed += wnd_Closed;
                    wnd.Activated += wnd_Activated;
                }
                Windows[wnd].Add(element);
            }
        }

        static void wnd_Activated(object sender, EventArgs e)
        {
            var wnd = sender as Window;
            wnd.Activated -= wnd_Activated;
            RunAuthorize(wnd);
        }

        static void wnd_Closed(object sender, EventArgs e)
        {
            var wnd = sender as Window;
            if (Windows.ContainsKey(wnd))
            {
                foreach (var element in Windows[wnd])
                {
                    AuthKeys.Remove(element);
                }
                Windows.Remove(wnd);
            }
            wnd.Closed -= wnd_Closed;
        }

        public static String GetAuthorizationKey(UIElement element)
        {
            return (String)element.GetValue(AuthorizationKeyProperty);
        }

        static void RunAuthorize(Window wnd)
        {
            //return;
            foreach (var element in Windows[wnd])
            {
                if (!_context.CheckPermission(AuthKeys[element]))
                {
                    SmartHideElement(element);
                }
            }
        }

        private static void SmartHideElement(UIElement uIElement)
        {
            uIElement.Visibility = Visibility.Collapsed;

            if (uIElement is RadPanelBarItem)
            {
                var pb = uIElement as RadPanelBarItem;
                var bar = pb.Parent as RadPanelBar;
                if (bar != null && pb.IsExpanded)
                {
                    pb.IsExpanded = false;
                    foreach (RadPanelBarItem i in bar.Items)
                    {
                        if (i.Visibility == Visibility.Visible)
                        {
                            i.IsExpanded = true;
                            break;
                        }
                    }
                }
                if (bar != null)
                {
                    bar.Items.Remove(pb);
                }
            }
            if (uIElement is RadTileViewItem)
            {
                var pb = uIElement as RadTileViewItem;
                var bar = pb.Parent as RadTileView;
                if (bar != null)
                {
                    bar.Items.Remove(pb);
                }
            }
            if (uIElement is RadTabItem)
            {
                var tab = uIElement as RadTabItem;
                var tc = tab.Parent as RadTabControl;
                if (tc != null)
                {
                    tc.Items.Remove(tab);
                }
            }
        }

        public static bool SetElementVisible(UIElement element)
        {
            if (element == null) return false;
            if (AuthKeys.ContainsKey(element))
            {
                if (!_context.CheckPermission(AuthKeys[element]))
                {
                    element.Visibility = Visibility.Collapsed;
                    return false;
                }
            }
            element.Visibility = Visibility.Visible;
            return true;
        }

        public static void PostPermissions(ClientContext context)
        {
            context.PostPermissions(AuthKeys.Values.Distinct().ToArray());
        }
    }
}

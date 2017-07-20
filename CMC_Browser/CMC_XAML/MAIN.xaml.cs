using System.Windows;

using System.Windows.Controls;
using System.Windows.Navigation;
using System.Reflection;

namespace CMC_Browser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CMC_Html CMC_HTML;

        public MainWindow(CMC_Html cmcHtml)
        {
            InitializeComponent();
            this.CMC_HTML = cmcHtml;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CMC_FRAME.Navigated += new NavigatedEventHandler(WebBrowser_Navigated);
        }

        void WebBrowser_Navigated(object sender, NavigationEventArgs e)
        {
            HideJsScriptErrors((WebBrowser)sender);
        }

        public void HideJsScriptErrors(WebBrowser wb)
        {
            // IWebBrowser2 interface
            // Exposes methods that are implemented by the WebBrowser control 
            // Searches for the specified field, using the specified binding constraints.
            FieldInfo fld = typeof(WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fld == null)
                return;
            object obj = fld.GetValue(wb);
            if (obj == null)
                return;
            // Silent: Sets or gets a value that indicates whether the object can display dialog boxes.
            // HRESULT IWebBrowser2::get_Silent(VARIANT_BOOL *pbSilent);HRESULT IWebBrowser2::put_Silent(VARIANT_BOOL bSilent);
            obj.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, obj, new object[] { true });
        }
    }
}

using System;
using System.Windows;
using System.Threading.Tasks;

namespace CMC_Browser.CMC_XAML
{
    /// <summary>
    /// Interaction logic for LOAD.xaml
    /// </summary>
    public partial class LOAD : Window
    {
        public Task LOAD_TASK;
        public CMC_Html CMC_HTML = new CMC_Html();
        public MainWindow MAIN;

        public LOAD()
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;

            LoadSequence();
        }
    private async void LoadSequence()
        {

            await Task.Run(() => CMC_HTML.BuildCMCData(this));
            //pass reference to the data source
            MAIN = new MainWindow(CMC_HTML);

            //send html data to the frame
            MAIN.frameCMC.Source = CMC_HTML.GetFileLocation();
            MAIN.Show();

            this.Hide();

            //handle form close
            MAIN.Closing += delegate
            {
                this.Close();
            };
        }
    }
}

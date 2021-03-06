using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Win32;
using PT_MAPACKET.Models;

namespace PT_MAPACKET
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<TabItem> _tabItems;
        private TabItem _tabAdd;
        private int graphCounter;

        static int a = 3;

        private void AddStartPage()
        {
            _tabAdd = new TabItem();
            _tabAdd.Header = "Strona startowa";
            StartPage sp = new StartPage();
            _tabAdd.Content = sp;
            int newItemIndex = 0;

            if (_tabItems.Count == 0) _tabItems.Add(_tabAdd);
            else
            {
                _tabItems.Insert(tabControl.SelectedIndex + 1, _tabAdd);
                newItemIndex = tabControl.SelectedIndex + 1;
            }

            tabControl.DataContext = null;
            tabControl.DataContext = _tabItems;

            tabControl.Items.Refresh();
            tabControl.SelectedIndex = newItemIndex;
        }

        private string ExtractFileName(string fullName)
        {
            string[] split = fullName.Split('\\');
            return split[split.Length - 1];
        }

        private bool IsAPcapFile(string text)
        {
            if (text.Contains(".pcap")) return true;
            return false;
        }

        public MainWindow()
        {
            InitializeComponent();

            _tabItems = new List<TabItem>();
            AddStartPage();
            Analize.IsEnabled = false;
            graphCounter = 0;
        }

        private void File_Open_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "PCAP Files (*.pcap)|*.pcap";
                openFileDialog.Title = "Please select an file to open.";
                int newItemIndex = 0;
                if (openFileDialog.ShowDialog() == true)
                {
                    //TODO wczytywanie pliku lub plików
                    foreach (String s in openFileDialog.FileNames)
                    {
                        _tabAdd = new TabItem();
                        _tabAdd.Header = ExtractFileName(s);
                        PcapFileControl pfc = new PcapFileControl(s);
                        _tabAdd.Content = pfc;
                        if (_tabItems.Count == 0) _tabItems.Add(_tabAdd);
                        else _tabItems.Insert(tabControl.SelectedIndex + 1, _tabAdd);
                        newItemIndex = tabControl.SelectedIndex + 1;
                    }

                    tabControl.DataContext = null;
                    tabControl.DataContext = _tabItems;

                    tabControl.Items.Refresh();
                    tabControl.SelectedIndex = newItemIndex;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Wystąpił błąd podczas otwierania pliku");
            }
        }


        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tabControl.SelectedIndex >= 0)
            {
                int selectionIndex = tabControl.SelectedIndex;
                if (selectionIndex < 0) selectionIndex = 0;
                TabItem ti = _tabItems[selectionIndex];
                if (IsAPcapFile(ti.Header.ToString())) Analize.IsEnabled = true;
                else Analize.IsEnabled = false;
            }
        }

        private void Analize_Graph_Click(object sender, RoutedEventArgs e)
        {
            var tabContent = tabControl.SelectedContent as UserControl;

            GraphTabControl GTC = new GraphTabControl(((IGetPacket)tabControl.SelectedContent).getPacketsData());
            _tabAdd = new TabItem();
            _tabAdd.Header = "Graph[" + graphCounter + "]";
            graphCounter++;
            _tabAdd.Content = GTC;
            if (_tabItems.Count == 0) _tabItems.Add(_tabAdd);
            else _tabItems.Insert(tabControl.SelectedIndex + 1, _tabAdd);
            int newItemIndex = tabControl.SelectedIndex + 1;

            tabControl.DataContext = null;
            tabControl.DataContext = _tabItems;

            tabControl.Items.Refresh();
            tabControl.SelectedIndex = newItemIndex;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //TODO zamykanie wszystkich okien
        }

        private void View_StartPage_Click(object sender, RoutedEventArgs e)
        {
            AddStartPage();
        }

        private void File_Close_Card_Click(object sender, RoutedEventArgs e)
        {
            int nextItem = tabControl.SelectedIndex - 1;
            if (nextItem < 0) nextItem = 0;
            _tabItems.Remove(_tabItems[tabControl.SelectedIndex]);

            tabControl.DataContext = null;
            tabControl.DataContext = _tabItems;

            tabControl.Items.Refresh();
            tabControl.SelectedIndex = nextItem;
        }

        private void File_Close_Cards_Click(object sender, RoutedEventArgs e)
        {
            int nextItem = tabControl.SelectedIndex - 1;
            if (nextItem < 0) nextItem = 0;
            _tabItems.Clear();

            tabControl.DataContext = null;
            tabControl.DataContext = _tabItems;

            tabControl.Items.Refresh();
            tabControl.SelectedIndex = nextItem;
        }

        private void File_Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void File_Connect_Click(object sender, RoutedEventArgs e)
        {
            _tabAdd = new TabItem();
            _tabAdd.Header = "Merged file.pcap";
            PcapFileControl mf = new PcapFileControl();
            _tabAdd.Content = mf;
            int newItemIndex = 0;


            _tabItems.Insert(0, _tabAdd);

            tabControl.DataContext = null;
            tabControl.DataContext = _tabItems;

            tabControl.Items.Refresh();
            tabControl.SelectedIndex = newItemIndex;

            ConnectFilesWindow cfw = new ConnectFilesWindow(_tabItems);
            cfw.Show();
        }
        private void Credits(object sender, RoutedEventArgs e)
        {
            _tabAdd = new TabItem();
            _tabAdd.Header = "Autorzy";
            Credits cr = new Credits();
            _tabAdd.Content = cr;
            int newItemIndex = 0;


            _tabItems.Insert(0, _tabAdd);

            tabControl.DataContext = null;
            tabControl.DataContext = _tabItems;

            tabControl.Items.Refresh();
            tabControl.SelectedIndex = newItemIndex;
        }

        private void Documentation(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/SzymonK95/PT_PCAP_Project/blob/master/dokumentacja/MAPacket-Dokumentacja.pdf");
        }

        private void Save_file(object sender, RoutedEventArgs e)
        {
            TabItem currentPage = tabControl.SelectedItem as TabItem;

            if (IsAPcapFile(currentPage.Header.ToString()))
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(currentPage.Header + ".txt"))
            {
                PcapFileControl pcf = (PcapFileControl)currentPage.Content;
                List<MyPacket> lines = pcf.getPacketsData();
                foreach (MyPacket line in lines)
                {
                    file.WriteLine(line);
                }

                MessageBox.Show("Plik został zapisany", "Zapis udany");
            }
            else
                MessageBox.Show("Ten plik nie moze zostać zapisany", "Błąd zapisu");
        }

        private void Save_file_as(object sender, RoutedEventArgs e)
        {
            TabItem currentPage = tabControl.SelectedItem as TabItem;

            if (IsAPcapFile(currentPage.Header.ToString()))
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "Txt file|*.txt";
                saveFileDialog1.Title = "Save Txt File";
                saveFileDialog1.ShowDialog();

                PcapFileControl pcf = (PcapFileControl)currentPage.Content;
                List<MyPacket> lines = pcf.getPacketsData();

                if (saveFileDialog1.FileName != "")
                {
                    // Saves the Image via a FileStream created by the OpenFile method.
                    System.IO.FileStream fs =
                       (System.IO.FileStream)saveFileDialog1.OpenFile();
                    foreach (MyPacket line in lines)
                        WriteLine(fs, line.ToString() + Environment.NewLine);

                    fs.Close();

                    MessageBox.Show("Plik został zapisany", "Zapis udany");
                }
            }
            else
            {
                MessageBox.Show("Ten plik nie moze zostać zapisany", "Błąd zapisu");
            }
        }

        private static void WriteLine(FileStream fs, string value)
        {
            byte[] info = new UTF8Encoding(true).GetBytes(value);
            fs.Write(info, 0, info.Length);
        }
    }
}

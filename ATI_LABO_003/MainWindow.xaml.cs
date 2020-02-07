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
using System.Net.NetworkInformation;
using System.Net;
using System.Threading;
using System.Windows.Threading;

namespace ATI_LABO_003
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            GetActiveNics();
        }

        private void GetActiveNics()
        {
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus.ToString().ToUpper() == "UP")
                {
                    if (nic.GetPhysicalAddress().ToString() == "") continue;
                    ComboBoxItem itm = new ComboBoxItem();
                    itm.Content = nic.Name;
                    itm.Tag = nic;
                    cmbNics.Items.Add(itm);
                }

            }
        }

        private void CmbNics_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            tbkInfo.Text = "";
            if (cmbNics.SelectedItem == null) return;

            List<string[]> tepingen;

            ComboBoxItem itm = (ComboBoxItem)cmbNics.SelectedItem;
            NetworkInterface nic = (NetworkInterface)itm.Tag;
            foreach (UnicastIPAddressInformation ip in nic.GetIPProperties().UnicastAddresses)
            {
                if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    tepingen = new List<string[]>();

                    tbkInfo.Text += $"IP4 \t {ip.Address.ToString()} \n";
                    tbkInfo.Text += $"Subnet \t {ip.IPv4Mask.ToString()} \n";


                    string[] pinginfo = new string[2];
                    pinginfo[0] = "Start pinging to loopback";
                    pinginfo[1] = "127.0.0.1";
                    tepingen.Add(pinginfo);

                    pinginfo = new string[2];
                    pinginfo[0] = "Start pinging to NIC IP";
                    pinginfo[1] = ip.Address.ToString();
                    tepingen.Add(pinginfo);



                    int gwCounter = 1;
                    foreach (GatewayIPAddressInformation gwadres in nic.GetIPProperties().GatewayAddresses)
                    {

                        if (gwadres.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            pinginfo = new string[2];
                            pinginfo[0] = "Start pinging to gateway";
                            pinginfo[1] = gwadres.Address.ToString();
                            tepingen.Add(pinginfo);
                            tbkInfo.Text += $"GW {gwCounter} \t {gwadres.Address.ToString()} \n";
                            gwCounter++;
                        }
                    }
                    int dnsCounter = 1;
                    foreach (IPAddress dnsadres in nic.GetIPProperties().DnsAddresses)
                    {
                        if (dnsadres.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            pinginfo = new string[2];
                            pinginfo[0] = $"Start pinging to DNS {dnsCounter}";
                            pinginfo[1] = dnsadres.ToString();
                            tepingen.Add(pinginfo);
                            tbkInfo.Text += $"DNS {dnsCounter} \t {dnsadres.ToString()} \n";
                            dnsCounter++;
                        }
                    }
                    int dhcpCounter = 1;
                    foreach (IPAddress dhcpadres in nic.GetIPProperties().DhcpServerAddresses)
                    {
                        if (dhcpadres.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            pinginfo = new string[2];
                            pinginfo[0] = $"Start pinging to gateway DHCP {dhcpCounter}";
                            pinginfo[1] = dhcpadres.ToString();
                            tepingen.Add(pinginfo);
                            tbkInfo.Text += $"DHCP {dhcpCounter} \t {dhcpadres.ToString()} \n";
                            dhcpCounter++;
                        }
                    }

                    Ping pingSender;
                    PingOptions options;
                    foreach (string[] pingip in tepingen)
                    {
                        pingSender = new Ping();
                        options = new PingOptions();
                        options.DontFragment = true;

                        // Create a buffer of 32 bytes of data to be transmitted.
                        string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
                        byte[] buffer = Encoding.ASCII.GetBytes(data);
                        int timeout = 120;
                        tbkInfo.Text += "========================================================" + Environment.NewLine;
                        tbkInfo.Text += $"\t {pingip[0]} \n";
                        Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
                        PingReply reply = pingSender.Send(pingip[1], timeout, buffer, options);
                        if (reply.Status == IPStatus.Success)
                        {
                            tbkInfo.Text += $"\t Address: {reply.Address.ToString()} \n";
                            tbkInfo.Text += $"\t RoundTrip time: {reply.RoundtripTime} \n";
                            tbkInfo.Text += $"\t Time to live: {reply.Options.Ttl} \n";
                        }
                        else
                        {
                            tbkInfo.Text += $"Error : no response from this IP number \n";

                        }
                    }

                }
            }

        }


    }
}

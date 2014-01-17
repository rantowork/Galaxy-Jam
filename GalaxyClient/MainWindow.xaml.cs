using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Awesomium.Core;
using GalaxyClient.Core;
using GalaxyJam;

namespace GalaxyClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private Game1 m_galaxyJam;
        readonly HttpClient m_client = new HttpClient();
        readonly HighScoreCollection m_highScoreCollection = new HighScoreCollection();
        private WebSession m_session;

        public MainWindow()
        {
            InitializeComponent();

            m_client.BaseAddress = new Uri("https://localhost:44300/");
            m_client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

            //HighScoreList.ItemsSource = m_highScoreCollection;
            GetHighScores();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            m_session = WebCore.CreateWebSession(WebPreferences.Default);
        }

        private void LaunchGalaxyJamButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
            m_galaxyJam = new Game1();
            m_galaxyJam.Run();
        }

        private async void GetHighScores()
        {
            try
            {
                LaunchGalaxyJamButton.IsEnabled = false;

                var response = await m_client.GetAsync("api/highscore");
                response.EnsureSuccessStatusCode();

                var highScores = await response.Content.ReadAsAsync<IEnumerable<HighScore>>();
                m_highScoreCollection.CopyFrom(highScores);
            }
            catch (Newtonsoft.Json.JsonException jsonException)
            {
                MessageBox.Show(jsonException.Message);
            }
            catch (HttpRequestException exception)
            {
                MessageBox.Show(exception.Message);
            }
            finally
            {
                LaunchGalaxyJamButton.IsEnabled = true;
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("https://localhost:44300/Account/Register");
            }
            catch (System.ComponentModel.Win32Exception browserException)
            {
                if (browserException.ErrorCode == 2147467259)
                {
                    MessageBox.Show(browserException.Message);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Your User Id:" + AuthControl.ExecuteJavascriptWithResult("document.getElementById('id').value") + "       Your Nickname:" + (AuthControl.ExecuteJavascriptWithResult("document.getElementById('nickname').value")));
        }
    }
}

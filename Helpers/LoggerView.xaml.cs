namespace DBListenTest.Helpers
{
    using System;
    using System.Windows;
    using System.Collections.Generic;
    using System.Windows.Input;
    using System.Linq;
    /// <summary>
    /// Окно журнала приложения
    /// </summary>
    public partial class LoggerView : Window
    {
        /// <summary>
        /// Создает экземпляр класса <see cref="LoggerView"/>
        /// </summary>
        //public static string CurrentLogState = "test";
        //public List<string> log_string_list { get; set; } = new List<string>();
        public LoggerView(string log_title, string CurrentLogState)
        {

            InitializeComponent();
            TextBlock1.Text = CurrentLogState;
            TextBox1.Text = CurrentLogState;
            this.PreviewKeyDown += new KeyEventHandler(HandleEsc);
            this.Title = log_title;
        }

        private void HandleEsc(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }
        //public LoggerView()
        //{
        //    InitializeComponent();
        //    //log_string_list = new List<string>();
        //    TextBlock1.Text = String.Join(Environment.NewLine, log_string_list.ToArray());
        //    TextBox1.Text = String.Join(Environment.NewLine, log_string_list.ToArray());
        //}
    }
}

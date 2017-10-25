﻿using System.Windows;
using UserInterface.ViewModels;

namespace UserInterface.Views.UserControls
{
    /// <summary>
    /// Interaction logic for CurrentPlaybackView.xaml
    /// </summary>
    public partial class CurrentPlaybackView
    {
        public CurrentPlaybackView()
        {
            InitializeComponent();
        }

        private void CurrentPlaybackView_OnLoaded(object sender, RoutedEventArgs e)
        {
            var vm = (CurrentPlaybackViewModel) DataContext;
            vm?.LoadValues(Dispatcher);
        }
    }
}
using System;
using System.IO;
using LiveCharts;
using ReactiveUI;
using SpotifyPlaylistMixer.ViewModels;

namespace SpotifyPlaylistMixer.Views.UserControls
{
    /// <summary>
    /// Interaction logic for GenreView.xaml
    /// </summary>
    public partial class GenreView : IViewFor<GenreViewModel>
    {
        public GenreView()
        {
            ViewModel = new GenreViewModel();
            InitializeComponent();

            DataContext = ViewModel;

            var dir = Directory.GetCurrentDirectory();
            ViewModel.Path = $@"{dir}\Resources\Examples\";
            // TODO: Remove this
            PointLabel = chartPoint =>
                $"{chartPoint.Y} ({chartPoint.Participation:P})";
            DataContext = this;
        }
        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (GenreViewModel)value; }
        }

        public GenreViewModel ViewModel { get; set; }
        public Func<ChartPoint, string> PointLabel { get; set; }
    }
}

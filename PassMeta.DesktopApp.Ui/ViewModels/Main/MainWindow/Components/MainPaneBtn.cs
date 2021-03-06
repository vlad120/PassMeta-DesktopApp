namespace PassMeta.DesktopApp.Ui.ViewModels.Main.MainWindow.Components
{
    using System;
    using System.Reactive.Linq;
    using ReactiveUI;

    public class MainPaneBtn : ReactiveObject
    {
        private bool _isActive;
        public bool IsActive
        {
            get => _isActive;
            set => this.RaiseAndSetIfChanged(ref _isActive, value);
        }

        public IObservable<string> Content { get; }

        public MainPaneBtn(string text, string icon, IObservable<bool> shortModeObservable)
        {
            Content = shortModeObservable.Select(isShort => isShort ? icon : text);
        }
    }
}
namespace PassMeta.DesktopApp.Ui.Views
{
    using DesktopApp.Ui.Views.Base;
    
    using Avalonia.Markup.Xaml;
    using ViewModels.Journal;

    public class JournalView : PageView<JournalViewModel>
    {
        public JournalView()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
using Microsoft.Extensions.DependencyInjection;
using Fluver.UI.Controls;
using Fluver.UI.ViewModels;

namespace Fluver.UI.Views
{
    public sealed partial class AboutPage : FluverView
    {
        public AboutPageViewModel ViewModel { get; } = App.Services.GetRequiredService<AboutPageViewModel>();

        protected override PageViewModel PageViewModel => ViewModel;

        public AboutPage()
        {
            InitializeComponent();

            if (!ViewModel.IsEvaluationCopy)
            {
                LegalText.Blocks.Remove(EvaluationCopyParagraph);
            }
        }
    }
}

using Microsoft.Extensions.DependencyInjection;
using Fluver.ViewModels;
using Fluver.UI.Controls;

namespace Fluver.Views
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

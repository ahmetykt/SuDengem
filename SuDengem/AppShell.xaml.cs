
namespace SuDengem
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("GecmisPage", typeof(GecmisPage));
        }
    }
}

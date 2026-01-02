namespace Fluver.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Storage : InfoPage
    {
        public Storage()
        {
            this.InitializeComponent();
            ExpanderStates = SettingValues.DiskExpanderStates;

            GetAllDisks(true);
        }

        private void GetAllDisks(bool hookEvent = false)
        {
            if (hookEvent)
                App.MainWindow.DeviceChanged += (s, e) => GetAllDisks();

            var expanders = AssignerHelper.TryAssign(DriveInfo.GetDrives, () => []).Select(drive =>
            {
                Expander expander = null;
                if (drive.IsReady && (expander = Children.FirstOrDefault(i => (i as StorageExpander).DriveInfo.RootDirectory.FullName == drive.RootDirectory.FullName, null)) is null)
                    Children.Add(expander = AssignerHelper.TryAssign(() => new StorageExpander(drive)));
                return expander;
            });

            foreach (var expander in Children.Except(expanders).ToArray())
                Children.Remove(expander);

            //Refactored:
            //var currentDrives = AssignerHelper.TryAssign(DriveInfo.GetDrives, () => []);
            //var currentDriveNames = currentDrives.Select(d => d.GetBestDisplayName());
            //var currentChildrenNames = Children.Select(c => c.Header.ToString());

            //var drivesToAdd = currentDrives.Where(d => d.IsReady && !currentChildrenNames.Contains(d.GetBestDisplayName()));
            //foreach (var d in drivesToAdd)
            //    Children.Add(new StorageExpander(d));

            //var childrenToRemove = Children.Where(c => !currentDriveNames.Contains(c.Header.ToString()));
            //foreach (var child in childrenToRemove.ToArray())
            //    Children.Remove(child);
        }
    }
}

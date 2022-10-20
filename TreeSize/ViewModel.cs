using Model;
using NLog;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using WorckWithFileSystem;

namespace TreeSize
{
    public class ViewModel : Observable
    {
        const string FolderImage = "images/folder.png";
        const string FileImage = "images/file.png";
        const string DiskImage = "images/disk.png";

        private List<string> _rootDirectories;
        private ObservableCollection<DirectoryItem> _subfolders;
        private DataProvider _dataProvider;
        private Dispatcher _dispatcher;
        private string _selectedRootDirectory;
        private int _loadingProgress = 0;
        private ulong _allocatedVolumeOnDisk;
        private ulong _loadedBytes = 0;
        private CancellationToken _token;
        CancellationTokenSource _cancellationTotenSource;

        public int fileCount { get; set; }

        public ViewModel()
        {
            _dataProvider = new DataProvider();
            RootDirectories = _dataProvider.GetRootDirectories();
            _dispatcher = Dispatcher.CurrentDispatcher;
            Application.Current.MainWindow.Closing += new CancelEventHandler(MainWindowClosing);
            _cancellationTotenSource = new CancellationTokenSource();
            _token = _cancellationTotenSource.Token;
        }

        public int LoadingProgress
        {
            get { return _loadingProgress; }
            set
            {
                _loadingProgress = value;
                OnPropertyChanged("LoadingProgress");
            }
        }

        public List<string> RootDirectories
        {
            get { return _rootDirectories; }
            set
            {
                _rootDirectories = value;
                OnPropertyChanged("RootDirectories");
            }
        }

        public ObservableCollection<DirectoryItem> Subfolders
        {
            get { return _subfolders; }
            set
            {
                _subfolders = value;
                OnPropertyChanged("Subfolders");
            }
        }

        public string SelectedRootDirectory
        {
            get { return _selectedRootDirectory; }
            set
            {
                _selectedRootDirectory = value;
                OnPropertyChanged("SelectedRootDirectory");
                var dataProvider = new DataProvider();
                _allocatedVolumeOnDisk = dataProvider.GetDiskAllocatedVolume(_selectedRootDirectory);
                Subfolders = dataProvider.GetFolders(_selectedRootDirectory);
                Subfolders[0].ImageSource = DiskImage;

                BuildTreeAsync();
            }
        }

        public void ConnectLevelResursive(DirectoryInfo directoryInfo, DirectoryItem folder, Logger log)
        {
            if (folder.Elements.Count > 0)
            {
                _dispatcher.Invoke(
                () =>
                {
                    folder.Elements.Clear();
                });
            }
            СonnectFolders(directoryInfo, folder, log);
            СonnectFiles(directoryInfo, folder, log);
            SizeCalculator.ConvertSizeAvto(folder);
        }

        private async void BuildTreeAsync()
        {
            Logger log = LogManager.GetCurrentClassLogger();

            await Task.Run(() =>
            {
                var path = Subfolders[0].Name;
                var dirInfo = new DirectoryInfo(path);
                ConnectLevelResursive(dirInfo, Subfolders[0], log);
            });
            var tree = new DirectoryItem(Subfolders[0].Path)
            {
                Name = Subfolders[0].Name,
                Elements = Subfolders[0].Elements,
                FilesAmount = Subfolders[0].FilesAmount,
                FoldersAmount = Subfolders[0].FoldersAmount,
                SizeString = Subfolders[0].SizeString,
                ImageSource = Subfolders[0].ImageSource
            };

            Subfolders[0] = tree;
            LoadingProgress = 100;
        }

        private void СonnectFiles(DirectoryInfo directoryInfo, DirectoryItem folder, Logger log)
        {
            var files = _dataProvider.GetFilesOrCatchException(directoryInfo, log);
            foreach (FileInfo file in files)
            {
                var child = new DirectoryItem(directoryInfo.FullName + "/" + file.Name) { Name = file.Name, ImageSource = FileImage };
                fileCount++;

                child.FilesAmount = 1;
                child.Size = (ulong)file.Length;

                SizeCalculator.ConvertSizeAvto(child);

                if (_token.IsCancellationRequested) return;
                _dispatcher.Invoke(
                 () =>
                 {
                     _dataProvider.AddFile(folder, child);
                     _loadedBytes += child.Size;
                     LoadingProgress = _dataProvider.GetLoadingProgress(_allocatedVolumeOnDisk, _loadedBytes);
                 });
            }
        }

        private void СonnectFolders(DirectoryInfo directoryInfo, DirectoryItem folder, Logger log)
        {
            var subDirectories = _dataProvider.GetDirectoriesOrCatchException(directoryInfo, log);
            foreach (DirectoryInfo subdirectory in subDirectories)
            {
                var child = new DirectoryItem(directoryInfo.FullName + "/" + subdirectory.Name) { Name = subdirectory.Name, ImageSource = FolderImage };
                var dirInfo = new DirectoryInfo(directoryInfo.FullName + "/" + subdirectory.Name);
                ConnectLevelResursive(dirInfo, child, log);
                if (_token.IsCancellationRequested) return;
                _dispatcher.Invoke(
                   () =>
                   {
                       _dataProvider.AddFolder(folder, child);
                   });
            }
        }

        private void MainWindowClosing(object sender, CancelEventArgs e)
        {
            _cancellationTotenSource.Cancel();
        }
    }
}

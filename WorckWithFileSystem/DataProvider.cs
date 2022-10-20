using Model;
using NLog;
using System.Collections.ObjectModel;

namespace WorckWithFileSystem
{
    public class DataProvider
    {
        private DirectoryItem _rootDirectoryItem;

        public ObservableCollection<DirectoryItem> GetFolders(string path)
        {
            ConnectFirstLevel(path);
            return _rootDirectoryItem.Elements;
        }

        public List<string> GetRootDirectories()
        {
            var rootDirectories = new List<string>();
            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (DriveInfo driveInfo in drives)
            {
                rootDirectories.Add(driveInfo.Name);
            }
            return rootDirectories;
        }

        public void AddFolder(DirectoryItem folder, DirectoryItem child)
        {
            folder.Size += child.Size;
            folder.FoldersAmount += 1 + child.FoldersAmount;
            folder.FilesAmount += child.FilesAmount;
            folder.Elements.Add(child);
        }

        public void AddFile(DirectoryItem folder, DirectoryItem child)
        {
            folder.Size += child.Size;
            folder.FilesAmount++;
            folder.Elements.Add(child);
        }

        public DirectoryInfo[] GetDirectoriesOrCatchException(DirectoryInfo directoryInfo, Logger log)
        {
            DirectoryInfo[] directories = new DirectoryInfo[0];
            try
            {
                directories = directoryInfo.GetDirectories();
            }
            catch (System.UnauthorizedAccessException ex)
            {
                log.Error(ex, ex.Message);
            }
            return directories;
        }

        public FileInfo[] GetFilesOrCatchException(DirectoryInfo directoryInfo, Logger log)
        {
            FileInfo[] files = new FileInfo[0];
            try
            {
                files = directoryInfo.GetFiles();
            }
            catch (System.UnauthorizedAccessException ex)
            {

                log.Error(ex, ex.Message);
            }
            return files;
        }

        public ulong GetDiskAllocatedVolume(string name)
        {
            DriveInfo[] drives = DriveInfo.GetDrives();
            var selectedDrive = drives.Where(u => u.Name == name).First();
            ulong allocatedVolume = (ulong)(selectedDrive.TotalSize - selectedDrive.AvailableFreeSpace);
            return allocatedVolume;
        }

        public int GetLoadingProgress(ulong allocatedVolume, ulong loadedVolume)
        {
            int loadedProgress = 0;
            if (allocatedVolume > 0)
            {
                ulong lp = ((100 * loadedVolume) / allocatedVolume);
                loadedProgress = Convert.ToInt32(lp);
            }
            return loadedProgress;
        }

        private void ConnectFirstLevel(string path)
        {
            _rootDirectoryItem = new DirectoryItem("") { Name = "" };
            var childList = new ObservableCollection<DirectoryItem>();
            var initialDriver = new DriveInfo(path);
            var child = new DirectoryItem(initialDriver.Name) { Name = initialDriver.Name };
            ConnectChildren(initialDriver.RootDirectory, child);
            childList.Add(child);
            _rootDirectoryItem.Elements = childList;
        }

        private void ConnectChildren(DirectoryInfo directoryInfo, DirectoryItem folder)
        {
            if (directoryInfo.Exists)
            {
                var subDirectories = directoryInfo.GetDirectories();
                foreach (DirectoryInfo subdirectory in subDirectories)
                {
                    folder.Elements.Add(new DirectoryItem(directoryInfo.FullName + subdirectory.Name) { Name = subdirectory.Name });
                }
            }
        }
    }
}
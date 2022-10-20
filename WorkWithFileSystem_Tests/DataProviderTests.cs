using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;
using Model;
using NLog;
using WorckWithFileSystem;
using Logger = NLog.Logger;

namespace WorkWithFileSystem_Tests
{
    [TestClass]
    public class DataProviderTests
    {
        [TestMethod()]
        public void GetRootDirectoriesTest()
        {
            //arrange
            var dataProvider = new DataProvider();

            DriveInfo[] drives = DriveInfo.GetDrives();
            int expectedCount = drives.Length;

            //act
            var folders = dataProvider.GetRootDirectories();
            int actualCount = folders.Count;

            //assert
            Assert.AreEqual(expectedCount, actualCount);
        }

        [TestMethod()]
        public void AddFolderTest()
        {
            //arrange
            var dataProvider = new DataProvider();

            var parent = new DirectoryItem("");
            var child = new DirectoryItem("");

            //act
            dataProvider.AddFolder(parent, child);
            int expectedCount = 1;
            int actualCount = parent.Elements.Count;

            int expectedFoldersCount = 1;
            int actualFoldersCount = parent.FoldersAmount;

            //assert
            Assert.AreEqual(expectedCount, actualCount);
            Assert.AreEqual(expectedFoldersCount, actualFoldersCount);
        }

        [TestMethod()]
        public void AddFileTest()
        {
            //arrange
            var dataProvider = new DataProvider();

            var parent = new DirectoryItem("");
            var child = new DirectoryItem("");

            //act
            dataProvider.AddFile(parent, child);
            int expectedCount = 1;
            int actualCount = parent.Elements.Count;

            //assert
            Assert.AreEqual(expectedCount, actualCount);
        }

        [TestMethod()]
        public void GetDirectoriesOrCatchExceptionTest()
        {
            //arrange
            var dataProvider = new DataProvider();

            string path = Directory.GetCurrentDirectory();
            var dirInfo = new DirectoryInfo(path);
            var subdirs = dirInfo.GetDirectories();
            Logger log = LogManager.GetCurrentClassLogger();

            //act
            var directories = dataProvider.GetDirectoriesOrCatchException(dirInfo, log);
            int expectedCount = subdirs.Length;
            int actualCount = directories.Length;

            //assert
            Assert.AreEqual(expectedCount, actualCount);
        }

        [TestMethod()]
        public void GetFilesOrCatchExceptionTest()
        {
            //arrange
            var dataProvider = new DataProvider();

            string path = Directory.GetCurrentDirectory();
            var dirInfo = new DirectoryInfo(path);
            var files = dirInfo.GetFiles();
            Logger log = LogManager.GetCurrentClassLogger();

            //act
            var actualFiles = dataProvider.GetFilesOrCatchException(dirInfo, log);
            int expectedCount = files.Length;
            int actualCount = actualFiles.Length;

            //assert
            Assert.AreEqual(expectedCount, actualCount);
        }

        [TestMethod()]
        public void GetLoadingProgressTest()
        {
            //arrange
            var dataProvider = new DataProvider();

            ulong allocatedVolume = 1000000;
            ulong loadedVolume = 250000;
            int expectedProgress = 25;

            //act
            var actualProgress = dataProvider.GetLoadingProgress(allocatedVolume, loadedVolume);

            //assert
            Assert.AreEqual(expectedProgress, actualProgress);
        }

        [TestMethod()]
        public void GetDiskAllocatedVolumeTest()
        {
            //arrange
            var dataProvider = new DataProvider();

            DriveInfo[] drives = DriveInfo.GetDrives();
            string name = drives[0].Name;
            ulong expected = (ulong)(drives[0].TotalSize - drives[0].AvailableFreeSpace);

            //act
            ulong actual = dataProvider.GetDiskAllocatedVolume(name);

            //assert
            Assert.AreEqual(expected, actual);
        }
    }
}
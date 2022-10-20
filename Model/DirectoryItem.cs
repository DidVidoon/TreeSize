using System.Collections.ObjectModel;

namespace Model
{
    public class DirectoryItem
    {
        public Units Unit { get; set; }
        public string Name { get; set; }
        public string ImageSource { get; set; }
        public ObservableCollection<DirectoryItem> Elements { get; set; }
        public ulong Size { get; set; }
        public int FoldersAmount { get; set; } = 0;
        public int FilesAmount { get; set; } = 0;
        public string Path { get; set; }
        public string SizeString { get; set; } = "Size";

        public DirectoryItem(string path)
        {
            Path = path;
            Elements = new ObservableCollection<DirectoryItem>();
        }
    }
}

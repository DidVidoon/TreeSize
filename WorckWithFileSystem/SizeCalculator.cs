using Model;

namespace WorckWithFileSystem
{
    public static class SizeCalculator
    {
        const int SizeOfKB = 1024;
        const int SizeOfMB = 1048576;
        const ulong SizeOfGB = 1074741824;
        const ulong SizeOfTB = 1099511627776;
        const int BoundaryKoeffForUNnits = 10;

        public static void ConvertSizeAvto(DirectoryItem item)
        {
            if (item.Size < BoundaryKoeffForUNnits * SizeOfKB)
            {
                item.Unit = Units.Bytes;
                item.SizeString = Convert.ToString(item.Size) + " " + Convert.ToString(item.Unit);
            }
            else if (item.Size < BoundaryKoeffForUNnits * SizeOfMB)
            {
                item.Unit = Units.kB;

                item.SizeString = Convert.ToString(item.Size / SizeOfKB) + " " + Convert.ToString(item.Unit);
            }
            else if (item.Size < BoundaryKoeffForUNnits * SizeOfGB)
            {
                item.Unit = Units.MB;
                item.SizeString = Convert.ToString(item.Size / SizeOfMB) + " " + Convert.ToString(item.Unit);
            }
            else if (item.Size < BoundaryKoeffForUNnits * SizeOfTB)
            {
                item.Unit = Units.GB;
                item.SizeString = Convert.ToString(item.Size / SizeOfGB) + " " + Convert.ToString(item.Unit);
            }
            else
            {
                item.Unit = Units.TB;
                item.SizeString = Convert.ToString(item.Size / SizeOfTB) + " " + Convert.ToString(item.Unit);
            }
        }
    }
}

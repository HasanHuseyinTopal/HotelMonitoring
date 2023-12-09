

namespace EntityLayer.DTOs
{
    public class MonthlyVisitorDTO
    {
        public int monthNumber { get; set; }
        public int RestOfRoomsCount { get; set; }
        public double MonthlyPercent { get; set; }
        public double MaxMonthlyPercent { get; set; }
        public int TotalRoomCount { get; set; }
        public (int CellState, string Name, int Identity,int count,bool? DontChange,string? TransferDatas, string? crashedDate)[,] tableValues { get; set; } 
        public int[] roomNumbers { get; set; }
        public bool ShowAll { get; set; }

    }
}

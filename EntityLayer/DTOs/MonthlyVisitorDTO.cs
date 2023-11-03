

namespace EntityLayer.DTOs
{
    public class MonthlyVisitorDTO
    {
        public int monthNumber { get; set; }
        public int RestOfRoomsCount { get; set; }
        public double MonthlyPercent { get; set; }
        public double MaxMonthlyPercent { get; set; }
        public int TotalRoomCount { get; set; }
        public (int CellState, string Name, int Identity,int count,bool? DontChange,string? TransferDatas)[,] tableValues { get; set; } 
        public int[] roomNumbers { get; set; } = { 101, 102, 103, 104, 201, 202, 203, 204, 301, 302, 303, 304, 401, 402, 403, 404, 501, 502, 503, 601, 602, 603, 701, 702 };

        public bool ShowAll { get; set; }

    }
}

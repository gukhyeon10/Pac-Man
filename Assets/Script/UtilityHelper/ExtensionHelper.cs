using GGame;

namespace GUtility
{
    public static class ExtensionHelper
    {
        public static (int row, int col) Calculate(this (int row, int col) coord, EDirect direct)
        {
            return direct switch
            { 
                EDirect.EAST  => (coord.row, coord.col + 1),
                EDirect.WEST  => (coord.row, coord.col - 1),
                EDirect.SOUTH => (coord.row + 1, coord.col),
                EDirect.NORTH => (coord.row - 1, coord.col),
                
                _ => (coord.row, coord.col)
            };
        }
        
        public static void Update(ref this (int row, int col) coord, EDirect direct)
        {
            coord = direct switch
            { 
                EDirect.EAST  => (coord.row, coord.col + 1),
                EDirect.WEST  => (coord.row, coord.col - 1),
                EDirect.SOUTH => (coord.row + 1, coord.col),
                EDirect.NORTH => (coord.row - 1, coord.col),
                
                _ => (coord.row, coord.col)
            };
        }
        
        public static bool Equals(this (int row, int col) a, (int row, int col) b)
        {
            return a.row == b.row && a.col == b.col;
        }
    }    
}


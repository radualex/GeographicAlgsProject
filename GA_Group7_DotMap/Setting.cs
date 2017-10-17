namespace GA_Group7_DotMap
{
    public class Setting
    {
        // Raw data, Propotion * Propotion positions, each position can possibly contains 1 person.
        public static int Propotion = 1000;
        
        // Population distribution. 80% from western eu, 10 percentage from northern eu ...
        public static int WestPecrentage = 80;
        public static int NorthPercentage = 10;
        public static int Eastecerntage = 5;
        public static int SouthPercentage = 3;
        public static int NonEuPercentage = 2;

        public static int BorderWidth = 4;
        public static int CityBlockWidth = 2;

        public static int BaseRaduis = 4;
        public static int MaximumRaduis = 10;
        public static int BaseNumberOfGroupsPerLine = 50;
    }
}

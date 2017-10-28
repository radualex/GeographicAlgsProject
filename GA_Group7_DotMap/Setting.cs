namespace GA_Group7_DotMap
{
    public class Setting
    {
        // Raw data, Propotion * Propotion positions, each position can possibly contains 1 person.
        public static int Propotion = 1000;
        
        // Population distribution. 60% from western eu, 15% from northern eu ...
        public static int WestPecrentage = 60;
        public static int NorthPercentage = 15;
        public static int EastPercentage = 10;
        public static int SouthPercentage = 10;
        public static int NonEuPercentage = 5;

        public static int BorderWidth = 4;
        public static int CityBlockWidth = 2;

        public static int MinimumAggregationDotRadius = 4; // When resolving the overlap, we ignore the minimum radius requirement.

        public static int BaseNumberOfGroupsPerLine = 1; // will be set to some number when the program starts.

        // for instance, each dots represents 1000 people,
        // If a there is a area which contains more than 200 people,
        // then we determine the main group (which region) and show it,
        // Otherwise, we do not display it.
        public static double MinimumPercentageToShow = 0.2;
    }
}

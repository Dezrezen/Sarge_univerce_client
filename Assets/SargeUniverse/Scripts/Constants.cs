using SargeUniverse.Scripts.BattleSystem.PathFinding;

namespace SargeUniverse.Scripts
{
    public static class Constants
    {
        // Scenes
        public const string StartScene = "Start";
        public const string GameScene = "Game";
        public const string BattleScene = "BattleMap";
        public static string WarScene = "WarScreen";
        
        // Map
        public const int GridSize = 44;
        public const int GridOffset = 3;
        public const float GridCellSize = 1;

        // Battle
        public const int BattleDuration = 120;
        public const int BattlePrepDuration = 30;
        public const float BattleFrameRate = 0.05f;

        public const int BattleTilesWorthOfOneWall = 15;
        public const int BattleGroupWallAttackRadius = 5;
        public const int BattleGridOffset = 3;
        
        // Direction
        public const int DirectionsCount = 8;
        private static readonly MovementDirection NDirection = new(0, 1);
        private static readonly MovementDirection NWDirection = new(-1, -1);
        private static readonly MovementDirection WDirection = new(-1, 0);
        private static readonly MovementDirection SWDirection = new(-1, 1);
        private static readonly MovementDirection SDirection = new(0, -1);
        private static readonly MovementDirection SEDirection = new(1, 1);
        private static readonly MovementDirection EDirection = new(1, 0);
        private static readonly MovementDirection NEDirection = new(1, -1);

        public static MovementDirection GetDirection(int index)
        {
            return index switch
            {
                0 => NDirection,
                1 => NWDirection,
                2 => WDirection,
                3 => SWDirection,
                4 => SDirection,
                5 => SEDirection,
                6 => EDirection,
                7 => NEDirection,
                _ => new MovementDirection(0, 0)
            };
        }
        
        public const int MinCollectValue = 10;
        
        public static int GetBuilderStationBuildCost(int buildingsCount)
        {
            return buildingsCount switch
            {
                2 => 250,
                3 => 500,
                4 => 1000,
                5 => 2000,
                _ => 0
            };
        }
    }
}
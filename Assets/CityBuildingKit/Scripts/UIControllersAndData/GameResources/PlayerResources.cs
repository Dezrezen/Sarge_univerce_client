/**
 * This source file is part of CityBuildingKit.
 * Copyright (c) 2018.
 * All rights reserved.
 */

namespace Assets.Scripts.UIControllersAndData.GameResources
{
    public class PlayerResources
    {
        public PlayerResources()
        {
            Builder = new ResourceData();
            Housing = new ResourceData();
            Gold = new ResourceData();
            Mana = new ResourceData();
            Crystal = new ResourceData();
            Power = new ResourceData();
            Supplies = new ResourceData();
        }

        public ResourceData Builder { get; set; }

        public ResourceData Gold { get; set; }

        public ResourceData Mana { get; set; }

        public ResourceData Crystal { get; set; }

        public ResourceData Housing { get; set; }

        public ResourceData Power { get; set; }

        public ResourceData Supplies { get; set; }
    }
}
using UnityEngine;

namespace SargeUniverse.Scripts.BattleSystem.Data
{
    public class BattleTrophies
    {
        public static int GetStorageSuppliesAndPowerLoot(int hqLevel, float storage)
        {
            var p = hqLevel switch
            {
                1 or 2 or 3 or 4 or 5 or 6 => 0.2d,
                7 => 0.18d,
                8 => 0.16d,
                9 => 0.14d,
                10 => 0.12d,
                _ => 0.1d
            };
            return (int)Mathf.Floor((float)(storage * p));
        }

        public static int GetStorageEnergyLoot(int hqLevel, float storage)
        {
            var p = hqLevel switch
            {
                1 or 2 or 3 or 4 or 5 or 6 or 7 or 8 => 0.06d,
                9 => 0.05d,
                _ => 0.04d
            };
            return (int)Mathf.Floor((float)(storage * p));
        }

        public static (int, int) GetBattleTrophies(int attackerTrophies, int defendderTrophies)
        {
            var win = 0;
            var lose = 0;
            if(attackerTrophies == defendderTrophies)
            {
                win = 30;
                lose = 20;
            }
            else
            {
                double delta = Mathf.Abs(attackerTrophies - defendderTrophies);
                if(attackerTrophies > defendderTrophies)
                {
                    win = 30 - (int)Mathf.Floor((float)(delta * (28d / 600d)));
                    lose = 20 + (int)Mathf.Floor((float)(delta * (19d / 600d)));
                    if(win < 2)
                    {
                        win = 2;
                    }
                }
                else
                {
                    win = 30 + (int)Mathf.Floor((float)(delta * (28f / 600f)));
                    lose = 20 - (int)Mathf.Floor((float)(delta * (19f / 600f)));
                    if(lose < 1)
                    {
                        lose = 1;
                    }
                }
            }
            return (win, lose);
        }
    }
}
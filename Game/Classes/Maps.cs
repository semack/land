using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Land.Enums;

namespace Land.Classes
{
    public static class Maps
    {
        public const int CapacityX = 50;
        public const int CapacityY = 16;

        public static int MapsCount
        {
            get { return Directory.EnumerateFiles("Data").Count(file => file.Contains(".map")); }
        }

        public static bool IsBiomass(SpriteTypeEnum source)
        {
            return source >= SpriteTypeEnum.Biomass1 && source <= SpriteTypeEnum.Biomass4;
        }

        public static bool IsWall(SpriteTypeEnum source)
        {
            return IsBrickWall(source) || IsStoneWall(source);
        }

        public static bool IsBrickWall(SpriteTypeEnum source)
        {
            return source == SpriteTypeEnum.BrickWall;
        }

        public static bool IsStoneWall(SpriteTypeEnum source)
        {
            return source == SpriteTypeEnum.StoneWall;
        }


        public static bool IsChest(SpriteTypeEnum source)
        {
            return source == SpriteTypeEnum.Chest;
        }

        public static bool IsExitDoor(SpriteTypeEnum source)
        {
            return source >= SpriteTypeEnum.ExitDoorLeft && source <= SpriteTypeEnum.ExitDoorRight;
        }

        public static bool IsStairs(SpriteTypeEnum source)
        {
            return source >= SpriteTypeEnum.StairsLeft && source <= SpriteTypeEnum.StairsRight;
        }

        public static bool IsFloor(SpriteTypeEnum source)
        {
            return source == SpriteTypeEnum.Floor;
        }

        public static bool IsSpace(SpriteTypeEnum source)
        {
            return source == SpriteTypeEnum.Space || source == SpriteTypeEnum.Chest;
        }

        public static bool IsLiveWall(SpriteTypeEnum source)
        {
            return (source >= SpriteTypeEnum.WallLive1 && source <= SpriteTypeEnum.WallLive3);
        }


        public static List<string> LoadMap(int number)
        {
            var result = new List<string>();
            try
            {
                string fileName = string.Format("Data/{0}.map", number);
                using (var reader = new StreamReader(fileName))
                {
                    string line = string.Empty;
                    while (!line.StartsWith("01234567890"))
                    {
                        line = reader.ReadLine();
                    }
                    for (int i = 0; i < 14; i++)
                    {
                        line = reader.ReadLine();
                        line = line.Substring(1, line.Length - 2);
                        if (line.Length < 48)
                            throw new Exception();
                        result.Add(line);
                    }
                }
            }
            catch (Exception)
            {
                throw new FileLoadException("Map loading error.\r\n Wrong map format or map doesn't not exists.");
            }
            return result;
        }

        public static SpriteTypeEnum[,] Get(int stage)
        {
            List<string> map = LoadMap(stage);

            var result = new SpriteTypeEnum[CapacityX, CapacityY];
            for (int j = 0; j < CapacityY; j++)
            {
                for (int i = 0; i < CapacityX; i++)
                {
                    // render borders
                    if ((i == 0 || (i == CapacityX - 1) || (j == 0 || (j == CapacityY - 1))))
                        result[i, j] = SpriteTypeEnum.StoneWall;
                    else
                    {
                        // render map
                        char value = map[j - 1][i - 1];
                        if (value == 'W')
                            result[i, j] = SpriteTypeEnum.StoneWall;
                        else if (value == 'B')
                            result[i, j] = SpriteTypeEnum.BrickWall;
                        else if (value == 'C')
                            result[i, j] = SpriteTypeEnum.Chest;
                        else if (value == '`')
                            result[i, j] = SpriteTypeEnum.Floor;
                        else if (value == ']')
                            result[i, j] = SpriteTypeEnum.StairsLeft;
                        else if (value == '[')
                            result[i, j] = SpriteTypeEnum.StairsRight;
                        else if (value == '1')
                            result[i, j] = SpriteTypeEnum.Biomass1;
                        else if (value == '2')
                            result[i, j] = SpriteTypeEnum.Biomass2;
                        else if (value == '3')
                            result[i, j] = SpriteTypeEnum.Biomass3;
                        else if (value == '4')
                            result[i, j] = SpriteTypeEnum.Biomass4;
                    }
                }
            }


            for (int i = 0; i < 2; i++) // devil entrance render
            {
                result[15 + i, 0] = SpriteTypeEnum.Space;
                result[30 + i, 0] = SpriteTypeEnum.Space;
            }

            //exit door render
            result[1, 0] = SpriteTypeEnum.ExitDoorLeft;
            result[2, 0] = SpriteTypeEnum.ExitDoorRight;

            return result;
        }
    }
}
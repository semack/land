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

        private const string MapsRoot = "Content/Maps";
        private const string MapExtension = ".map";

        public static readonly List<string> Banks = new List<string>(Directory.EnumerateDirectories(MapsRoot).OrderBy(s=>s).Select(s => s.Replace(MapsRoot, String.Empty).Replace("/", String.Empty).Replace("\\", String.Empty)));

        private static string GetMapBankPath(int bank)
        {
            return string.Format("{0}/{1}/", MapsRoot, Banks[bank]);
        }

        public static int GetMapsCount(int bank)
        {
            string dir = GetMapBankPath(bank);
            return Directory.GetFiles(dir).Count(file => file.Contains(MapExtension));
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


        public static List<string> LoadMap(int bank, int number)
        {
            var result = new List<string>();
            string fileName = string.Format("{0}{1:D3}{2}", GetMapBankPath(bank), number, MapExtension);
            using (var reader = new StreamReader(fileName))
            {

                try
                {

                    string line = string.Empty;
                    while (line != null && !line.Contains("{"))
                    {
                        line = reader.ReadLine();
                    }
                    for (int i = 0; i < 16; i++)
                    {
                        line = reader.ReadLine();
                        if (line != null && line.Length < 50)
                            throw new Exception();
                        result.Add(line);
                    }
                    reader.Close();
                }
#if XBOX360
            catch (Exception e)
            {
                throw e;
                
#else
                catch (Exception)
                {
                    throw new FileLoadException("Map loading error.\r\n Wrong map format or map doesn't not exist.");
#endif
                }
            }
            return result;
        }

        public static SpriteTypeEnum[,] Get(int mapsBank, int stage)
        {
            List<string> map = LoadMap(mapsBank, stage);

            var result = new SpriteTypeEnum[CapacityX, CapacityY];
            for (int j = 0; j < CapacityY; j++)
            {
                for (int i = 0; i < CapacityX; i++)
                {
                    // render map
                    char value = map[j][i];
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
                    else if (value == '<')
                        result[i, j] = SpriteTypeEnum.ExitDoorLeft;
                    else if (value == '>')
                        result[i, j] = SpriteTypeEnum.ExitDoorRight;
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

            for (int i = 0; i < 2; i++) // devil entrance render
            {
                result[15 + i, 0] = SpriteTypeEnum.Space;
                result[30 + i, 0] = SpriteTypeEnum.Space;
            }

            return result;
        }
    }
}

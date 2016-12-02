// <summary>
//   The class containing the BuildData used by the interpreter to buy items in order
// </summary>
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using AramBuddy.MainCore.Utility.MiscUtil;
using EloBuddy;
using System.Collections.Generic;

namespace AramBuddy.AutoShop
{
    /// <summary>
    ///     The class containing the BuildData used by the interpreter to buy items in order
    /// </summary>
    public class Build
    {
        /// <summary>
        ///     An array of the item names
        /// </summary>
        public string[] BuildData { get; set; }

        /// <summary>
        /// returns The build name.
        /// </summary>
        /// 

        public static string BuildName()
        {
            var ChampionName = Player.Instance.CleanChampionName();

            if (ADC.Any(s => s.Equals(ChampionName, StringComparison.CurrentCultureIgnoreCase)))
            {
                return "ADC";
            }

            if (AD.Any(s => s.Equals(ChampionName, StringComparison.CurrentCultureIgnoreCase)))
            {
                return "AD";
            }

            if (AP.Any(s => s.Equals(ChampionName, StringComparison.CurrentCultureIgnoreCase)))
            {
                return "AP";
            }

            if (ManaAP.Any(s => s.Equals(ChampionName, StringComparison.CurrentCultureIgnoreCase)))
            {
                return "ManaAP";
            }

            if (Tank.Any(s => s.Equals(ChampionName, StringComparison.CurrentCultureIgnoreCase)))
            {
                return "Tank";
            }

            Logger.Send("Failed to detect champion: " + ChampionName, Logger.LogLevel.Warn);
            //Logger.Send("Using Default Build !");
            return "Default";
        }

        /// <summary>
        ///     Creates Builds
        /// </summary>
        public static void CreateDefaultBuild()
        {
            try
            {
                var filename = $"{BuildName()}.json";
                var WebClient = new WebClient();
                WebClient.DownloadStringTaskAsync($"https://raw.githubusercontent.com/plsfixrito/AramBuddy/master/DefaultBuilds/{filename}");
                WebClient.DownloadStringCompleted += delegate (object sender, DownloadStringCompletedEventArgs args)
                {
                    try
                    {
                        if (args.Cancelled || args.Error != null)
                        {
                            Logger.Send(args.Error?.Message, Logger.LogLevel.Error);
                            Logger.Send("External server bad response.", Logger.LogLevel.Error);
                            Logger.Send("/!\\ NO BUILD IS CURRENTLY BEING USED /!\\", Logger.LogLevel.Error);
                            return;
                        }
                        if (args.Result.Contains("data"))
                        {
                            File.WriteAllText(Setup.BuildPath + "\\" + filename, args.Result);
                            Setup.Builds.Add(BuildName(), File.ReadAllText(Setup.BuildPath + "\\" + filename));
                            Logger.Send(BuildName() + " Build Created for " + Player.Instance.ChampionName + " - " + BuildName());
                            Setup.UseDefaultBuild();
                        }
                        else
                        {
                            Logger.Send("External server bad response.", Logger.LogLevel.Error);
                            Console.WriteLine(args.Result);
                        }
                    }
                    catch (TargetInvocationException ex)
                    {
                        Logger.Send("Failed to create default build for " + Player.Instance.ChampionName, ex, Logger.LogLevel.Error);
                        Logger.Send("/!\\ NO BUILD IS CURRENTLY BEING USED /!\\", Logger.LogLevel.Error);
                        Logger.Send(ex?.InnerException?.Message, Logger.LogLevel.Error);
                    }
                };
            }
            catch (Exception ex)
            {
                // if faild to create build terminate the AutoShop
                Logger.Send("Failed to create default build for " + Player.Instance.ChampionName, ex, Logger.LogLevel.Error);
                Logger.Send("/!\\ NO BUILD IS CURRENTLY BEING USED /!\\", Logger.LogLevel.Error);
            }
        }

        /// <summary>
        ///     Creates Builds
        /// </summary>
        public static void GetBuildFromService()
        {
            try
            {
                try
                {
                    var filename = $"{Player.Instance.CleanChampionName()}.json";
                    var WebClient = new WebClient();
                    WebClient.DownloadStringTaskAsync($"https://raw.githubusercontent.com/plsfixrito/AramBuddy/master/DefaultBuilds/{Config.CurrentPatchUsed}/{filename}");
                    WebClient.DownloadStringCompleted += delegate (object sender, DownloadStringCompletedEventArgs args)
                    {
                        if (args.Cancelled || args.Error != null)
                        {
                            Logger.Send("External server bad response.", Logger.LogLevel.Error);
                            Setup.UseDefaultBuild();
                            return;
                        }
                        if (args.Result.Contains("data"))
                        {
                            var filepath = $"{Setup.BuildPath}/{Config.CurrentPatchUsed}/{filename}";
                            File.WriteAllText(filepath, args.Result);
                            Setup.Builds.Add(Player.Instance.CleanChampionName(), File.ReadAllText(filepath));
                            Logger.Send("Created Build for " + Player.Instance.ChampionName);
                            Setup.CustomBuildService();
                        }
                        else
                        {
                            Logger.Send("External server bad response.", Logger.LogLevel.Error);
                            Setup.UseDefaultBuild();
                        }
                    };
                }
                catch (TargetInvocationException ex)
                {
                    Logger.Send(ex?.InnerException?.Message, Logger.LogLevel.Error);
                    Setup.UseDefaultBuild();
                }
            }
            catch (Exception ex)
            {
                // if faild to create build terminate the AutoShop
                Logger.Send("Failed to create Build from service " + Config.CurrentBuildService + " " + Config.CurrentPatchUsed + " for " + Player.Instance.ChampionName, Logger.LogLevel.Error);
                Logger.Send(ex.InnerException?.Message, Logger.LogLevel.Error);
                Logger.Send("Getting default build", Logger.LogLevel.Warn);
                Setup.UseDefaultBuild();
            }
        }

        /// <summary>
        ///  ADC Champions.
        /// </summary>
        public static readonly string[] ADC =
        {
            "Ashe", "Caitlyn", "Corki", "Draven", "Ezreal", "Graves", "Jhin", "Jinx", "Kalista", "Kindred", "KogMaw", "Lucian", "MissFortune", "Sivir", "Quinn",
            "Tristana", "Twitch", "Urgot", "Varus", "Vayne"
        };

        /// <summary>
        ///  Mana AP Champions.
        /// </summary>
        public static readonly string[] ManaAP =
        {
            "Ahri", "Anivia", "Annie", "AurelionSol", "Azir", "Brand", "Cassiopeia", "Diana", "Elise", "Ekko", "Evelynn", "Fiddlesticks", "Fizz", "Galio",
            "Gragas", "Heimerdinger", "Janna", "Karma", "Karthus", "Kassadin", "Kayle", "Leblanc", "Lissandra", "Lulu", "Lux", "Malzahar", "Morgana", "Nami", "Nidalee", "Ryze", "Orianna", "Sona",
            "Soraka", "Swain", "Syndra", "Taliyah", "Teemo", "TwistedFate", "Veigar", "Viktor", "VelKoz", "Xerath", "Ziggs", "Zilean", "Zyra"
        };

        /// <summary>
        ///  AP no Mana Champions.
        /// </summary>
        public static readonly string[] AP = { "Akali", "Katarina", "Kennen", "Mordekaiser", "Rumble", "Vladimir" };

        /// <summary>
        ///  AD Champions.
        /// </summary>
        public static readonly string[] AD =
        {
            "Aatrox", "Fiora", "Gangplank", "Jax", "Jayce", "KhaZix", "LeeSin", "MasterYi", "Nocturne", "Olaf", "Pantheon", "Rengar", "Riven", "Talon", "Tryndamere",
            "Wukong", "XinZhao", "Yasuo", "Zed"
        };

        /// <summary>
        ///  Tank Champions.
        /// </summary>
        public static readonly string[] Tank =
        {
            "Alistar", "Amumu", "Blitzcrank", "Bard", "Braum", "ChoGath", "Darius", "DrMundo", "Garen", "Gnar", "Hecarim", "Kled", "Illaoi", "Irelia", "Ivern", "JarvanIV",
            "Leona", "Malphite", "Maokai", "Nasus", "Nautilus", "Nunu", "Poppy", "Rammus", "RekSai", "Renekton", "Sejuani", "Shaco", "Shen", "Shyvana", "Singed", "Sion", "Skarner", "TahmKench",
            "Taric", "Thresh", "Trundle", "Udyr", "Vi", "Volibear", "Warwick", "Yorick", "Zac"
        };
    }
}

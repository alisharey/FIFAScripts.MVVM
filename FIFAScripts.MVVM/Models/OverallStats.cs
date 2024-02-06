using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIFAScripts.MVVM.Models;

public class OverallStats
{
    public static Dictionary<string, List<string>> GroupedStats =
           new Dictionary<string, List<string>>()
           {
                { "Pace",
                    new List<string>{
                        "acceleration",
                        "sprintspeed"
                    }
                },

                { "Shooting",
                    new List<string>
                    {
                        "positioning",
                        "finishing",
                        "shotpower",
                        "longshots",
                        "volleys",
                        "penalties"
                    }
                },

                { "Passing",
                    new List<string>
                    {
                        "vision",
                        "crossing",
                        "freekickaccuracy",
                        "shortpassing",
                        "longpassing",
                        "curve"
                    }
                },

                { "Dribbling",
                    new List<string>
                    {
                        "agility",
                        "balance",
                        "reactions",
                        "ballcontrol",
                        "dribbling",
                        "composure"
                    }
                },

                { "Defending",
                    new List<string>
                    {
                        "interceptions",
                        "defensiveawareness",
                        "standingtackle",
                        "slidingtackle",
                        "headingaccuracy",
                    }
                },

                { "Physicality",
                    new List<string>
                    {
                         "jumping",
                         "stamina",
                         "strength",
                         "aggression",

                    }
                },

                { "GK",
                    new List<string>
                    {

                         "gkdiving",
                         "gkhandling",
                         "gkkicking",
                         "gkpositioning",
                         "gkreflexes",
                    }
                }


           };
    
    public int Pace { get; set; }
    public int Shooting { get; set; }
    public int Passing { get; set; }
    public int Dribbling { get; set; }
    public int Defending { get; set; }
    public int Physicality { get; set; }
    public int GK { get; set; }

    public OverallStats(Dictionary<string, string> playersStatToValue)
    {
        foreach(var statGroup in GroupedStats)
        {
            int sum = 0;            
            foreach(var stat in statGroup.Value)
            {
                sum += int.Parse(playersStatToValue[stat]);
            }

            decimal temp = Math.Round((decimal)(sum / statGroup.Value.Count), 0);
            int avg = Convert.ToInt32(temp);

            _ = statGroup.Key switch
            {
                "Pace" => Pace = avg,
                "Shooting" => Shooting = avg,
                "Passing" => Passing = avg,
                "Dribbling" => Dribbling = avg,
                "Defending" => Defending = avg,
                "Physicality" => Physicality = avg,
                "GK" => GK = avg,
                _ => throw new NotImplementedException(),
            };



        }
    }


}

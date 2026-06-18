using BepInEx;
using UnityEngine;
using System;
using RWCustom;
using Random = UnityEngine.Random;

namespace RW_Bases;

public static class HomeOfHooks
{
  public static void RegisterHooks()
  {
    // void hook
    On.Player.ctor += Player_ctor;

    // other value hook, will probably be a bool or float to begin with. dnSpy for the win
    // nvm it's a SoundID, but gets the job done
    On.LizardVoice.GetMyVoiceTrigger += LizardVoice_GetMyVoiceTrigger;
  }

  private static void Player_ctor(On.Player.orig_ctor orig, Player self, AbstractCreature abstractCreature, World world)
  {
    // REMEMBER TO CALL ORIG EVERYWHERE !!! unless it's a special thing but CALL ORIG please and thank you
    orig(self, abstractCreature, world);

    // checks if the player is Hunter, or Red in code.
    // a full list of names is:
    // Monk = SlugcatStats.Name.Yellow | Survivor = SlugcatStats.Name.White | Hunter = SlugcatStats.Name.Red
    // Gourmand = MoreSlugcatsEnums.SlugcatStatsName.Gourmand | Artificer = MoreSlugcatsEnums.SlugcatStatsName.Artificer
    // Rivulet = MoreSlugcatsEnums.SlugcatStatsName.Rivulet | Spearmaster = MoreSlugcatsEnums.SlugcatStatsName.Spear | Saint = MoreSlugcatsEnums.SlugcatStatsName.Saint
    // Watcher = WatcherEnums.SlugcatStatsName.Watcher
    if (self.SlugCatClass == SlugcatStats.Name.Red)
    {
      // sets hunter to be a pup graphics wise. does not give them the one hand or weakness treatment.
      self.setPupStatus(true);
    }
  }

  private static SoundID LizardVoice_GetMyVoiceTrigger(On.LizardVoice.orig_GetMyVoiceTrigger, LizardVoice self)
  {
    // make a variable as the result
    var res = orig(self);
    // other variables depend on what you're doing/hooking, but i'm changing a blue lizard voice in this example
    List<SoundID> list;
    SoundID soundID;

    // checks if the LizardVoice.lizard variable is the actual Lizard, which is shortened to the variable "l"
    if (self.lizard is Lizard l)
    {
      // checks if l is a Blue Lizard
      if (l is CreatureTemplate.Type.BlueLizard)
      {
        // makes a new array and adds a new SoundID inside
        var array = new[]
        {
          SoundID.Lizard_Voice_Green_A
        };
        // makes the list we declared earlier be the same as the array
        list = [];
        // a loop to add SoundIDs within the array
        for (var i = 0; i < array.Length; i++)
        {
          // the soundID we declared earlier now is equal to the array we defined, at the "i" position
          soundID = array[i];
          // checks if the soundID index isn't -1 and the sound triggers are working
          if (soundID.Index != -1 && l.abstractPhysicalObject.world.game.soundLoader.workingTriggers[soundID.Index])
          {
            // adds the soundID to the list
            list.Add(soundID);
          }
        }
        // checks if the list is empty
        if (list.Count == 0)
        {
          // and makes it so no sound plays
          res = SoundID.None;
        }
        // if something else is caught instead, run the following equals statement
        else
        {
          // our result variable, res, now equals our list variable with UnityEngine.Random.Range(0, list.Count) as the array number
          // list.Count is just how many SoundIDs are in the list; Random.Range picks something random from the specified numbers, which in this case it goes from 0 to how many are in our list
          res = list[Random.Range(0, list.Count)];
        }
      }
    }
    // make sure to return something on non-void methods or you'll get errors
    return res;
  }
}

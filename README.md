# Track-O-Matic
![trackerimage (3)](https://github.com/Brian0255/Track-O-Matic/assets/61294586/fb4b9e90-a44a-4dc3-aa8c-9f6487578fd9)
### This is a tracker in the style of Kingdom Hearts II Randomizer trackers, but for Donkey Kong 64 Randomizer. It has support for alternate hint systems, such as Points, Vials, and HitList that other trackers are not tailored to. 

## Installation
1. You'll need some sort of emulator. I heavily recommend using [Project64](https://www.pj64-emu.com/) with this tracker since it will run DK64 Randomizer the best. At the moment, autotracking is only supported for Project 64 as well.
2. Download the latest release zip from [here.](https://github.com/Brian0255/Track-O-Matic/releases/latest) Extract that to wherever you'd like.
3. You'll need a randomizer seed from the [Dev site](dev.dk64randomizer.com/randomizer.html) since this tracker relies exclusively on hint systems that are not on the main website yet.
4. When you generate a seed, hit that "Save Log" button next to the giant warnings everywhere. You can't miss it!
5. Run the executable file and the tracker should begin running correctly.
6. You'll probably want **autotracking** on, which you can find at the top menu. All you have to do is turn it on and that's it!
7. Open your emulator of choice and load up your seed.
8. Drag the spoiler log that you downloaded onto the tracker, or just click File -> Open Spoiler Log and select it. **The tracker will not do anything until you give it one.**
9. That's it! You're all set for that seed. If you want to do another one, make sure to click File -> Reset Tracker and then repeat steps 7 and 8.

## User Controls
- Left clicking an item in the bottom area will allow you to drag it to a world. If the item is in a world, left clicking will put it back to the bottom.
- **If the autotracker has placed an item, you cannot left click it.** This is to avoid any sort of confusion.
- Middle clicking an item or scrolling your wheel on it will **star** it. A starred item will subtract 1 from a world's required check total (if you have that on), so this is very useful when you want to note things that you know are logically needed.
- Right clicking and dragging an item in the bottom will allow you to place a semi visible item. This is useful for if you *think* an item is somewehere but you're not completely sure.
- Left/Right clicking a world will cycle a world's number forward and backward to help you notate what the level orders are. You can also use your mouse wheel to achieve the same effect.
- Left/Right clicking collectibles (Pearls, Battle Crowns, Blueprints, etc.) will increase/decrease them. **If autotracking is on, these are noted automatically and will not respond to your inputs as a result.**
- Clicking in the bottom area will allow you to type in notes of your choosing.
- With autotracking on, you really don't have to do much (which is the intention). I heavily recommend it!

## Supported Hint Systems

### Points
- Points is based on KH2's point system.
- Each world has a point value based on the sum of its important checks.
- Items have varying point values. For instance, you could make Kongs worth 13 points, keys worth 11 points, stuff like that.
- Based on what is left and remaining point values, you can math it out and figure out where things are. Ideal for people who like these sorts of number game puzzles.
- For this hint system I recommend using the dev site option to include Way of the Hoard counts, as I find it makes the endgames of this much less of a slog.

### Vials
- This is a hint system where you are immediately shown what vials are in each level.
- For instance, Jungle Japes could have 1 Clear Vial, 1 Yellow Vial, and 2 Red Vials.
- Based on what vials are left, you can quickly pin down where certain moves are, making this very unique.
- For this hint system I would also recommend the Way of the Hoard counts, as it again makes the endgame much less of a blind casino and more of a deduction puzzle.

## Special Thanks
- The KH2 community for creating hint systems/trackers that this one is based off of.
- JXJacob for their [GSTHD](https://github.com/jxjacob/GSTHD) autotracking code that I referenced/used a lot in creating mine.
- Mukomo for their [PapeTracker](https://github.com/mukomo/PapeTracker) which this code was based off of.
- [dk64randomizer.com](dk64randomizer.com) and the [Discord](discord.gg/dk64randomizer) especially. Come join it!

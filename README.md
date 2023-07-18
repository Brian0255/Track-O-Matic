# PapeTracker
A Paper Mario 64 item tracker for use with the online randomizer [PM64Randomizer](https://pm64randomizer.com)

This was forked from RedBuddha's [KH2Tracker](https://github.com/Red-Buddha/KH2Tracker)

Icons used in this tracker come from the Mario Wiki page on [Paper Mario](https://www.mariowiki.com/Paper_Mario)


## Options
* Save Progress
 * Saves the current tracker state to a file. If hints are loaded they will be saved as well
* Load Progress
 * Loads a saved tracker file.
  
***
  
* Reset Window
  * Resets the main and broadcast windows to their default size
* Reset Tracker
  * Resets the tracker to its default state 
  
## Toggles
  * Currently not implemented. Wait for a future release
  
***

* Drag and Drop
  * Toggles between drag and dropping items + selecting a world and double clicking an item or just selecting a world and single clicking an item to track items
  
## How To Use

You MUST first drag a spoiler log file into the tracker for proper function.

Drag an item to the location that you found it in. Alternatively highlight worlds by clicking on them and then double click on items to mark them as collected in that world. Clicking on a marked item will return it to the item pool. (if not using drag and drop controls then only a single click on an item is required)

The question marks indicate a spoiler log has not been loaded. When numbers appear the log was read. Regional point counts will load based on which items are found in them.

* A partner or hammer/boots upgrade is worth 9 points
* Chapter required items plus the Ultra Stone are worth 7 points
* Other key items that aren't required for chapter clearing but can give checks are worth 5 points
  * Every instance of these items will give the points (items such as the colored berries or cake mix appear multiple times, though you don't need all of them for progress)
* Below items (badges and item pouches) are worth 3 points
  * Item Pouch (x5)
  * FP Plus (x3)
  * HP Plus (x3)
  * Power Plus (x2)
  * Spike Shield
  * Feeling Fine
  * Dodge Master
  * Mega Quake
  * Mega Smash
  * Mega Jump
  * Bump Attack

A new feature to this tracker is that regions with 0 points left have their numbers turn blue. This leads to quick visual updates for viewers.

## Thanks

* Tommadness
  * Created the broadcast window and the framework from which the auto tracker was built. Spent a ton of time helping figure out bugs and solutions to get the auto tracker working. (Currently neither of those work but we'll get there)
* RedBuddha
  * Created the version of the KH2Tracker that I use and was the base of this tracker
* Supreme
  * Part of the dynamic duo that came up with the idea and got me inspired to really work on this
  * Chose a lot of icons to represent regions of the world
* Spikevegeta
  * The other half of the dynamic duo that was manually doing this before I made the tracker
  * Gave me good ideas for displaying progress throughout the seed

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TrackOMatic
{
    /// <summary>
    /// Interaction logic for EntranceTracker.xaml
    /// </summary>
    public partial class EntranceTracker : UserControl
    {
        private Dictionary<string, EntranceItem> entranceItems = new Dictionary<string, EntranceItem>();
        private List<string> allEntranceNames = new List<string>();

        public EntranceTracker()
        {
            InitializeComponent();
            InitializeEntrances();
            // After all entrances are added, provide the list to each entrance item
            foreach (var item in entranceItems.Values)
            {
                item.SetEntranceNamesList(allEntranceNames);
            }
        }

        private void InitializeEntrances()
        {
            // Add Level Lobby entrances
            AddEntranceCategory("LEVEL LOBBIES");
            AddEntrance("Japes Lobby to Jungle Japes", "Japes Lobby to Jungle Japes");
            AddEntrance("Jungle Japes to Japes Lobby", "Jungle Japes to Japes Lobby");
            AddEntrance("Aztec Lobby to Angry Aztec", "Aztec Lobby to Angry Aztec");
            AddEntrance("Angry Aztec to Aztec Lobby", "Angry Aztec to Aztec Lobby");
            AddEntrance("Factory Lobby to Frantic Factory", "Factory Lobby to Frantic Factory");
            AddEntrance("Frantic Factory to Factory Lobby", "Frantic Factory to Factory Lobby");
            AddEntrance("Galleon Lobby to Gloomy Galleon", "Galleon Lobby to Gloomy Galleon");
            AddEntrance("Gloomy Galleon to Galleon Lobby", "Gloomy Galleon to Galleon Lobby");
            AddEntrance("Fungi Lobby to Fungi Forest", "Fungi Lobby to Fungi Forest");
            AddEntrance("Fungi Forest to Fungi Lobby", "Fungi Forest to Fungi Lobby");
            AddEntrance("Caves Lobby to Crystal Caves", "Caves Lobby to Crystal Caves");
            AddEntrance("Crystal Caves to Caves Lobby", "Crystal Caves to Caves Lobby");
            AddEntrance("Castle Lobby to Creepy Castle", "Castle Lobby to Creepy Castle");
            AddEntrance("Creepy Castle to Castle Lobby", "Creepy Castle to Castle Lobby");
            AddEntrance("Helm Lobby to Hideout Helm", "Helm Lobby to Hideout Helm");
            AddEntrance("Hideout Helm to Helm Lobby", "Hideout Helm to Helm Lobby");

            // Add DK Isles Main Area entrances
            AddEntranceCategory("DK ISLES");
            AddEntrance("DK's Treehouse to Training Grounds", "DK's Treehouse to Training Grounds");
            AddEntrance("Training Grounds to DK's Treehouse", "Training Grounds to DK's Treehouse");
            AddEntrance("Training Grounds to DK Isles Main", "Training Grounds to DK Isles Main");
            AddEntrance("DK Isles Main to Training Grounds", "DK Isles Main to Training Grounds");
            AddEntrance("DK Isles Main to K-Lumsy", "DK Isles Main to K-Lumsy");
            AddEntrance("DK Isles K-Lumsy to Main", "DK Isles K-Lumsy to Main");
            AddEntrance("DK Isles Main to Banana Fairy Isle", "DK Isles Main to Banana Fairy Isle");
            AddEntrance("DK Isles Banana Fairy Isle to Main", "DK Isles Banana Fairy Isle to Main");
            AddEntrance("DK Isles Main to Snide's Room", "DK Isles Main to Snide's Room");
            AddEntrance("DK Isles Snide's Room to Main", "DK Isles Snide's Room to Main");
            AddEntrance("DK Isles Main to Japes Lobby", "DK Isles Main to Japes Lobby");
            AddEntrance("DK Isles Japes Lobby to Main", "DK Isles Japes Lobby to Main");
            AddEntrance("DK Isles Main to Aztec Lobby", "DK Isles Main to Aztec Lobby");
            AddEntrance("DK Isles Aztec Lobby to Main", "DK Isles Aztec Lobby to Main");
            AddEntrance("DK Isles Main to Factory Lobby", "DK Isles Main to Factory Lobby");
            AddEntrance("DK Isles Factory Lobby to Main", "DK Isles Factory Lobby to Main");
            AddEntrance("DK Isles Main to Galleon Lobby", "DK Isles Main to Galleon Lobby");
            AddEntrance("DK Isles Galleon Lobby to Main", "DK Isles Galleon Lobby to Main");
            AddEntrance("DK Isles Main to Fungi Lobby", "DK Isles Main to Fungi Lobby");
            AddEntrance("DK Isles Fungi Lobby to Main", "DK Isles Fungi Lobby to Main");
            AddEntrance("DK Isles Main to Caves Lobby", "DK Isles Main to Caves Lobby");
            AddEntrance("DK Isles Caves Lobby to Main", "DK Isles Caves Lobby to Main");
            AddEntrance("DK Isles Main to Castle Lobby", "DK Isles Main to Castle Lobby");
            AddEntrance("DK Isles Castle Lobby to Main", "DK Isles Castle Lobby to Main");
            AddEntrance("DK Isles Main to Helm Lobby", "DK Isles Main to Helm Lobby");
            AddEntrance("DK Isles Helm Lobby to Main", "DK Isles Helm Lobby to Main");

            // Add Jungle Japes entrances
            AddEntranceCategory("JUNGLE JAPES");
            AddEntrance("Jungle Japes Main to Mountain", "Jungle Japes Main to Mountain");
            AddEntrance("Jungle Japes Mountain to Main", "Jungle Japes Mountain to Main");
            AddEntrance("Jungle Japes Main to Painting Room", "Jungle Japes Main to Painting Room");
            AddEntrance("Jungle Japes Painting Room to Main", "Jungle Japes Painting Room to Main");
            AddEntrance("Jungle Japes Main to Underground", "Jungle Japes Main to Underground");
            AddEntrance("Jungle Japes Underground to Main", "Jungle Japes Underground to Main");
            AddEntrance("Jungle Japes Main to Beehive", "Jungle Japes Main to Beehive");
            AddEntrance("Jungle Japes Beehive to Main", "Jungle Japes Beehive to Main");
            AddEntrance("Jungle Japes Mountain to Minecart", "Jungle Japes Mountain to Minecart");
            AddEntrance("Jungle Japes Minecart to Main", "Jungle Japes Minecart to Main");

            // Add Angry Aztec entrances
            AddEntranceCategory("ANGRY AZTEC");
            AddEntrance("Angry Aztec Main to Tiny Temple", "Angry Aztec Main to Tiny Temple");
            AddEntrance("Angry Aztec Tiny Temple to Main", "Angry Aztec Tiny Temple to Main");
            AddEntrance("Angry Aztec Main to Donkey 5DTemple", "Angry Aztec Main to Donkey 5DTemple");
            AddEntrance("Angry Aztec Donkey 5DTemple to Main", "Angry Aztec Donkey 5DTemple to Main");
            AddEntrance("Angry Aztec Main to Diddy 5DTemple", "Angry Aztec Main to Diddy 5DTemple");
            AddEntrance("Angry Aztec Diddy 5DTemple to Main", "Angry Aztec Diddy 5DTemple to Main");
            AddEntrance("Angry Aztec Main to Lanky 5DTemple", "Angry Aztec Main to Lanky 5DTemple");
            AddEntrance("Angry Aztec Lanky 5DTemple to Main", "Angry Aztec Lanky 5DTemple to Main");
            AddEntrance("Angry Aztec Main to Tiny 5DTemple", "Angry Aztec Main to Tiny 5DTemple");
            AddEntrance("Angry Aztec Tiny 5DTemple To Main", "Angry Aztec Tiny 5DTemple To Main");
            AddEntrance("Angry Aztec Main to Chunky 5DTemple", "Angry Aztec Main to Chunky 5DTemple");
            AddEntrance("Angry Aztec Chunky 5DTemple to Main", "Angry Aztec Chunky 5DTemple to Main");
            AddEntrance("Angry Aztec Main to Beetle Race", "Angry Aztec Main to Beetle Race");
            AddEntrance("Angry Aztec Beetle Race to Main", "Angry Aztec Beetle Race to Main");
            AddEntrance("Angry Aztec Llama Temple to Main", "Angry Aztec Llama Temple to Main");
            AddEntrance("Angry Aztec Main to Llama Temple", "Angry Aztec Main to Llama Temple");

            // Add Frantic Factory entrances
            AddEntranceCategory("FRANTIC FACTORY");
            AddEntrance("Frantic Factory R&D to Car Race", "Frantic Factory R&D to Car Race");
            AddEntrance("Frantic Factory Car Race to R&D", "Frantic Factory Car Race to R&D");
            AddEntrance("Frantic Factory Main to Power Shed", "Frantic Factory Main to Power Shed");
            AddEntrance("Frantic Factory Power Room to Chunky Room", "Frantic Factory Power Room to Chunky Room");
            AddEntrance("Frantic Factory Main to Crusher Room", "Frantic Factory Main to Crusher Room");
            AddEntrance("Frantic Factory Crusher Room to Main", "Frantic Factory Crusher Room to Main");
            AddEntrance("Frantic Factory Storage to Arcade", "Frantic Factory Storage to Arcade");
            AddEntrance("Frantic Factory Arcade to Storage", "Frantic Factory Arcade to Storage");

            // Add Gloomy Galleon entrances
            AddEntranceCategory("GLOOMY GALLEON");
            AddEntrance("Gloomy Galleon Main to Lighthouse", "Gloomy Galleon Main to Lighthouse");
            AddEntrance("Gloomy Galleon Lighthouse to Main", "Gloomy Galleon Lighthouse to Main");
            AddEntrance("Gloomy Galleon Main to Mermaid", "Gloomy Galleon Main to Mermaid");
            AddEntrance("Gloomy Galleon Mermaid to Main", "Gloomy Galleon Mermaid to Main");
            AddEntrance("Gloomy Galleon Main to Seasick Ship", "Gloomy Galleon Main to Seasick Ship");
            AddEntrance("Gloomy Galleon Seasick Ship to Main", "Gloomy Galleon Seasick Ship to Main");
            AddEntrance("Gloomy Galleon Main to Seal Race", "Gloomy Galleon Main to Seal Race");
            AddEntrance("Gloomy Galleon Seal Race to Main", "Gloomy Galleon Seal Race to Main");
            AddEntrance("Gloomy Galleon Main to Submarine", "Gloomy Galleon Main to Submarine");
            AddEntrance("Gloomy Galleon Submarine to Main", "Gloomy Galleon Submarine to Main");
            AddEntrance("Gloomy Galleon Main to Lanky 2DShip", "Gloomy Galleon Main to Lanky 2DShip");
            AddEntrance("Gloomy Galleon Lanky 2DShip to Main", "Gloomy Galleon Lanky 2DShip to Main");
            AddEntrance("Gloomy Galleon Main to Tiny 2DShip", "Gloomy Galleon Main to Tiny 2DShip");
            AddEntrance("Gloomy Galleon Tiny 2DShip to Main", "Gloomy Galleon Tiny 2DShip to Main");
            AddEntrance("Gloomy Galleon Main to DK 5DShip", "Gloomy Galleon Main to DK 5DShip");
            AddEntrance("Gloomy Galleon DK 5DShip to Main", "Gloomy Galleon DK 5DShip to Main");
            AddEntrance("Gloomy Galleon Main to Diddy 5DShip", "Gloomy Galleon Main to Diddy 5DShip");
            AddEntrance("Gloomy Galleon Diddy 5DShip to Main", "Gloomy Galleon Diddy 5DShip to Main");
            AddEntrance("Gloomy Galleon Main to Lanky 5DShip", "Gloomy Galleon Main to Lanky 5DShip");
            AddEntrance("Gloomy Galleon Lanky 5DShip to Main", "Gloomy Galleon Lanky 5DShip to Main");
            AddEntrance("Gloomy Galleon Main to Tiny 5DShip", "Gloomy Galleon Main to Tiny 5DShip");
            AddEntrance("Gloomy Galleon Tiny 5DShip to Main", "Gloomy Galleon Tiny 5DShip to Main");
            AddEntrance("Gloomy Galleon Main to Chunky 5DShip", "Gloomy Galleon Main to Chunky 5DShip");
            AddEntrance("Gloomy Galleon Chunky 5DShip to Main", "Gloomy Galleon Chunky 5DShip to Main");
            AddEntrance("Gloomy Galleon Main to Treasure Chest", "Gloomy Galleon Main to Treasure Chest");
            AddEntrance("Gloomy Galleon Treasure Chest to Main", "Gloomy Galleon Treasure Chest to Main");
            AddEntrance("Gloomy Galleon Main to Mech Fish", "Gloomy Galleon Main to Mech Fish");
            AddEntrance("Gloomy Galleon Mech Fish to Main", "Gloomy Galleon Mech Fish to Main");

            // Add Fungi Forest entrances
            AddEntranceCategory("FUNGI FOREST");
            AddEntrance("Fungi Forest Main To Minecart", "Fungi Forest Main To Minecart");
            AddEntrance("Fungi Forest Minecart To Main", "Fungi Forest Minecart To Main");
            AddEntrance("Fungi Forest Main to Giant Mushroom (Lower)", "Fungi Forest Main to Giant Mushroom (Lower)");
            AddEntrance("Fungi Forest Giant Mushroom (Lower) to Main", "Fungi Forest Giant Mushroom (Lower) to Main");
            AddEntrance("Fungi Forest Main to Giant Mushroom (Middle Low)", "Fungi Forest Main to Giant Mushroom (Middle Low)");
            AddEntrance("Fungi Forest Giant Mushroom (Middle Low) to Main", "Fungi Forest Giant Mushroom (Middle Low) to Main");
            AddEntrance("Fungi Forest Main to Giant Mushroom (Middle High)", "Fungi Forest Main to Giant Mushroom (Middle High)");
            AddEntrance("Fungi Forest Giant Mushroom (Middle High) to Main", "Fungi Forest Giant Mushroom (Middle High) to Main");
            AddEntrance("Fungi Forest Main to Giant Mushroom (Top)", "Fungi Forest Main to Giant Mushroom (Top)");
            AddEntrance("Fungi Forest Giant Mushroom (Top) to Main", "Fungi Forest Giant Mushroom (Top) to Main");
            AddEntrance("Fungi Forest Main to Giant Mushroom (Night Door)", "Fungi Forest Main to Giant Mushroom (Night Door)");
            AddEntrance("Fungi Forest Giant Mushroom (Night Door) to Main", "Fungi Forest Giant Mushroom (Night Door) to Main");
            AddEntrance("Fungi Forest Main to Face Puzzle (Chunky)", "Fungi Forest Main to Face Puzzle (Chunky)");
            AddEntrance("Fungi Forest Face Puzzle (Chunky) to Main", "Fungi Forest Face Puzzle (Chunky) to Main");
            AddEntrance("Fungi Forest Main to Bouncy Shrooms (Lanky)", "Fungi Forest Main to Bouncy Shrooms (Lanky)");
            AddEntrance("Fungi Forest Bouncy Shrooms (Lanky) to Main", "Fungi Forest Bouncy Shrooms (Lanky) to Main");
            AddEntrance("Fungi Forest Main to Mushroom Puzzle (Lanky)", "Fungi Forest Main to Mushroom Puzzle (Lanky)");
            AddEntrance("Fungi Forest Mushroom Puzzle (Lanky) to Main", "Fungi Forest Mushroom Puzzle (Lanky) to Main");
            AddEntrance("Fungi Forest Main to Anthill", "Fungi Forest Main to Anthill");
            AddEntrance("Fungi Forest Anthill to Main", "Fungi Forest Anthill to Main");
            AddEntrance("Fungi Forest Main to Mill Back Door", "Fungi Forest Main to Mill Back Door");
            AddEntrance("Fungi Forest Mill Back Door to Main", "Fungi Forest Mill Back Door to Main");
            AddEntrance("Fungi Forest Main to Mill Tiny Hole", "Fungi Forest Main to Mill Tiny Hole");
            AddEntrance("Fungi Forest Mill Tiny Hole to Main", "Fungi Forest Mill Tiny Hole to Main");
            AddEntrance("Fungi Forest Main to Mill Front Door", "Fungi Forest Main to Mill Front Door");
            AddEntrance("Fungi Forest Mill Front Door to Main", "Fungi Forest Mill Front Door to Main");
            AddEntrance("Fungi Forest Main to Dark Rafters", "Fungi Forest Main to Dark Rafters");
            AddEntrance("Fungi Forest Dark Rafters to Main", "Fungi Forest Dark Rafters to Main");
            AddEntrance("Fungi Forest Main to Winch Room", "Fungi Forest Main to Winch Room");
            AddEntrance("Fungi Forest Winch Room to Main", "Fungi Forest Winch Room to Main");
            AddEntrance("Fungi Forest Main to Mill Attic (Lanky)", "Fungi Forest Main to Mill Attic (Lanky)");
            AddEntrance("Fungi Forest Mill Attic (Lanky) to Main", "Fungi Forest Mill Attic (Lanky) to Main");
            AddEntrance("Fungi Forest Back Mill to Spider", "Fungi Forest Back Mill to Spider");
            AddEntrance("Fungi Forest Spider to Back Mill", "Fungi Forest Spider to Back Mill");
            AddEntrance("Fungi Forest Back Mill to Front Mill", "Fungi Forest Back Mill to Front Mill");
            AddEntrance("Fungi Forest Front Mill to Back Mill", "Fungi Forest Front Mill to Back Mill");
            AddEntrance("Fungi Forest Main to Thornvine Barn", "Fungi Forest Main to Thornvine Barn");
            AddEntrance("Fungi Forest Thornvine Barn to Main", "Fungi Forest Thornvine Barn to Main");

            // Add Crystal Caves entrances
            AddEntranceCategory("CRYSTAL CAVES");
            AddEntrance("Crystal Caves Main to Beetle Race", "Crystal Caves Main to Beetle Race");
            AddEntrance("Crystal Caves Beetle Race to Main", "Crystal Caves Beetle Race to Main");
            AddEntrance("Crystal Caves Main to Ice Castle", "Crystal Caves Main to Ice Castle");
            AddEntrance("Crystal Caves Ice Castle to Main", "Crystal Caves Ice Castle to Main");
            AddEntrance("Crystal Caves Main to DK 5DIgloo", "Crystal Caves Main to DK 5DIgloo");
            AddEntrance("Crystal Caves DK 5DIgloo to Main", "Crystal Caves DK 5DIgloo to Main");
            AddEntrance("Crystal Caves Main to Diddy 5DIgloo", "Crystal Caves Main to Diddy 5DIgloo");
            AddEntrance("Crystal Caves Diddy 5DIgloo to Main", "Crystal Caves Diddy 5DIgloo to Main");
            AddEntrance("Crystal Caves Main to Lanky 5DIgloo", "Crystal Caves Main to Lanky 5DIgloo");
            AddEntrance("Crystal Caves Lanky 5DIgloo to Main", "Crystal Caves Lanky 5DIgloo to Main");
            AddEntrance("Crystal Caves Main to Tiny 5DIgloo", "Crystal Caves Main to Tiny 5DIgloo");
            AddEntrance("Crystal Caves Tiny 5DIgloo to Main", "Crystal Caves Tiny 5DIgloo to Main");
            AddEntrance("Crystal Caves Main to Chunky 5DIgloo", "Crystal Caves Main to Chunky 5DIgloo");
            AddEntrance("Crystal Caves Chunky 5DIgloo to Main", "Crystal Caves Chunky 5DIgloo to Main");
            AddEntrance("Crystal Caves Main to Rotating Room", "Crystal Caves Main to Rotating Room");
            AddEntrance("Crystal Caves Rotating Room to Main", "Crystal Caves Rotating Room to Main");
            AddEntrance("Crystal Caves Main to DK 5DCabin", "Crystal Caves Main to DK 5DCabin");
            AddEntrance("Crystal Caves DK 5DCabin to Main", "Crystal Caves DK 5DCabin to Main");
            AddEntrance("Crystal Caves Main to Diddy Lower 5DCabin", "Crystal Caves Main to Diddy Lower 5DCabin");
            AddEntrance("Crystal Caves Diddy Lower 5DCabin to Main", "Crystal Caves Diddy Lower 5DCabin to Main");
            AddEntrance("Crystal Caves Main to Diddy Upper 5DCabin", "Crystal Caves Main to Diddy Upper 5DCabin");
            AddEntrance("Crystal Caves Diddy Upper 5DCabin to Main", "Crystal Caves Diddy Upper 5DCabin to Main");
            AddEntrance("Crystal Caves Main to Lanky 1DCabin", "Crystal Caves Main to Lanky 1DCabin");
            AddEntrance("Crystal Caves Lanky 1DCabin to Main", "Crystal Caves Lanky 1DCabin to Main");
            AddEntrance("Crystal Caves Main to Tiny 5DCabin", "Crystal Caves Main to Tiny 5DCabin");
            AddEntrance("Crystal Caves Tiny 5DCabin to Main", "Crystal Caves Tiny 5DCabin to Main");
            AddEntrance("Crystal Caves Main to Chunky 5DCabin", "Crystal Caves Main to Chunky 5DCabin");
            AddEntrance("Crystal Caves Chunky 5DCabin to Main", "Crystal Caves Chunky 5DCabin to Main");

            // Add Creepy Castle entrances
            AddEntranceCategory("CREEPY CASTLE");
            AddEntrance("Creepy Castle Main to Tree", "Creepy Castle Main to Tree");
            AddEntrance("Creepy Castle Tree to Main", "Creepy Castle Tree to Main");
            AddEntrance("Creepy Castle Tree Drain to Main", "Creepy Castle Tree Drain to Main");
            AddEntrance("Creepy Castle Main to Library (Entrance)", "Creepy Castle Main to Library (Entrance)");
            AddEntrance("Creepy Castle Library (Entrance) to Main", "Creepy Castle Library (Entrance) to Main");
            AddEntrance("Creepy Castle Main to Library (Exit)", "Creepy Castle Main to Library (Exit)");
            AddEntrance("Creepy Castle Library (Exit) to Main", "Creepy Castle Library (Exit) to Main");
            AddEntrance("Creepy Castle Main to Ballroom", "Creepy Castle Main to Ballroom");
            AddEntrance("Creepy Castle Ballroom to Main", "Creepy Castle Ballroom to Main");
            AddEntrance("Creepy Castle Main to Tower", "Creepy Castle Main to Tower");
            AddEntrance("Creepy Castle Tower to Main", "Creepy Castle Tower to Main");
            AddEntrance("Creepy Castle Main to Greenhouse", "Creepy Castle Main to Greenhouse");
            AddEntrance("Creepy Castle Greenhouse (Entrance) to Main", "Creepy Castle Greenhouse (Entrance) to Main");
            AddEntrance("Creepy Castle Greenhouse (Exit) to Main", "Creepy Castle Greenhouse (Exit) to Main");
            AddEntrance("Creepy Castle Main to Trash Can", "Creepy Castle Main to Trash Can");
            AddEntrance("Creepy Castle Trash Can to Main", "Creepy Castle Trash Can to Main");
            AddEntrance("Creepy Castle Main to Shed", "Creepy Castle Main to Shed");
            AddEntrance("Creepy Castle Shed to Main", "Creepy Castle Shed to Main");
            AddEntrance("Creepy Castle Main to Museum", "Creepy Castle Main to Museum");
            AddEntrance("Creepy Castle Museum to Main", "Creepy Castle Museum to Main");
            AddEntrance("Creepy Castle Main to Lower Cave", "Creepy Castle Main to Lower Cave");
            AddEntrance("Creepy Castle Lower Cave to Main", "Creepy Castle Lower Cave to Main");
            AddEntrance("Creepy Castle Main to Upper Cave (Back door)", "Creepy Castle Main to Upper Cave (Back door)");
            AddEntrance("Creepy Castle Upper Cave to Main (Back door)", "Creepy Castle Upper Cave to Main (Back door)");
            AddEntrance("Creepy Castle Main to Upper Cave (Moat door)", "Creepy Castle Main to Upper Cave (Moat door)");
            AddEntrance("Creepy Castle Upper Cave to Main (Moat door)", "Creepy Castle Upper Cave to Main (Moat door)");
            AddEntrance("Creepy Castle Ballroom to Museum", "Creepy Castle Ballroom to Museum");
            AddEntrance("Creepy Castle Museum to Ballroom", "Creepy Castle Museum to Ballroom");
            AddEntrance("Creepy Castle Museum to Car Race", "Creepy Castle Museum to Car Race");
            AddEntrance("Creepy Castle Car Race to Museum", "Creepy Castle Car Race to Museum");
            AddEntrance("Creepy Castle Lower Cave to Crypt", "Creepy Castle Lower Cave to Crypt");
            AddEntrance("Creepy Castle Crypt to Lower Cave", "Creepy Castle Crypt to Lower Cave");
            AddEntrance("Creepy Castle Lower Cave to Mausoleum", "Creepy Castle Lower Cave to Mausoleum");
            AddEntrance("Creepy Castle Mausoleum to Lower cave", "Creepy Castle Mausoleum to Lower cave");
            AddEntrance("Creepy Castle Crypt to Minecart", "Creepy Castle Crypt to Minecart");
            AddEntrance("Creepy Castle Minecart to Crypt", "Creepy Castle Minecart to Crypt");
            AddEntrance("Creepy Castle Upper Cave to Dungeon", "Creepy Castle Upper Cave to Dungeon");
            AddEntrance("Creepy Castle Dungeon to Upper Cave", "Creepy Castle Dungeon to Upper Cave");
        }

        private void AddEntranceCategory(string categoryName)
        {
            var categoryBlock = new TextBlock
            {
                Text = categoryName,
                FontFamily = (FontFamily)Application.Current.Resources["DK64Font"],
                FontSize = 16,
                Foreground = Brushes.White,
                Background = (Brush)Application.Current.Resources["BaseBG"],
                Padding = new Thickness(10, 5, 10, 5),
                Margin = new Thickness(0, 10, 0, 5)
            };
            EntrancesPanel.Children.Add(categoryBlock);
        }

        private void AddEntrance(string displayName, string internalName)
        {
            var entranceItem = new EntranceItem(displayName, internalName);
            entranceItems[internalName] = entranceItem;
            allEntranceNames.Add(displayName);
            EntrancesPanel.Children.Add(entranceItem);
        }

        private void ClearAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in entranceItems.Values)
            {
                item.ClearDestination();
            }
        }

        public void SetEntranceDestination(string entranceName, string destination)
        {
            if (entranceItems.ContainsKey(entranceName))
            {
                entranceItems[entranceName].SetDestination(destination);
            }
        }

        public Dictionary<string, string> GetEntranceMapping()
        {
            var mapping = new Dictionary<string, string>();
            foreach (var kvp in entranceItems)
            {
                var destination = kvp.Value.GetDestination();
                if (!string.IsNullOrEmpty(destination))
                {
                    mapping[kvp.Key] = destination;
                }
            }
            return mapping;
        }

        public void Reset()
        {
            foreach (var item in entranceItems.Values)
            {
                item.ClearDestination();
            }
        }
    }
}

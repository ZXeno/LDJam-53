namespace DefaultNamespace
{
    using System.Collections.Generic;
    using UnityEngine;

    public static class Names
    {
        private static List<string> usedNames = new List<string>();
        public static string GetRandomStationName()
        {
            string randomName = SpaceStationNames[Random.Range(0, SpaceStationNames.Length)];
            while (usedNames.Contains(randomName))
            {
                randomName = SpaceStationNames[Random.Range(0, SpaceStationNames.Length)];
            }

            return randomName;
        }

        public static string[] SpaceStationNames = {
            "Nova Station",
            "Starfall Station",
            "Nebula Outpost",
            "Interstellar Hub",
            "Gravity Point Station",
            "Cosmic Nexus",
            "Hyperion Station",
            "Black Hole Base",
            "Lunar Gateway",
            "Celestial Observatory",
            "Orion Outpost",
            "Solar Flare Station",
            "Quantum Nexus",
            "Delta Quadrant Station",
            "Galactic Exchange",
            "Plasma Point Station",
            "Asteroid Outpost",
            "Stellar Anchor",
            "Warp Way Station",
            "Infinity Point Station",
            "Dark Matter Station",
            "Nova Prime",
            "Supernova Station",
            "Cosmic Crossroads",
            "Aurora Station",
            "Meteorite Outpost",
            "Planetary Beacon",
            "Starlight Station",
            "Gravity Well Station",
            "Deep Space Station",
            "Nebula Crossing",
            "Pulsar Point Station",
            "Lunar Colony",
            "Interstellar Junction",
            "Solaris Station",
            "Terra Nova",
            "Alpha Centauri Outpost",
            "Interstellar Oasis",
            "Quantum Station",
            "Beyond Horizon Station"
        };
    }
}
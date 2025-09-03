using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace AtomicSimulation.Authoring
{
    [BurstCompile]
    public static class AtomicData
    {
        public static readonly FixedString64Bytes[] ElementNames = new FixedString64Bytes[]
        {
            "Hydrogen", "Helium", "Lithium", "Beryllium", "Boron", "Carbon", "Nitrogen", "Oxygen",
            "Fluorine", "Neon", "Sodium", "Magnesium", "Aluminum", "Silicon", "Phosphorus", "Sulfur",
            "Chlorine", "Argon", "Potassium", "Calcium", "Scandium", "Titanium", "Vanadium", "Chromium",
            "Manganese", "Iron", "Cobalt", "Nickel", "Copper", "Zinc", "Gallium", "Germanium",
            "Arsenic", "Selenium", "Bromine", "Krypton", "Rubidium", "Strontium", "Yttrium", "Zirconium",
            "Niobium", "Molybdenum", "Technetium", "Ruthenium", "Rhodium", "Palladium", "Silver", "Cadmium",
            "Indium", "Tin", "Antimony", "Tellurium", "Iodine", "Xenon", "Cesium", "Barium", "Lanthanum",
            "Cerium", "Praseodymium", "Neodymium", "Promethium", "Samarium", "Europium", "Gadolinium", "Terbium",
            "Dysprosium", "Holmium", "Erbium", "Thulium", "Ytterbium", "Lutetium", "Hafnium", "Tantalum",
            "Tungsten", "Rhenium", "Osmium", "Iridium", "Platinum", "Gold", "Mercury", "Thallium",
            "Lead", "Bismuth", "Polonium", "Astatine", "Radon", "Francium", "Radium", "Actinium",
            "Thorium", "Protactinium", "Uranium", "Neptunium", "Plutonium", "Americium", "Curium", "Berkelium",
            "Californium", "Einsteinium", "Fermium", "Mendelevium", "Nobelium", "Lawrencium", "Rutherfordium",
            "Dubnium", "Seaborgium", "Bohrium", "Hassium", "Meitnerium", "Darmstadtium", "Roentgenium", "Copernicium",
            "Nihonium", "Flerovium", "Moscovium", "Livermorium", "Tennessine", "Oganesson"
        };


        public static readonly FixedList32Bytes<int> MaxElectronsPerShell = new()
        {
            Length = 7,
            [0] = 2, // K shell
            [1] = 8, // L shell  
            [2] = 18, // M shell
            [3] = 32, // N shell
            [4] = 32, // O shell
            [5] = 18, // P shell
            [6] = 8 // Q shell
        };

        // don't change we need this scale to gamify
        public static readonly FixedList32Bytes<float> ShellRadius = new()
        {
            Length = 7,
            [0] = 0.5f, // K shell (n=1, Bohr radius for hydrogen)
            [1] = .65f, // L shell (n=2)
            [2] = 0.8f, // M shell (n=3)
            [3] = 1.0f, // N shell (n=4)
            [4] = 1.2f, // O shell (n=5)
            [5] = 1.35f, // P shell (n=6)
            [6] = 1.5f // Q shell (n=7)
        };


        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ElectronInitialAngle(
            int electronIndex, int electronsInShell,
            float3 centerPos, float shellRadius,
            out float initialAngle, out float3 orbitPos
        )
        {
            initialAngle = (float)electronIndex / electronsInShell * 2f * math.PI;

            orbitPos = centerPos + new float3(
                shellRadius * math.cos(initialAngle),
                shellRadius * math.sin(initialAngle),
                0f
            );
        }

        /// <summary>
        /// Simplified neutron approximation
        /// </summary>
        /// <param name="atomicNumber"></param>
        /// <returns></returns>
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SimplifiedNeutronCount(int atomicNumber)
        {
            if (atomicNumber <= 2) return math.max(0, atomicNumber - 1);
            if (atomicNumber <= 20) return atomicNumber;
            return (int)(atomicNumber * 1.4f);
        }

        /// <summary>
        /// Simplified Neutron Nucleus approximation
        /// </summary>
        /// <param name="atomicNumber"></param>
        /// <param name="neutrons"></param>
        /// <param name="nucleus"></param>
        [BurstCompile]
        public static void SimplifiedNeutronNucleusCount(int atomicNumber, out int neutrons, out int nucleus)
        {
            neutrons = SimplifiedNeutronCount(atomicNumber);
            nucleus = atomicNumber + neutrons;
        }

        [BurstCompile]
        public static void GetNucleusParticleOffset(
            int particleIndex, int totalParticles,
            float m, float c,
            out float3 nucleusOffset
        )
        {
            if (totalParticles == 1) nucleusOffset = float3.zero;

            float radius = c + (totalParticles * m);

            // Golden spiral distribution for roughly uniform sphere packing
            float goldenAngle = 2.39996322972865332f;
            float theta = particleIndex * goldenAngle;
            float phi = math.acos(1f - 2f * particleIndex / totalParticles);

            nucleusOffset = new float3(
                radius * math.sin(phi) * math.cos(theta),
                radius * math.sin(phi) * math.sin(theta),
                radius * math.cos(phi)
            );
        }
    }
}
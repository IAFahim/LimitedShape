using System.Runtime.CompilerServices;
using AtomicSimulation.Core;
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
                0f,
                shellRadius * math.sin(initialAngle)
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
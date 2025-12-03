using ColossalFramework.Math;
using UnityEngine;

namespace Commons.Utils
{
    public class RandomPastelColorGenerator(uint randomSeed, float randomizerFactor, PastelConfig pastelConfig)
    {
        private readonly Randomizer m_random = new(randomSeed);
        private readonly float m_randomizerFactor = randomizerFactor;
        private readonly PastelConfig m_pastelConfig = pastelConfig;


        /// <summary>
        /// Returns a random pastel color
        /// </summary>
        /// <returns></returns>
        public Color32 GetNext()
        {
            byte[] colorBytes =
            [
                (byte) (m_random.UInt32(128) + 64),
                (byte) (m_random.UInt32(128) + 64),
                (byte) (m_random.UInt32(128) + 64),
            ];
            uint colorDestaq = 7;
            if (m_randomizerFactor != 1)
            {
                colorDestaq = m_random.UInt32(7);
                colorDestaq &= ((uint) ~m_pastelConfig & 0b111);
                if ((m_pastelConfig & PastelConfig.AVOID_NEUTRALS) > 0 && colorDestaq % 7 == 0)
                {
                    colorDestaq = (m_random.UInt32(5) + 1) & (uint) ~m_pastelConfig;
                }
                colorDestaq %= 7;
                if (colorDestaq == 0)
                {
                    colorDestaq = 7;
                }
            }

            Color32 color = new()
            {
                a = 255,
                r = (byte) (colorBytes[0] / ((colorDestaq & 0b100) > 0 ? 1 : m_randomizerFactor)),
                g = (byte) (colorBytes[1] / ((colorDestaq & 0b10) > 0 ? 1 : m_randomizerFactor)),
                b = (byte) (colorBytes[2] / ((colorDestaq & 0b1) > 0 ? 1 : m_randomizerFactor))
            };

            return color;
        }
    }
}
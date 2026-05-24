using ColossalFramework.Globalization;
using Commons.Utils.StructExtensions;
using System;
using UnityEngine;

namespace Commons.Utils.UtilitiesClasses 
{
	public struct CardinalPoint
    {
        public static readonly string[] m_cardinal16 =
        [
            "N",
            "NNE",
            "NE",
            "ENE",
            "E",
            "ESE",
            "SE",
            "SSE",
            "S",
            "SSW",
            "SW",
            "WSW",
            "W",
            "WNW",
            "NW",
            "NNW",
        ];

        private static string GetCardinalPoint16_internal(float angle)
        {
            float diagSize = 22.5f;
            angle %= 360;
            angle += 360;
            angle %= 360;

            for (int i = 1; i < m_cardinal16.Length; i++)
            {
                if (Math.Abs(angle - diagSize * i) < diagSize / 2)
                {
                    return m_cardinal16[i];
                }
            }
            return m_cardinal16[0];
        }
        public static string GetCardinalPoint16LocalizedShort(float angle) => Locale.Get("CMNS_CARDINALPOINT_SHORT", GetCardinalPoint16_internal(angle));

        public static CardinalPoint GetCardinalPoint(float angle, float diagSize = 45)
        {
            angle %= 360;
            angle += 360;
            angle %= 360;

            if (Math.Abs(angle - 45) < diagSize / 2)
            {
                return NE;
            }
            else if (Math.Abs(angle - 90) < diagSize / 2)
            {
                return E;
            }
            else if (Math.Abs(angle - 135) < diagSize / 2)
            {
                return SE;
            }
            else if (Math.Abs(angle - 180) < diagSize / 2)
            {
                return S;
            }
            else if (Math.Abs(angle - 225) < diagSize / 2)
            {
                return SW;
            }
            else if (Math.Abs(angle - 270) < diagSize / 2)
            {
                return W;
            }
            else if (Math.Abs(angle - 315) < diagSize / 2)
            {
                return NW;
            }
            else
            {
                return N;
            }
        }

        public static CardinalPoint GetCardinalPoint4(float angle, bool azimutal = false)
        {
            angle %= 360;
            if (azimutal)
            {
                angle += 630;
            }
            else
            {
                angle += 360;
            }
            angle %= 360;

            if (angle < 135f && angle >= 45f)
            {
                return E;
            }
            else if (angle < 45f || angle >= 315f)
            {
                return N;
            }
            else if (angle < 315f && angle >= 225f)
            {
                return W;
            }
            else
            {
                return S;
            }
        }

        private CardinalInternal InternalValue { get; set; }

        public readonly CardinalInternal Value => InternalValue;

        public static readonly CardinalPoint N = CardinalInternal.N;
        public static readonly CardinalPoint E = CardinalInternal.E;
        public static readonly CardinalPoint S = CardinalInternal.S;
        public static readonly CardinalPoint W = CardinalInternal.W;
        public static readonly CardinalPoint NE = CardinalInternal.NE;
        public static readonly CardinalPoint SE = CardinalInternal.SE;
        public static readonly CardinalPoint SW = CardinalInternal.SW;
        public static readonly CardinalPoint NW = CardinalInternal.NW;
        public static readonly CardinalPoint ZERO = CardinalInternal.ZERO;

        public static implicit operator CardinalPoint(CardinalInternal otherType) => new()
        {
            InternalValue = otherType
        };

        public static implicit operator CardinalInternal(CardinalPoint otherType) => otherType.InternalValue;

        public readonly int StepsTo(CardinalPoint other)
        {
            if (other.InternalValue == InternalValue)
            {
                return 0;
            }

            if ((((int)other.InternalValue) & ((int)other.InternalValue - 1)) != 0 || (((int)InternalValue) & ((int)InternalValue - 1)) != 0)
            {
                return int.MaxValue;
            }

            CardinalPoint temp = other;
            int count = 0;
            while (temp.InternalValue != InternalValue)
            {
                temp++;
                count++;
            }
            if (count > 4)
            {
                count -= 8;
            }

            return count;
        }

        public static int operator -(CardinalPoint c, CardinalPoint other) => c.StepsTo(other);

        public readonly Vector2 GetCardinalOffset()
        {
            return InternalValue switch
            {
                CardinalInternal.E => new Vector2(1, 0),
                CardinalInternal.W => new Vector2(-1, 0),
                CardinalInternal.N => new Vector2(0, 1),
                CardinalInternal.S => new Vector2(0, -1),
                CardinalInternal.NE => new Vector2(1, 1),
                CardinalInternal.NW => new Vector2(-1, 1),
                CardinalInternal.SE => new Vector2(1, -1),
                CardinalInternal.SW => new Vector2(-1, -1),
                _ => Vector2.zero,
            };
        }


        public readonly Vector2 GetCardinalOffset2D()
        {
            return InternalValue switch
            {
                CardinalInternal.E => new Vector2(1, 0),
                CardinalInternal.W => new Vector2(-1, 0),
                CardinalInternal.S => new Vector2(0, 1),
                CardinalInternal.N => new Vector2(0, -1),
                CardinalInternal.SE => new Vector2(1, 1),
                CardinalInternal.SW => new Vector2(-1, 1),
                CardinalInternal.NE => new Vector2(1, -1),
                CardinalInternal.NW => new Vector2(-1, -1),
                _ => Vector2.zero,
            };
        }

        public readonly int GetCardinalAngle()
        {
            return InternalValue switch
            {
                CardinalInternal.N => 0,
                CardinalInternal.S => 180,
                CardinalInternal.E => 90,
                CardinalInternal.W => 270,
                CardinalInternal.NE => 45,
                CardinalInternal.NW => 315,
                CardinalInternal.SE => 135,
                CardinalInternal.SW => 225,
                _ => 0,
            };
        }
        public readonly byte GetCardinalIndex8()
        {
            return InternalValue switch
            {
                CardinalInternal.N => 0,
                CardinalInternal.S => 4,
                CardinalInternal.E => 2,
                CardinalInternal.W => 6,
                CardinalInternal.NE => 1,
                CardinalInternal.NW => 7,
                CardinalInternal.SE => 3,
                CardinalInternal.SW => 5,
                _ => 8,
            };
        }

        public static CardinalPoint operator ++(CardinalPoint c)
        {
            return c.InternalValue switch
            {
                CardinalInternal.N => NE,
                CardinalInternal.NE => E,
                CardinalInternal.E => SE,
                CardinalInternal.SE => S,
                CardinalInternal.S => SW,
                CardinalInternal.SW => W,
                CardinalInternal.W => NW,
                CardinalInternal.NW => N,
                _ => ZERO,
            };
        }

        public static CardinalPoint operator --(CardinalPoint c)
        {
            return c.InternalValue switch
            {
                CardinalInternal.N => NW,
                CardinalInternal.NE => N,
                CardinalInternal.E => NE,
                CardinalInternal.SE => E,
                CardinalInternal.S => SE,
                CardinalInternal.SW => S,
                CardinalInternal.W => SW,
                CardinalInternal.NW => W,
                _ => ZERO,
            };
        }

        public static CardinalPoint operator &(CardinalPoint c1, CardinalPoint c2) => new()
        {
            InternalValue = c1.InternalValue & c2.InternalValue
        };

        public static CardinalPoint operator |(CardinalPoint c1, CardinalPoint c2) => new()
        {
            InternalValue = c1.InternalValue | c2.InternalValue
        };

        public override readonly int GetHashCode() => base.GetHashCode();

        public override readonly bool Equals(object o) => o.GetType() == GetType() && this == ((CardinalPoint)o);

        public static bool operator ==(CardinalPoint c1, CardinalPoint c2) => c1.InternalValue == c2.InternalValue;

        public static bool operator <(CardinalPoint left, CardinalPoint right) => (Compare(left, right) < 0);

        public static bool operator >(CardinalPoint left, CardinalPoint right) => (Compare(left, right) > 0);

        public readonly int CompareTo(CardinalPoint other)
        {
            if (this == other)
            {
                return 0;
            }

            var a = GetCardinalAngle();
            var b = other.GetCardinalAngle() + 360;
            if (b - a > 180)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }

        public static int Compare(CardinalPoint left, CardinalPoint right)
        {
            if (object.ReferenceEquals(left, right))
            {
                return 0;
            }
            if (object.ReferenceEquals(left, default(CardinalPoint)))
            {
                return -1;
            }
            return left.CompareTo(right);
        }


        public static bool operator !=(CardinalPoint c1, CardinalPoint c2) => c1.InternalValue != c2.InternalValue;

        public static CardinalPoint operator ~(CardinalPoint c)
        {
            return c.InternalValue switch
            {
                CardinalInternal.N => S,
                CardinalInternal.NE => SW,
                CardinalInternal.E => W,
                CardinalInternal.SE => NW,
                CardinalInternal.S => N,
                CardinalInternal.SW => NE,
                CardinalInternal.W => E,
                CardinalInternal.NW => SE,
                _ => ZERO,
            };
            ;
        }

        public enum CardinalInternal
        {
            N = 1,
            NE = 2,
            E = 4,
            SE = 8,
            S = 0x10,
            SW = 0x20,
            W = 0x40,
            NW = 0x80,
            ZERO = 0
        }

        public readonly Vector2 GetPointForAngle(Vector2 p1, float distance) => p1 + GetCardinalOffset() * distance;


        public override readonly string ToString() => InternalValue.ToString();
        public readonly string ToStringLocalized8() => Locale.Get("CMNS_CARDINALPOINT_SHORT", InternalValue.ToString());

        public static CardinalPoint GetCardinal2D(Vector2 p1, Vector2 p2)
        {
            Vector2 p1Inv = new(p1.x, -p1.y);
            Vector2 p2Inv = new(p2.x, -p2.y);
            return GetCardinalPoint((p1Inv).GetAngleToPoint(p2Inv));
        }

        public static CardinalPoint GetCardinal2D4(Vector2 p1, Vector2 p2)
        {
            Vector2 p1Inv = new(p1.x, -p1.y);
            Vector2 p2Inv = new(p2.x, -p2.y);
            return GetCardinalPoint4((p1Inv).GetAngleToPoint(p2Inv));
        }

    }
}

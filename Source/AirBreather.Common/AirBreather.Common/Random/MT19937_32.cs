// Ported from mt19937ar.c, which contains this copyright header:
/* 
   A C-program for MT19937, with initialization improved 2002/1/26.
   Coded by Takuji Nishimura and Makoto Matsumoto.

   Before using, initialize the state by using init_genrand(seed)  
   or init_by_array(init_key, key_length).

   Copyright (C) 1997 - 2002, Makoto Matsumoto and Takuji Nishimura,
   All rights reserved.                          

   Redistribution and use in source and binary forms, with or without
   modification, are permitted provided that the following conditions
   are met:

     1. Redistributions of source code must retain the above copyright
        notice, this list of conditions and the following disclaimer.

     2. Redistributions in binary form must reproduce the above copyright
        notice, this list of conditions and the following disclaimer in the
        documentation and/or other materials provided with the distribution.

     3. The names of its contributors may not be used to endorse or promote 
        products derived from this software without specific prior written 
        permission.

   THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
   "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
   LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
   A PARTICULAR PURPOSE ARE DISCLAIMED.  IN NO EVENT SHALL THE COPYRIGHT OWNER OR
   CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
   EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
   PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
   PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
   LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
   NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
   SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.


   Any feedback is very welcome.
   http://www.math.sci.hiroshima-u.ac.jp/~m-mat/MT/emt.html
   email: m-mat @ math.sci.hiroshima-u.ac.jp (remove space)
*/
using System;
using System.Diagnostics.CodeAnalysis;

using static System.FormattableString;

namespace AirBreather.Random
{
    public struct MT19937_32State : IEquatable<MT19937_32State>, IRandomGeneratorState
    {
        internal uint[] data;
        internal int idx;

        public MT19937_32State(uint seed)
        {
            this.data = new uint[624];
            this.data[0] = seed;
            for (int i = 1; i < 624; i++)
            {
                uint prev = this.data[i - 1];
                this.data[i] = unchecked((1812433253 * (prev ^ (prev >> 30))) + (uint)i);
            }

            this.idx = 624;
        }

        public MT19937_32State(MT19937_32State copyFrom)
        {
            this.idx = copyFrom.idx;

            if (copyFrom.data == null)
            {
                this.data = null;
            }
            else
            {
                this.data = new uint[624];
                Buffer.BlockCopy(copyFrom.data, 0, this.data, 0, 624 * sizeof(uint));
            }
        }

        // TODO: our cousins in other languages allow seeding by multiple input values... support that too.
        public bool IsValid => StateIsValid(this);

        private static bool StateIsValid(MT19937_32State state)
        {
            if (state.data == null)
            {
                return false;
            }

            if (!state.idx.IsInRange(0, 625))
            {
                return false;
            }

            uint accumulator = 0;
            foreach (uint value in state.data)
            {
                accumulator |= value;
            }

            return accumulator != 0;
        }

        public static bool Equals(MT19937_32State first, MT19937_32State second)
        {
            if (first.idx != second.idx)
            {
                return false;
            }

            bool firstIsDefault = first.data == null;
            bool secondIsDefault = second.data == null;

            if (firstIsDefault != secondIsDefault)
            {
                return false;
            }

            if (firstIsDefault)
            {
                return true;
            }

            uint accumulator = 0;
            for (int i = 0; i < 624; i++)
            {
                uint differentBits = first.data[i] ^ second.data[i];
                accumulator |= differentBits;
            }

            return accumulator == 0;
        }

        public static int GetHashCode(MT19937_32State state)
        {
            int hashCode = HashCode.Seed;

            hashCode = hashCode.HashWith(state.idx);

            if (state.data == null)
            {
                return hashCode;
            }

            uint accumulator = 0;
            foreach (uint value in state.data)
            {
                accumulator ^= value;
            }

            hashCode = hashCode.HashWith(accumulator);

            return hashCode;
        }

        public static string ToString(MT19937_32State state) => Invariant($"{nameof(MT19937_32State)}[]");

        public static bool operator ==(MT19937_32State first, MT19937_32State second) => Equals(first, second);
        public static bool operator !=(MT19937_32State first, MT19937_32State second) => !Equals(first, second);
        public override bool Equals(object obj) => obj is MT19937_32State && Equals(this, (MT19937_32State)obj);
        public bool Equals(MT19937_32State other) => Equals(this, other);
        public override int GetHashCode() => GetHashCode(this);
        public override string ToString() => ToString(this);
    }

    public sealed class MT19937_32Generator : IRandomGenerator<MT19937_32State>
    {
        /// <summary>
        /// The size of each "chunk" of bytes that can be generated at a time.
        /// </summary>
        public static readonly int ChunkSize = sizeof(uint);

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        int IRandomGenerator<MT19937_32State>.ChunkSize => ChunkSize;

        /// <inheritdoc />
        public MT19937_32State FillBuffer(MT19937_32State state, byte[] buffer, int index, int count)
        {
            buffer.ValidateNotNull(nameof(buffer));
            index.ValidateInRange(nameof(index), 0, buffer.Length);

            if (buffer.Length - index < count)
            {
                throw new ArgumentException("Not enough room", nameof(buffer));
            }

            if (index % ChunkSize != 0)
            {
                throw new ArgumentException("Must be a multiple of ChunkSize.", nameof(index));
            }

            if (count % ChunkSize != 0)
            {
                throw new ArgumentException("Must be a multiple of ChunkSize.", nameof(count));
            }

            if (!state.IsValid)
            {
                throw new ArgumentException("State is not valid; use the parameterized constructor to initialize a new instance with the given seed values.", nameof(state));
            }

            state = new MT19937_32State(state);
            FillBufferCore();
            return state;

            unsafe void FillBufferCore()
            {
                fixed (uint* fData = state.data)
                fixed (byte* fbuf = buffer)
                {
                    // count has already been validated to be a multiple of ChunkSize,
                    // and so has index, so we can do this fanciness without fear.
                    uint* pbuf = (uint*)(fbuf + index);
                    uint* pend = pbuf + (count / ChunkSize);
                    while (pbuf < pend)
                    {
                        if (state.idx == 624)
                        {
                            Twist(fData);
                            state.idx = 0;
                        }

                        uint x = fData[state.idx++];

                        x ^= (x >> 11);
                        x ^= (x << 7) & 0x9D2C5680;
                        x ^= (x << 15) & 0xEFC60000;
                        x ^= (x >> 18);

                        *(pbuf++) = x;
                    }
                }

                unsafe void Twist(uint* vals)
                {
                    const uint Upper01 = 0x80000000;
                    const uint Lower31 = 0x7FFFFFFF;

                    for (int curr = 0; curr < 624; curr++)
                    {
                        int near = (curr + 1) % 624;
                        int far = (curr + 397) % 624;

                        uint x = vals[curr] & Upper01;
                        uint y = vals[near] & Lower31;
                        uint z = vals[far] ^ ((x | y) >> 1);

                        vals[curr] = z ^ ((y & 1) * 0x9908B0DF);
                    }
                }
            }
        }
    }
}

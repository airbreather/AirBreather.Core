﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;

using Xunit;
using Xunit.Abstractions;

using AirBreather.Common.Random;

namespace AirBreather.Common.Tests
{
    public sealed class XorShift128PlusTests
    {
        private readonly ITestOutputHelper output;

        public XorShift128PlusTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Theory]
        [InlineData(1234524356ul, 47845723665ul, 10356027574996968ul, 421627830503766283ul, 7267806761253193977ul)]
        [InlineData(262151541652562ul, 468594272265ul, 3923822141990852456ul, 3993942717521754294ul, 13070632098572223408ul)]
        public void Test(ulong s0, ulong s1, ulong expectedResult1, ulong expectedResult2, ulong expectedResult3)
        {
            // this was params ulong[], but I like InlineData too much to make that work...
            ulong[] expectedResults = { expectedResult1, expectedResult2, expectedResult3 };

            var gen = new XorShift128PlusGenerator();
            var state = new RngState128(s0, s1);
            byte[] buf = new byte[expectedResults.Length * 8];

            // First, do it in separate calls.
            for (int i = 0; i < expectedResults.Length; i++)
            {
                state = gen.FillBuffer(state, buf, i * 8, 8);
                Assert.Equal(expectedResults[i], BitConverter.ToUInt64(buf, i * 8));
            }

            // Now, do it all in one call.
            state = new RngState128(s0, s1);
            state = gen.FillBuffer(state, buf, 0, buf.Length);
            for (int i = 0; i < expectedResults.Length; i++)
            {
                Assert.Equal(expectedResults[i], BitConverter.ToUInt64(buf, i * 8));
            }

            // Now, repeat the same for the extension method.
            state = new RngState128(s0 ,s1);
            ulong[] typedBuf = new ulong[expectedResults.Length];
            for (int i = 0; i < expectedResults.Length; i++)
            {
                state = gen.FillBuffer(state, typedBuf, i, 1);
                Assert.Equal(expectedResults[i], typedBuf[i]);
            }

            // again, all in one call
            state = new RngState128(s0, s1);
            typedBuf = new ulong[expectedResults.Length];
            state = gen.FillBuffer(state, typedBuf, 0, typedBuf.Length);
            for (int i = 0; i < expectedResults.Length; i++)
            {
                Assert.Equal(expectedResults[i], typedBuf[i]);
            }

            // Now, ensure that it throws if we're out of alignment.
            Assert.Throws<ArgumentException>("index", () => state = gen.FillBuffer(state, buf, 3, 8));
        }

        [Theory]
        [InlineData(1234524356ul, 47845723665ul, 1)]
        [InlineData(1234524356ul, 47845723665ul, 2)]
        [InlineData(1234524356ul, 47845723665ul, 4)]
        [InlineData(1234524356ul, 47845723665ul, 8)]
        [InlineData(1234524356ul, 47845723665ul, 16)]
        [InlineData(1234524356ul, 47845723665ul, 32)]
        [InlineData(1234524356ul, 47845723665ul, 64)]
        public unsafe void SpeedTestSingleArray(ulong s0, ulong s1, int chunks)
        {
            var gen = new XorShift128PlusGenerator();

            // stage 1: set up the initial state, output buffer, and chunk size.
            var initialState = new RngState128(s0, s1);

            const int OutputBufferLength = 1 << 30;
            var outputBuffer = new byte[OutputBufferLength];
            var chunkSize = OutputBufferLength / chunks;

            // stage 2: use that state to set up the parallel independent states.
            var parallelStateBuffer = new byte[sizeof(RngState128) * chunks];
            gen.FillBuffer(initialState, parallelStateBuffer);

            var parallelStates = new RngState128[chunks];
            fixed (byte* sFixed = parallelStateBuffer)
            fixed (RngState128* tFixed = parallelStates)
            {
                var s = (RngState128*)sFixed;
                var sEnd = s + chunks;
                var t = tFixed;

                while (s < sEnd)
                {
                    *(t++) = *(s++);
                }
            }

            Stopwatch sw = Stopwatch.StartNew();

            // stage 3: do those chunks in parallel
            const int Reps = 3;
            for (int rep = 0; rep < Reps; rep++)
            {
                Parallel.For(0, chunks, i =>
                {
                    parallelStates[i] = gen.FillBuffer(parallelStates[i], outputBuffer, i * chunkSize, chunkSize);
                });
            }

            sw.Stop();

            double seconds = sw.ElapsedTicks / (double)Stopwatch.Frequency / (double)Reps;
            this.output.WriteLine("XorShift128PlusTests.SpeedTestSingleArray: Took an average of {0:N5} seconds to fill a single buffer with a size of {1:N0} bytes ({2:N5} GiB per second) by directly writing to {3} chunk(s).",
                                  seconds,
                                  OutputBufferLength,
                                  OutputBufferLength / seconds / (1 << 30),
                                  chunks.ToString().PadLeft(2));

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        [Theory]
        [InlineData(1234524356ul, 47845723665ul, 1)]
        [InlineData(1234524356ul, 47845723665ul, 2)]
        [InlineData(1234524356ul, 47845723665ul, 4)]
        [InlineData(1234524356ul, 47845723665ul, 8)]
        [InlineData(1234524356ul, 47845723665ul, 16)]
        [InlineData(1234524356ul, 47845723665ul, 32)]
        [InlineData(1234524356ul, 47845723665ul, 64)]
        public unsafe void SpeedTestSeparateArraysWithMergeAtEnd(ulong s0, ulong s1, int chunks)
        {
            var gen = new XorShift128PlusGenerator();

            // stage 1: set up the initial state, output buffer, and chunk size.
            var initialState = new RngState128(s0, s1);

            const int OutputBufferLength = 1 << 30;
            var outputBuffer = new byte[OutputBufferLength];
            var chunkSize = OutputBufferLength / chunks;

            // stage 2: use that state to set up the parallel independent states.
            var parallelStateBuffer = new byte[sizeof(RngState128) * chunks];
            gen.FillBuffer(initialState, parallelStateBuffer);

            var parallelStates = new RngState128[chunks];
            fixed (byte* sFixed = parallelStateBuffer)
            fixed (RngState128* tFixed = parallelStates)
            {
                var s = (RngState128*)sFixed;
                var sEnd = s + chunks;
                var t = tFixed;

                while (s < sEnd)
                {
                    *(t++) = *(s++);
                }
            }

            // stage 2.99: preallocate buffers for the different chunks.
            // doing "new byte[chunkSize]" inside stopwatch block would be unfair.
            byte[][] chunkBuffers = new byte[chunks][];
            for (int i = 0; i < chunkBuffers.Length; i++)
            {
                chunkBuffers[i] = new byte[chunkSize];
            }

            Stopwatch sw = Stopwatch.StartNew();

            // stage 3: do those chunks in parallel, with a serial merge.
            const int Reps = 3;
            for (int rep = 0; rep < Reps; rep++)
            {
                Parallel.For(0, chunks, i =>
                {
                    gen.FillBuffer(parallelStates[i], chunkBuffers[i]);

                    // it's actually permissible to do this now, but testing suggests that
                    // this is actually slower overall than doing the copies at the end.
                    ////Buffer.BlockCopy(chunkBuffers[i], 0, outputBuffer, i * chunkSize, chunkSize);
                });

                for (int i = 0; i < chunkBuffers.Length; i++)
                {
                    Buffer.BlockCopy(chunkBuffers[i], 0, outputBuffer, i * chunkSize, chunkSize);
                }
            }

            sw.Stop();

            // Note two things about this, compared to the other test.
            // Not only is it *slower* than writing to the single big buffer,
            // but it requires *greater* peak memory consumption overall.
            // HOWEVER, the other one has one particular disadvantage: it fixes
            // the *entire* array of 1 GiB for the duration.
            double seconds = sw.ElapsedTicks / (double)Stopwatch.Frequency / (double)Reps;
            this.output.WriteLine("XorShift128PlusTests.SpeedTestSeparateArraysWithMergeAtEnd: Took an average of {0:N5} seconds to fill a single buffer with a size of {1:N0} bytes ({2:N5} GiB per second) by writing to {3} new chunk(s) in parallel and merging.",
                                  seconds,
                                  OutputBufferLength,
                                  OutputBufferLength / seconds / (1 << 30),
                                  chunks.ToString().PadLeft(2));

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }
    }
}

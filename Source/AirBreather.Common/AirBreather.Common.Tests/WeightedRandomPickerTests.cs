﻿using System;
using System.Collections.Generic;
using System.Linq;

using Xunit;
using Xunit.Abstractions;

using AirBreather.Random;

namespace AirBreather.Tests
{
    public sealed class WeightedRandomPickerTests
    {
        private readonly ITestOutputHelper output;

        public WeightedRandomPickerTests(ITestOutputHelper output) => this.output = output;

        [Fact]
        public void TestWeightedRandomPicker()
        {
            var builder = new WeightedRandomPicker<string>.Builder();

            var valsWithWeights = new Dictionary<string, int>
            {
                ["orly"] = 2,
                ["yarly"] = 1,
                ["nowai"] = 77,
                ["yaweh"] = 2
            };

            foreach (var (item, weight) in valsWithWeights)
            {
                builder = builder.AddWithWeight(item, weight);
            }

            var picker = builder.Build();

            var dct = valsWithWeights.Keys.ToDictionary(x => x, x => 0);

            const int TrialCount = 10000;
            var span = CryptographicRandomGenerator.GetBuffer(TrialCount * 8).AsSpan().NonPortableCast<byte, ulong>();
            for (int i = 0; i < span.Length; ++i)
            {
                ++dct[picker.Pick(span[i] / (double)UInt64.MaxValue)];
            }

            double scalar = TrialCount / (double)valsWithWeights.Values.Sum();
            var expect = valsWithWeights.ToDictionary(kvp => kvp.Key, kvp => kvp.Value * scalar);

            foreach (var (item, actualCount) in dct)
            {
                this.output.WriteLine("{0} appeared {1} times (expected {2})", item, actualCount, expect[item]);
            }
        }
    }
}

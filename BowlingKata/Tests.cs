using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xunit;

namespace BowlingKata
{
    public static class Kata
    {
        private class Roll
        {
            public Roll() { }


            public Roll(char value, int frame, int index)
            {
                Value = value;
                Frame = frame;
                Index = index;
                Score = GetScore();
            }

            public char Value { get; }
            public int Frame { get; }
            public int Index { get; }

            public int Score { get; }

            public bool IsStrike => Value == 'X';

            public bool IsSpare => Value == '/';

            private int GetScore()
            {

                if (Value == 'X')
                {
                    return 10;
                }
                else if (Value == '/')
                {
                    return 0;
                }

                return Value - '0';
            }
        }

        private class Rolls : Collection<Roll>
        {
            private Rolls() { }

            public Rolls(string inputString)
            {

                var index = 0;

                var frameStrings = inputString.Split(' ')
                    .Select((f, i) => new { f, Frame = i + 1 });

                foreach (var f in frameStrings)
                {
                    foreach (var r in f.f.ToCharArray())
                    {
                        this.Add(new Roll(r, f.Frame, index));
                        index++;
                    }
                }
            }

            public Roll GetPriorRoll(int index) => GetNextRoll(index, inReverse: true);

            public Roll GetNextRoll(int index, bool inReverse = false)
            {
                var toIterateOn = inReverse ? this.Reverse() : this;

                var roll = toIterateOn.Where(x => inReverse ? (x.Index < index) : (x.Index > index))
                    .FirstOrDefault();
                return roll ?? new Roll();
            }
        }

        public static int BowlingScore(string inputString)
        {
            var score = 0;

            var rolls = new Rolls(inputString);

            foreach (var roll in rolls)
            {
                if (roll.IsStrike)
                {
                    score += roll.Score;

                    if (roll.Frame < 10)
                    {
                        var bonus1 = rolls.GetNextRoll(roll.Index);
                        var bonus2 = rolls.GetNextRoll(roll.Index + 1);

                        var bonus1Score = bonus1.Score;
                        var bonus2Score = bonus2.Value == '/' ? 10 - bonus1Score : bonus2.Score;

                        score += bonus1Score;
                        score += bonus2Score;
                    }
                }
                else if (roll.IsSpare)
                {
                    var prior = rolls.GetPriorRoll(roll.Index);
                    var priorScore = prior.Score;

                    score += 10 - priorScore;

                    if (roll.Frame < 10)
                    {
                        var bonus1 = rolls.GetNextRoll(roll.Index);
                        score += bonus1.Score;
                    }
                }
                else
                {
                    score += roll.Score;
                }
            }

            return score;
        }




        public class Tests
        {
            [Theory]
            [InlineData("Perfect", 300, "X X X X X X X X X XXX")]
            [InlineData("Bumpers", 20, "11 11 11 11 11 11 11 11 11 11")]

            [InlineData("RandomTest", 83, "9/ 24 60 4/ 00 4/ 4/ 3/ 25 12")]
            [InlineData("RandomTest2", 125, "4/ 7/ 5/ 14 7/ 5/ 6/ 8/ 43 34")]
            [InlineData("RandomTest3", 126, "52 3/ 72 3/ 6/ 5/ X 54 52 34")]
            [InlineData("RandomTest4", 148, "X 8/ X 4/ 53 07 8/ 8/ 7/ 53")]

            [InlineData("BasicTest", 115, "00 5/ 4/ 53 33 22 4/ 5/ 45 XXX")]
            [InlineData("BasicTest2", 150, "5/ 4/ 3/ 2/ 1/ 0/ X 9/ 4/ 8/8")]
            public void TestAll(string testCase, int expectedScore, string input)
            {
                // act
                var score = Kata.BowlingScore(input);

                // assert
                score.Should().Be(expectedScore, because: testCase);
            }
        }
    }
}
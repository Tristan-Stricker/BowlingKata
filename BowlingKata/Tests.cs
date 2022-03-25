using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace BowlingKata
{
    public static class Kata
    {
        private class Roll
        {
            public char Value { get; set; }
            public int Frame { get; set; }
            public int Index { get; set; }

            public bool IsStrike => Value == 'X';

            public bool IsSpare => Value == '/';
        }

        public static int BowlingScore(string inputString)
        {
            var score = 0;

            var frameStrings = inputString.Split(' ')
                .Select((f, i) => new { f, Frame = i + 1 });

            var index = 0;
            var rolls = new List<Roll>();
            foreach (var f in frameStrings)
            {
                foreach (var r in f.f.ToCharArray())
                {
                    rolls.Add(new Roll { Frame = f.Frame, Value = r, Index = index });
                    index++;
                }
            }

            foreach (var roll in rolls)
            {
                if (roll.IsStrike)
                {
                    score += ScoreIt(roll.Value);

                    if (roll.Frame < 10)
                    {
                        var bonus1 = GetNextRoll(rolls, roll.Index);
                        var bonus2 = GetNextRoll(rolls, roll.Index + 1);

                        var bonus1Score = ScoreIt(bonus1);
                        var bonus2Score = bonus2 == '/' ? 10 - bonus1Score : ScoreIt(bonus2);

                        score += bonus1Score;
                        score += bonus2Score;
                    }
                }
                else if (roll.IsSpare)
                {
                    var prior = GetPriorRoll(rolls, roll.Index);
                    var priorScore = ScoreIt(prior);

                    score += 10 - priorScore;

                    if (roll.Frame < 10)
                    {
                        var bonus1 = GetNextRoll(rolls, roll.Index);
                        score += ScoreIt(bonus1);
                    }
                }
                else
                {
                    score += ScoreIt(roll.Value);
                }
            }

            return score;
        }

        private static int ScoreIt(char c)
        {
            if (c == 'X')
            {
                return 10;
            }
            else if (c == '/')
            {
                return 0;
            }

            return c - '0';
        }
        private static char GetPriorRoll(IEnumerable<Roll> vs, int index) => GetNextRoll(vs, index, inReverse: true);

        private static char GetNextRoll(IEnumerable<Roll> vs, int index, bool inReverse = false)
        {
            var toIterateOn = inReverse ? vs.Reverse() : vs;

            var roll = toIterateOn.Where(x => inReverse ? (x.Index < index) : (x.Index > index))
                .FirstOrDefault();
            return roll != null ? roll.Value : default;
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
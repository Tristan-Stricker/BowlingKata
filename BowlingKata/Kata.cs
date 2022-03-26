namespace BowlingKata;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

public static partial class Kata
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

    private class Rolls : ReadOnlyCollection<Roll>
    {
        public Rolls(IList<Roll> list) : base(list)
        {
        }

        public static Rolls Create(string inputString)
        {
            var rollIndex = 0;

            var frameStrings = inputString.Split(' ')
                .Select((f, i) => new { f, Frame = i + 1 });

            var list = new List<Roll>();

            foreach (var f in frameStrings)
            {
                foreach (var r in f.f.ToCharArray())
                {
                    list.Add(new Roll(r, f.Frame, rollIndex));
                    rollIndex++;
                }
            }

            return new Rolls(list);
        }

        public IList<Frame> AsFrames()
        {
            return this.GroupBy(x => x.Frame).Select(g => new Frame(g.Key, g)).ToList();
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

    private class Frame
    {
        private Frame() { }


        public Frame(int frameNumber, IEnumerable<Roll> rolls)
        {

        }        
    }

    private class Frames : ReadOnlyCollection<Frame>
    {
        private readonly Rolls rolls;

        public Frames(IList<Frame> list) : base(list)
        {
        }

        public Frames(Rolls rolls) : this(rolls.AsFrames())
        {
            this.rolls = rolls;
        }

        internal int GetScores()
        {
            var score = 0;
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
                    score += 10 - prior.Score;

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
    }

    public static int BowlingScore(string inputString)
    {
        var rolls = Rolls.Create(inputString);
        var frames = new Frames(rolls);
        return frames.GetScores();      
    }
}

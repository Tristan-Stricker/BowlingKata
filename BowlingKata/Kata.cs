namespace BowlingKata;

using System.Collections.Generic;
using System.Linq;

public static class Kata
{
    private class Roll
    {
        public Roll(char value, int frame)
        {
            Value = value;
            FrameNumber = frame;
        }

        public char Value { get; }

        public int FrameNumber { get; }

        public bool IsStrike => Value == 'X';

        public bool IsSpare => Value == '/';

        public bool IsInFinalFrame => FrameNumber == 10;
    }

    public static int BowlingScore(string inputString)
    {
        var rollList = inputString.Split(' ')
                .Select((frameString, i) => new { frameString, Frame = i + 1 })
                .Select(r => r.frameString.Select(c => new Roll(c, r.Frame)))
                .SelectMany(r => r); ;

        var rolls = new LinkedList<Roll>(rollList);
        var score = 0;

        for (var node = rolls.First; node != null; node = node.Next)
        {
            var roll = node.Value;
            score += Score(node);

            if (roll.IsStrike && !roll.IsInFinalFrame)
            {
                score += Score(node.Next);
                score += Score(node.Next?.Next);
            }
            else if (roll.IsSpare && !roll.IsInFinalFrame)
            {
                score += Score(node.Next);
            }
        }

        return score;
    }

    private static int Score(LinkedListNode<Roll>? rollNode)
    {
        var roll = rollNode.Value;

        if (roll.IsStrike)
        {
            return 10;
        }
        else if (roll.IsSpare)
        {
            return 10 - Score(rollNode.Previous);
        }
        else
        {
            return roll.Value - '0';
        }
    }
}

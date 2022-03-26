namespace BowlingKata;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

public static class Kata
{
    private class Roll
    {
        public Roll() { }

        public Roll(char value, int frame)
        {
            Value = value;
            Frame = frame;
        }

        public char Value { get; }
        public int Frame { get; }

        public bool IsStrike => Value == 'X';

        public bool IsSpare => Value == '/';
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
                    list.Add(new Roll(r, f.Frame));
                    rollIndex++;
                }
            }

            return new Rolls(list);
        }

        public LinkedList<Roll> AsLinkedList()
        {
            return new LinkedList<Roll>(this);
        }
    }

    public static int BowlingScore(string inputString)
    {
        var rolls = Rolls.Create(inputString).AsLinkedList();
        var score = 0;

        for (var node = rolls.First; node != null; node = node.Next)
        {
            var roll = node.Value;
            score += Score(node);

            if (roll.IsStrike && roll.Frame < 10)
            {
                score += Score(node.Next) + Score(node.Next?.Next);
            }
            else if (roll.IsSpare && roll.Frame < 10)
            {
                score += Score(node.Next);
            }
        }

        return score;
    }

    private static int Score(LinkedListNode<Roll>? rollNode)
    {
        if (rollNode == null)
        {
            return 0;
        }

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

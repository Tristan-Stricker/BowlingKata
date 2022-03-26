using FluentAssertions;
using Xunit;

namespace BowlingKata;

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


namespace CBBTop;

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public partial class AttackLogParser
{
    public class AttackEntry
    {
        public string? Blocker { get; set; }
        public string? Attack { get; set; }
        public int? Index { get; set; }
    }

    public static Dictionary<string, Dictionary<int, Dictionary<int, AttackEntry>>> ParseLog(List<string> lines)
    {
        var results = new Dictionary<string, Dictionary<int, Dictionary<int, AttackEntry>>>
        {
            { "rooks", new() },
            { "bishops", new() }
        };

        var byIndex = new Dictionary<string, Dictionary<int, Dictionary<int, int>>> // piece -> square -> index -> perm
        {
            { "rooks", new() },
            { "bishops", new() }
        };

        string? currentPiece = null;

        var blockerRegex = BlockerRegex();
        var attackRegex = AttackRegex();

        foreach (var rawLine in lines)
        {
            var line = rawLine.Trim();

            if (line.Contains("Initializing Rook attack table"))
                currentPiece = "rooks";
            else if (line.Contains("Initializing Bishop attack table"))
                currentPiece = "bishops";
            else if (line.Contains("attack table initialized"))
                currentPiece = null;
            else if (currentPiece != null)
            {
                var blockerMatch = blockerRegex.Match(line);
                var attackMatch = attackRegex.Match(line);

                if (blockerMatch.Success)
                {
                    var perm = int.Parse(blockerMatch.Groups[1].Value);
                    var square = int.Parse(blockerMatch.Groups[2].Value);
                    var value = blockerMatch.Groups[3].Value;

                    if (!results[currentPiece].TryGetValue(square, out var squareDict))
                    {
                        squareDict = [];
                        results[currentPiece][square] = squareDict;
                    }

                    squareDict[perm] = new AttackEntry { Blocker = value };
                }
                else if (attackMatch.Success)
                {
                    var perm = int.Parse(attackMatch.Groups[1].Value);
                    var square = int.Parse(attackMatch.Groups[2].Value);
                    var index = int.Parse(attackMatch.Groups[3].Value);
                    var value = attackMatch.Groups[4].Value;

                    if (!results[currentPiece].TryGetValue(square, out var squareDict))
                    {
                        squareDict = [];
                        results[currentPiece][square] = squareDict;
                    }

                    if (!squareDict.TryGetValue(perm, out var entry))
                    {
                        entry = new AttackEntry();
                        squareDict[perm] = entry;
                    }

                    entry.Attack = value;
                    entry.Index = index;

                    if (!byIndex[currentPiece].TryGetValue(square, out var indexDict))
                    {
                        indexDict = new Dictionary<int, int>();
                        byIndex[currentPiece][square] = indexDict;
                    }

                    indexDict[index] = perm;
                }
            }
        }

        return results;
    }

    [GeneratedRegex(@"Blocker (\d+) at square (\d+): ([0-9A-Fa-f]{16})")]
    private static partial Regex BlockerRegex();
    [GeneratedRegex(@"Attack perm (\d+) at square (\d+) placed in index (\d+): ([0-9A-Fa-f]{16})")]
    private static partial Regex AttackRegex();
}

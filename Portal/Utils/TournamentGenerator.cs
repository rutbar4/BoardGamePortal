using Portal.DTO;

namespace Portal.Utils
{
    public class TournamentGenerator
    {
        public static List<TournamentMatch> Generate(List<TournamentParticipant> players)
        {
            Console.WriteLine("Generating table");

            int playersCount = players.Count;
            var roundsNumber = (int)Math.Log(playersCount, 2);

            List<TournamentRound> rounds = GenerateEmptyTable(players.Count);
            PopulateTableWithPlayers(rounds, players);

            rounds.Reverse();
            for (int i = 0; i < rounds.Count; i++)
            {
                var tournamentRoundText = (i + 1).ToString();
                rounds[i].Matches.ForEach(match => match.TournamentRoundText = tournamentRoundText);
            }
            return rounds.SelectMany(r => r.Matches).ToList();
        }

        private static void PopulateTableWithPlayers(List<TournamentRound> emptyTableRounds, List<TournamentParticipant> players)
        {
            var playersQueue = new Queue<TournamentParticipant>(players);
            var firstRoundMatches = emptyTableRounds.Last().Matches;

            foreach (var match in firstRoundMatches)
            {
                match.PlayerA = playersQueue.Dequeue();
                match.PlayerB = playersQueue.Dequeue();
            }

            bool playersFitInBracket = players.Count == MathF.Pow(2, emptyTableRounds.Count);
            Console.WriteLine($"Players fit in bracket: {playersFitInBracket}");

            if (!playersFitInBracket)
            {
                //Add new round, for runoffs
                TournamentRound runoffsTournamentRound = new TournamentRound() { Matches = new List<TournamentMatch>() };

                for (var i = 0; i < firstRoundMatches.Count; i++)
                {
                    var match = firstRoundMatches[i];

                    if (playersQueue.Count == 0)//fill the rest of the tournament table with empty cells
                    {
                        TournamentMatch emptyRunoff1 = new TournamentMatch { ID = "Runoff1_null_" + match.ID, NextMatchId = match.ID, PlayerA = new TournamentParticipant { Name = "-" }, PlayerB = new TournamentParticipant { Name = "-" } };
                        TournamentMatch emptyRunoff2 = new TournamentMatch { ID = "Runoff2_null_" + match.ID, NextMatchId = match.ID, PlayerA = new TournamentParticipant { Name = "-" }, PlayerB = new TournamentParticipant { Name = "-" } };
                        runoffsTournamentRound.Matches.Add(emptyRunoff1);
                        runoffsTournamentRound.Matches.Add(emptyRunoff2);
                        continue;
                    }

                    TournamentMatch runoffMatch = new TournamentMatch { ID = "Runoff1_" + match.ID, NextMatchId = match.ID, PlayerA = match.PlayerA, PlayerB = match.PlayerB };
                    runoffsTournamentRound.Matches.Add(runoffMatch);

                    if (playersQueue.Count == 1)
                    {
                        match.PlayerA = null;
                        match.PlayerB = playersQueue.Dequeue();
                        TournamentMatch emptyRunoff2 = new TournamentMatch { ID = "Runoff2_null_" + match.ID, NextMatchId = match.ID, PlayerA = new TournamentParticipant { Name = "-" }, PlayerB = new TournamentParticipant { Name = "-" } };
                        runoffsTournamentRound.Matches.Add(emptyRunoff2);
                        continue;
                    }
                    else if (playersQueue.Count > 1)
                    {
                        match.PlayerA = null;
                        match.PlayerB = null;
                        TournamentMatch runoffMatch2 = new TournamentMatch { ID = "Runoff2_" + match.ID, NextMatchId = match.ID, PlayerA = playersQueue.Dequeue(), PlayerB = playersQueue.Dequeue() };
                        runoffsTournamentRound.Matches.Add(runoffMatch2);
                    }
                }

                emptyTableRounds.Add(runoffsTournamentRound);
            }

        }

        private static List<TournamentRound> GenerateEmptyTable(int playersCount)
        {
            int matchCounter = 0;//used for matches IDs incrementation

            var roundsNumber = (int)Math.Log(playersCount, 2);
            var rounds = new TournamentRound[roundsNumber];
            for (int i = 0; i < roundsNumber; i++)
            {
                var round = new TournamentRound();
                var prevRound = i > 0 ? rounds[i - 1] : null;
                if (prevRound == null)
                {
                    // if first round - result is known
                    round.Matches = new List<TournamentMatch>(){
                        new TournamentMatch() {
                            NextMatchId = null
                        }
                    };
                }
                else
                {
                    round.Matches = new List<TournamentMatch>(prevRound.Matches.Count * 2);
                    foreach (var match in prevRound.Matches)
                    {
                        round.Matches.Add(new TournamentMatch()
                        {
                            NextMatchId = match.ID
                        });
                        round.Matches.Add(new TournamentMatch()
                        {
                            NextMatchId = match.ID
                        });
                    }
                }
                rounds[i] = round;
            }

            return rounds.ToList();
        }

        public static List<TournamentParticipant> GetTestPlayers(int playerCount) => Enumerable.Range(1, playerCount).Select(number => new TournamentParticipant() { Name = "TournamentParticipant" + number }).ToList();
    }
}


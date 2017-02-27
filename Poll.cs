using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordCardBot
{
    #region Using

    using System.Collections.Generic;
    using System.Linq;
    using Discord;

    #endregion

    public enum VoteResult
    {
        Abstain,
        Yes,
        No
    }

    public class Poll
    {
        #region Public Fields + Properties

        public User Creator { get; }

        public bool IsPollComplete
            => _votes.Values.Count(x => x != null) == _votes.Keys.Count;

        public string Question { get; set; }

        #endregion Public Fields + Properties

        #region Private Fields + Properties

        readonly Dictionary<ulong, VoteResult?> _votes =
            new Dictionary<ulong, VoteResult?>();

        #endregion Private Fields + Properties

        #region Public Constructors

        public Poll(User creator, string question, IEnumerable<ulong> voters)
        {
            Creator = creator;
            Question = question;
            foreach (var voter in voters)
            {
                _votes.Add(voter, null);
            }
        }

        #endregion Public Constructors

        #region Public Methods

        public int GetAbstainCount()
            => _votes.Values.Count(x => x == VoteResult.Abstain);

        public int GetNoCount() => _votes.Values.Count(x => x == VoteResult.No);

        public int GetYesCount()
            => _votes.Values.Count(x => x == VoteResult.Yes);

        public void UpdateVote(ulong voter, VoteResult vote)
            => _votes[voter] = vote;

        public bool UpdateVote(ulong voter, string vote)
        {
            switch (vote.ToLower())
            {
                case "yes":
                case "y":
                    _votes[voter] = VoteResult.Yes;
                    return true;

                case "no":
                case "n":
                    _votes[voter] = VoteResult.No;
                    return true;

                case "abstain":
                case "idk":
                case "a":
                    _votes[voter] = VoteResult.Abstain;
                    return true;

                default:
                    return false;
            }
        }

        #endregion Public Methods
    }
}
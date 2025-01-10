using Steamworks.Data;

namespace Steamworks.Ugc
{
	public struct UserItemVote
	{
		public bool VotedUp;

		public bool VotedDown;

		public bool VoteSkipped;

		internal static UserItemVote? From(GetUserItemVoteResult_t result)
		{
			UserItemVote value = default(UserItemVote);
			value.VotedUp = result.VotedUp;
			value.VotedDown = result.VotedDown;
			value.VoteSkipped = result.VoteSkipped;
			return value;
		}
	}
}

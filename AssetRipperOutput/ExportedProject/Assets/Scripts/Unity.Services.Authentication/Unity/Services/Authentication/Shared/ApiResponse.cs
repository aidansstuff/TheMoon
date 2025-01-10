namespace Unity.Services.Authentication.Shared
{
	internal class ApiResponse : IApiResponse
	{
		public int StatusCode { get; internal set; }

		public Multimap<string, string> Headers { get; internal set; }

		public string ErrorText { get; internal set; }

		public string RawContent { get; internal set; }

		public virtual object Content => null;

		public bool IsSuccessful
		{
			get
			{
				if (StatusCode >= 200)
				{
					return StatusCode < 300;
				}
				return false;
			}
		}

		public bool IsRedirection
		{
			get
			{
				if (StatusCode >= 300)
				{
					return StatusCode < 400;
				}
				return false;
			}
		}

		public bool IsClientError
		{
			get
			{
				if (StatusCode >= 400)
				{
					return StatusCode < 500;
				}
				return false;
			}
		}

		public bool IsServerError
		{
			get
			{
				if (StatusCode >= 500)
				{
					return StatusCode < 600;
				}
				return false;
			}
		}
	}
	internal class ApiResponse<T> : ApiResponse, IApiResponse
	{
		public T Data { get; internal set; }

		public override object Content => Data;
	}
}

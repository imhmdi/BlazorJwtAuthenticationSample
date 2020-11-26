using System;
using System.Web;
using Microsoft.AspNetCore.Components;
using System.Collections.Specialized;

namespace BlazorJwtAuthenticationSample.Client.Helpers
{
    public static class UriExtentions
	{
        public static NameValueCollection QueryString(this NavigationManager navigationManager)
        {
            return HttpUtility.ParseQueryString(new Uri(navigationManager.Uri).Query);
        }

        public static string QueryString(this NavigationManager navigationManager, string key)
        {
            return navigationManager.QueryString()[key];
        }

		public static bool IsLocalUrl(this string url)
		{
			if (string.IsNullOrEmpty(url))
			{
				return false;
			}
			if (url[0] == '/')
			{
				if (url.Length == 1)
				{
					return true;
				}
				if (url[1] != '/' && url[1] != '\\')
				{
					return !HasControlCharacter(url.AsSpan(1));
				}
				return false;
			}
			if (url[0] == '~' && url.Length > 1 && url[1] == '/')
			{
				if (url.Length == 2)
				{
					return true;
				}
				if (url[2] != '/' && url[2] != '\\')
				{
					return !HasControlCharacter(url.AsSpan(2));
				}
				return false;
			}
			return false;
			static bool HasControlCharacter(ReadOnlySpan<char> readOnlySpan)
			{
				for (int i = 0; i < readOnlySpan.Length; i++)
				{
					if (char.IsControl(readOnlySpan[i]))
					{
						return true;
					}
				}
				return false;
			}
		}

	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace TebyanFilmRobat
{
	public static class PingChecker
	{
		public static async Task<KeyValuePair<string, long>> GetTheFastestAsync(List<string> proxyIps, Action<string> action)
		{
			KeyValuePair<string, long> theFastestProxy = new KeyValuePair<string, long>(string.Empty, -1);
			List<KeyValuePair<string, long>> proxiesWithRoundtripTime = new List<KeyValuePair<string, long>>();
			List<Task<KeyValuePair<string, long>>> tasksList = proxyIps.Select(PingAsync).ToList();
			foreach (Task<KeyValuePair<string, long>> task in tasksList)
			{
				KeyValuePair<string, long> taskResult = await task;
				if (taskResult.Value >= long.MaxValue) continue;
				proxiesWithRoundtripTime.Add(new KeyValuePair<string, long>(taskResult.Key, taskResult.Value));
				action(string.Format("Ping \"{0}\", Reply Time: \"{1}\"", taskResult.Key, taskResult.Value));
			}
			if (!proxiesWithRoundtripTime.Any()) return theFastestProxy;
			theFastestProxy = proxiesWithRoundtripTime.FirstOrDefault(q => q.Value == proxiesWithRoundtripTime.Min(c => c.Value));
			return theFastestProxy;
		}

		public async static Task<KeyValuePair<string, long>> PingAsync(string ip)
		{
			try
			{
				var ipWithoutPort = ip.Remove(ip.LastIndexOf(":", StringComparison.InvariantCultureIgnoreCase));
				KeyValuePair<string, long> keyValuePair = new KeyValuePair<string, long>(ip, long.MaxValue);
				Ping ping = new Ping();
				Task<PingReply> pingReplyTask = ping.SendPingAsync(ipWithoutPort, 2000);
				if (pingReplyTask == null) return keyValuePair;
				PingReply pingReply = await pingReplyTask;
				if (pingReply.Status == IPStatus.Success)
					keyValuePair = new KeyValuePair<string, long>(ip, pingReply.RoundtripTime);
				return keyValuePair;
			}
			catch
			{
				
			}
			return new KeyValuePair<string, long>(ip, long.MaxValue);
		}
	}
}

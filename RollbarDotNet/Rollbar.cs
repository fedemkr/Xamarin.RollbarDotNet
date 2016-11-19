﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RollbarDotNet
{
	public static class Rollbar
	{
		private static RollbarConfig _config;
		private static Func<Person> _personFunc;

		public static void Init(RollbarConfig config = null)
		{
			if (config == null)
			{
				config = new RollbarConfig();
			}

			_config = config;
		}

		public static void PersonData(Func<Person> personFunc)
		{
			_personFunc = personFunc;
		}

		public static async Task<Guid?> Report(System.Exception e, ErrorLevel? level = ErrorLevel.Error, IDictionary<string, object> custom = null)
		{
			return await SendBody(new Body(e), level, custom);
		}

		public static async Task<Guid?> Report(string message, ErrorLevel? level = ErrorLevel.Error, IDictionary<string, object> custom = null)
		{
			return await SendBody(new Body(new Message(message)), level, custom);
		}

		private static async Task<Guid?> SendBody(Body body, ErrorLevel? level, IDictionary<string, object> custom)
		{
			if (string.IsNullOrWhiteSpace(_config.AccessToken) || _config.Enabled == false)
			{
				return null;
			}

			var guid = Guid.NewGuid();

			var client = new RollbarClient(_config);
			var data = new Data(_config.Environment, body)
			{
				Custom = custom,
				Level = level ?? _config.LogLevel
			};

			var payload = new Payload(_config.AccessToken, data);
			payload.Data.GuidUuid = guid;
			payload.Data.Person = _personFunc?.Invoke();

			_config.Transform?.Invoke(payload);
			await client.PostItemAsync(payload);

			return guid;
		}
	}
}

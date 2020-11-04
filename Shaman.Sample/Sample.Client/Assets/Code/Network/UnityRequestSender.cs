using System;
using System.Collections;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Shaman.Client;
using Shaman.Common.Utils.Logging;
using Shaman.Contract.Common.Logging;
using Shaman.Serialization;
using Shaman.Serialization.Messages;
using Shaman.Serialization.Messages.Http;
using Shaman.Serialization.Messages.Udp;
using UnityEngine;


namespace Code.Network
{
	public class UnityRequestSender : MonoBehaviour, IRequestSender
	{
		private IShamanLogger _logger { get; set; }
		private ISerializer _serializer { get; set; }

		public void Initialize(IShamanLogger logger, ISerializer serializer)
		{
			_logger = logger;
			_serializer = serializer;
		}
		
		public Task<T> SendRequest<T>(string url, RequestBase request) where T : ResponseBase, new()
		{
			throw new NotImplementedException();
		}
		
		public async Task<T> SendRequest<T>(string serviceUrl, HttpRequestBase request) where T : HttpResponseBase, new()
		{
			var responseObject = new T();
			var stopwatch = Stopwatch.StartNew();
			var requestUri = $"{serviceUrl}/{request.EndPoint}";

			try
			{
				if (string.IsNullOrWhiteSpace(serviceUrl))
					throw new Exception($"Base Url address is empty for {request.GetType()}");                   
                    
				if (string.IsNullOrWhiteSpace(request.EndPoint))
					throw new Exception($"Request endpoint Url address is empty for {request.GetType()}");
                
				using (var client = new HttpClient())
				{
					client.Timeout = new TimeSpan(0, 0, 15);
                    
					var byteContent = new ByteArrayContent(_serializer.Serialize(request));

					using (var message = await client.PostAsync(requestUri, byteContent))
					{
						if (!message.IsSuccessStatusCode)
						{
							_logger.Error($"SendRequest {request.GetType()} to {requestUri} error ({stopwatch.ElapsedMilliseconds}ms): {message.StatusCode}");                            
							responseObject.ResultCode = ResultCode.SendRequestError;
						}
						else
						{
							var contentStream = await message.Content.ReadAsStreamAsync();
							responseObject = _serializer.DeserializeAs<T>(contentStream);
						}
					}
				}
			}
			catch (Exception e)
			{
				_logger.Error($"SendRequest {request.GetType()}  to {requestUri} error ({stopwatch.ElapsedMilliseconds}ms): {e}");
				responseObject.ResultCode = ResultCode.SendRequestError;                
			}
            
			return responseObject;
		}

		public Task SendRequest<T>(string url, HttpRequestBase request, Action<T> callback) where T : HttpResponseBase, new()
		{
			throw new NotImplementedException();
		}
	}
}



#pragma warning disable 8618
namespace PassMeta.DesktopApp.Common.Models
{
    using System;
    using System.Collections.Generic;
    using Interfaces.Mapping;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    
    /// <summary>
    /// PassMeta server unified response.
    /// </summary>
    public class OkBadResponse
    {
        private string? _message;

        /// <summary>
        /// Response message.
        /// </summary>
        [JsonProperty("message")]
        public string Message
        {
            get => _message!;
            private set
            {
                if (_message is null)
                {
                    Success = value == "OK";
                }
                _message = value;
            }
        }
        
        /// <summary>
        /// If not <see cref="Success"/>, short reason.
        /// </summary>
        [JsonProperty("what")]
        public string? What { get; private set; }
        
        /// <summary>
        /// Sub-responses. Sub-error information list.
        /// </summary>
        [JsonProperty("sub")]
        public List<OkBadResponse>? Sub { get; private set; }
        
        /// <summary>
        /// Additional failure information.
        /// </summary>
        [JsonProperty("more")]
        public OkBadMore? More { get; private set; }

        /// <summary>
        /// Is response success?
        /// </summary>
        public bool Success { get; private set; }

        /// <summary>
        /// Replace response field value with mapped values.
        /// </summary>
        public void ApplyMapping(IMapper<string, string> mapper)
        {
            Message = mapper.Map(Message, Message);
            What = mapper.Map(What, What);
            if (Sub is null) return;
            foreach (var sub in Sub)
            {
                sub.ApplyMapping(mapper);
            }
        }
    }

    /// <summary>
    /// <see cref="OkBadResponse"/> with data.
    /// </summary>
    public class OkBadResponse<TData> : OkBadResponse
    {
        /// <summary>
        /// If <see cref="OkBadResponse.Success"/>, response data.
        /// </summary>
        [JsonProperty("data")]
        public TData? Data { get; init; }
    }
    
    /// <summary>
    /// <see cref="OkBadResponse"/> additional information.
    /// </summary>
    public class OkBadMore
    {
        /// <summary>
        /// Some text message.
        /// </summary>
        [JsonProperty("text")]
        public string? Text { get; init; }
        
        /// <summary>
        /// Json-information.
        /// </summary>
        [JsonProperty("info")]
        public JObject? Info { get; init; }
        
        /// <summary>
        /// Allowed values.
        /// </summary>
        [JsonProperty("allowed")]
        public JArray? Allowed { get; init; }
        
        /// <summary>
        /// Disallowed values.
        /// </summary>
        [JsonProperty("disallowed")]
        public JArray? Disallowed { get; init; }

        /// <summary>
        /// Required fields/values.
        /// </summary>
        [JsonProperty("required")]
        public JArray? Required { get; init; }

        /// <summary>
        /// Minimum valid value.
        /// </summary>
        [JsonProperty("min_allowed")]
        public object? MinAllowed { get; init; }
        
        /// <summary>
        /// Maximum valid value.
        /// </summary>
        [JsonProperty("max_allowed")]
        public object? MaxAllowed { get; init; }

        /// <summary>
        /// Build string with information from notnull fields.
        /// </summary>
        public override string ToString()
        {
            var builder = new List<string>();
            
            if (Text is not null)
                builder.Add(Text);
            if (Info is not null) 
                builder.Add($"{Resources.OKBAD_MORE__INFO}: {Info}");
            if (Allowed is not null) 
                builder.Add($"{Resources.OKBAD_MORE__ALLOWED}: {string.Join(", ", Allowed)}");
            if (Disallowed is not null) 
                builder.Add($"{Resources.OKBAD_MORE__DISALLOWED}: {string.Join(", ", Disallowed)}");
            if (Required is not null) 
                builder.Add($"{Resources.OKBAD_MORE__REQUIRED}: {string.Join(", ", Required)}");
            if (MinAllowed is not null) 
                builder.Add($"{Resources.OKBAD_MORE__MIN_ALLOWED}: {MinAllowed}");
            if (MaxAllowed is not null) 
                builder.Add($"{Resources.OKBAD_MORE__MAX_ALLOWED}: {MaxAllowed}");

            return string.Join(Environment.NewLine, builder);
        }
    }
}
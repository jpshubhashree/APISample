﻿using Newtonsoft.Json;

namespace WebApiWrapper.Model
{
    public class ErrorResult
    {
        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("error_description")]
        public string ErrorDescription { get; set; }
    }
}
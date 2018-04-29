﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Magic.Models
{
    public class ResponseEnum
    {
        [JsonProperty("id")]
        public int Id;

        [JsonProperty("label")]
        public string Label;
    }
}
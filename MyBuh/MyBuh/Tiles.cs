﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuhLib;

namespace MyBuh
{
    public class SingleTile
    {
        public string Title { get; set; }
        public string ImageUri { get; set; }

        public SingleTile(string title, string uri)
        {
            Title = title;
            ImageUri = uri;
        }
    }
        

}

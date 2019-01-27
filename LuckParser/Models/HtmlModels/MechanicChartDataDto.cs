﻿using System.Collections.Generic;

namespace LuckParser.Models.HtmlModels
{  
    public class MechanicChartDataDto
    {       
        public string Symbol;     
        public int Size;
        public string Color;       
        public List<List<List<object>>> Points;      
        public bool Visible;
    }
}

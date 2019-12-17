//Interneuron Synapse

//Copyright(C) 2019  Interneuron CIC

//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.

//See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with this program.If not, see<http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;

namespace Interneuron.Autonomic.Models
{
    public class EWSResponse
    {
        public string observationevent_id { get; set; }
        public string score_id { get; set; }
        public string status { get; set; } 
        public int? score { get; set; }
        public string score_type { get; set; }
        public DateTime score_datetime { get; set; }
        public string guidance { get; set; }
        public string error { get; set; }
        //public List<EWSResponseScoreParameters> parameters { get; set; }
    }

    public class EWSResponseScoreParameters
    {
        public string type { get; set; }
        public int score { get; set; }
        public string value { get; set; }
    }
}

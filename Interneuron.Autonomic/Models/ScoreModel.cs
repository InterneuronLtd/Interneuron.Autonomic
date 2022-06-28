//BEGIN LICENSE BLOCK 
//Interneuron Autonomic

//Copyright(C) 2022  Interneuron CIC

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
//END LICENSE BLOCK 


using System;

namespace Interneuron.Autonomic.Models
{
    public class Score
    {
        public string score_id { get; set; }
        public string person_id { get; set; }
        public string encounter_id { get; set; }
        public string observationevent_id { get; set; }
        public int? score { get; set; }
        public string guidance { get; set; }
        public DateTime calculateddatetime { get; set; }
        public string calculatedby { get; set; }
        public string calculatedsystem { get; set; }
        public string scoretype { get; set; }
    }

    public class ScoreParameter
    {
        public string scoreparameter_id { get; set; }
        public string score_id { get; set; }
        public string observationevent_id { get; set; }
        public string parameter { get; set; }
        public int score { get; set; }
        public string value { get; set; }
    }
}

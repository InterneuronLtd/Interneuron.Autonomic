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
    public class ObservationEvent
    {

        public string observationevent_id { get; set; }
        public string person_id { get; set; }
        public string encounter_id { get; set; }
        public DateTime? datestarted { get; set; }
        public DateTime? datefinished { get; set; }
        public Double? resp { get; set; } //Respiratory Rate
        public string respunits { get; set; }
        public Double? spo2 { get; set; } //Spo2
        public string spo2units { get; set; }
        public bool? onoxygen {get; set;}
        public Double? oxygenperc {get; set;}
        public Double? oxygenlpm {get; set;}
        public Double? bps { get; set; } //Systolic BP
        public string bpsunits { get; set; }
        public Double? pulse { get; set; } //Pulse
        public string pulseunits { get; set; }
        public string acvpu { get; set; } //Conscious Level
        public Double? temp { get; set; } //Temp
        public string tempunits { get; set; }
        public string scaletype { get; set; }
        public string score_id { get; set; }

        //PEWS specific
        public bool? concern { get; set; } //
        public string respdistress { get; set; }
        public Double? hr { get; set; }

    }
}
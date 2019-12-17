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
using Interneuron.Autonomic.Models;
using Interneuron.Autonomic.Services;
using InterneuronAutonomic.API;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Interneuron.Autonomic.Controllers
{
    [Authorize]
    [Route("/Autonomic/EWS/")]
    [ApiController]
    public class EWSController : ControllerBase
    {
        DynamicAPIClient _dynamicAPIClient;
        public EWSController(DynamicAPIClient dynamicAPIClient)
        {
            _dynamicAPIClient = dynamicAPIClient;
        }

        [HttpGet]
        [Route("[action]/{observationEventId}")]
        public ActionResult<string> CalculateNEWS2(string observationEventId)
        {

            // Get Observation Event via autonomic_observationevent baseview and the DynamicAPI client.

            string obsEventJson = _dynamicAPIClient.GetBaseViewListObjectByAttribute("autonomic_observationevent", "observationevent_id", observationEventId);

            // Deserialize json to ObservationEvent model
            ObservationEvent obsEvent = JsonConvert.DeserializeObject<ObservationEvent>(obsEventJson);

            // Invoke the EWSCalculator service which returns an EWSResponse
            EWSResponse ewsResponse = EWSCalculator.CalculateNEWS2Score(obsEvent);

            var useridClaim = User.FindFirst("IPUId");
            var userId = "unknown";

            if (useridClaim != null)
            {
                userId = useridClaim.Value;
            }   

            if (ewsResponse.status == "SUCCESS")
            {
                //Create Score to persist.
                Score ewsScore = new Score();
                ewsScore.score_id = ewsResponse.score_id;
                ewsScore.person_id = obsEvent.person_id;
                ewsScore.encounter_id = ""; //TODO
                ewsScore.observationevent_id = obsEvent.observationevent_id;
                ewsScore.score = ewsResponse.score;
                ewsScore.scoretype = ewsResponse.score_type;
                ewsScore.guidance = ewsResponse.guidance;
                ewsScore.calculatedby = userId;
                ewsScore.calculateddatetime = ewsResponse.score_datetime;
                ewsScore.calculatedsystem = "Interneuron.Autonomic";

                string postResponse = _dynamicAPIClient.PostObject("core", "score", ewsScore);
            }

            return JsonConvert.SerializeObject(ewsResponse);
        }
    }
}
